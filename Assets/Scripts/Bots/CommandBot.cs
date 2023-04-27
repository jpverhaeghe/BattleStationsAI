using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBot : GenericBot
{
    private enum CommandActions
    {
        ADJUST_SPEED,
        STEER,
        //SIDESLIP,
        REQUEST_HELM_POWER,
        REPAIR,
        WAIT
    }

    // constant variables for this bot
    private const int MIN_DIST_TO_ADD_SPEED = 6;                    // the distance where we add speed or start braking...
    private const int MAX_POWER_LEVEL_TO_REQUEST = 4;               // the maximum power level - don't request more when it is here

    // private variables
    private CommandActions actionToTake;
    private int adjustmentLevel;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // command bots profession is piloting, they can control the ship from the helm well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        piloting = PROFESSION_SKILL_VALUE;
        myType = BotType.COMMAND;

    } // end Start

    /// <summary>
    /// Runs a generic idle state where it waits for a second before seting up a move state 
    /// (where the action choice takes place)
    /// </summary>
    /// <returns>yields the system until done with the wait, then finishes the state</returns>
    protected override IEnumerator PerformIdleState()
    {
        // choose the action to take based on several things (using heuristics to choose priority of base options)
        // - Is the module broken or slagged - repair it (takes precedence if there are no other modules)
        // - is there a request for power - do in priority (weapons, shields, helm)
        // - TODO: This bot is better at repairs so it may take other modules into account for reparing later

        // default action will be to wait
        actionToTake = CommandActions.WAIT;
        moduleToActOn = null;
        BotStates nextState = BotStates.MOVE;
        int currentShipHelmLevel = myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.HELM];

        // booleans for decision making
        bool brokenModule = false;
        bool isManeurving = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int numBrokenModules = 0;

        foreach (RoomInfo module in myModules)
        {
            if (module.IsBroken())
            {
                brokenModule = true;
                moduleNeedingRepairs = module;
                numBrokenModules++;
            }
            else
            {
                // there is only one command module at the moment, so have the command bot move terminals if it can
                currentTerminal = module.GetUnoccupiedTerminal();

                if (currentTerminal >= 0)
                {
                    moduleToActOn = module;
                }
            }
        }

        // only maneuver if there is a target
        if (myShip.currentTarget != null)
        {
            isManeurving = CheckManeuvers();
        }

        // else request energy if we are not firing and are not at our best power for weapons
        // (Cannon hullDamage is more effective with more power)
        if (!isManeurving && (currentShipHelmLevel < MAX_POWER_LEVEL_TO_REQUEST) &&
            (currentShipHelmLevel < GeneratedShip.MAX_ENERGY_LEVEL))
        {
            actionToTake = CommandActions.REQUEST_HELM_POWER;

            // this action can skip moving, as it is just a request to another ship bot
            nextState = BotStates.ACTING;
        }

        // regardless of previous actions, all modules are broken or
        // any modules are broken and we are not adjusting power, switch to repair
        if ((numBrokenModules >= myModules.Count) || (!isManeurving && brokenModule))
        {
            moduleToActOn = moduleNeedingRepairs;
            currentTerminal = 0;
            actionToTake = CommandActions.REPAIR;
        }
        else if (actionToTake == CommandActions.WAIT)
        {
            moduleToActOn = null;
        }

        // pause for a bit then move on (eventually will remove this when it is a turn based game)
        runningState = true;
        yield return new WaitForSeconds(1);
        currentState = nextState;
        runningState = false;

    } // end PerformIdleState

    /// <summary>
    /// Will perform a given action at the location
    /// - will need to set this up in idle perhaps - using heuristics to determine the best option for the bot at that time
    /// </summary>
    /// <returns>yields the system until done with the wait, then finishes the state<</returns>
    protected override IEnumerator DoAction()
    {
        // Do the selected action from previous Idle state
        switch (actionToTake)
        {
            case CommandActions.ADJUST_SPEED:
                // Attempt to pump the engines and apply the power first request in the queue
                if (PerformActionCheck(actionDifficulty))
                {
                    myShip.shipManagerScript.UpdateSpeed(myShip.shipID, adjustmentLevel);
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Speed successfully adjusted by " + adjustmentLevel);
                    //Debug.Log("Speed successfully adjusted by " + adjustmentLevel);
                }

                // used marker is added on success or failure - no used markers on the helm
                //moduleToActOn.AddUsedMarkers(1);
                myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)GeneratedShip.ShipPowerAreas.HELM, -1);
                break;

            case CommandActions.STEER:
                // if the action succeeds, find the area to transfer to and pull from any of the others (based on bigger levels)
                if (PerformActionCheck(actionDifficulty))
                {
                    myShip.shipManagerScript.UpdateShipDirection(myShip.shipID, adjustmentLevel);
                    myShip.requestedFacing = -1;

                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Direction successfully adjusted by " + adjustmentLevel);
                    //Debug.Log("Direction successfully adjusted by " + adjustmentLevel);
                }

                // used marker is added on success or failure - no used markers on the helm
                //moduleToActOn.AddUsedMarkers(1);
                myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)GeneratedShip.ShipPowerAreas.HELM, -1);
                break;

            case CommandActions.REQUEST_HELM_POWER:
                myShip.energyUpdateQueue.Enqueue(GeneratedShip.ShipPowerAreas.HELM);
                break;

            case CommandActions.REPAIR:
                // attempt a repair
                AttemptRepair(moduleToActOn);
                break;

            default:
                break;
        }

        runningState = true;
        yield return new WaitForSeconds(0.5f);
        currentState = BotStates.IDLE;
        runningState = false;

    } // end DoAction

    // Checks to see if there are maneuvers available and acts on them as it see fit
    private bool CheckManeuvers()
    {
        bool isManeuvering = false;

        // all maneuvers are difficulty (2x Ship Size + 2x Speed - Piloting Skill) 
        // helm doesn't have use markers but will have OOC penalties (not using for this project)
        actionDifficulty = (2 * myShip.shipSize) + (2 * myShip.currentSpeed);
        actionDifficulty -= piloting;

        // check to see if we are facing our target first, if not, time to turn
        Vector2Int targetPos = myShip.currentTarget.mapLocation;
        int distanceToTarget = GetDistanceToTarget();
        bool facingTarget = false;

        // check to see if it is 
        if (((myShip.currentDirection == 0) && (myShip.mapLocation.x < targetPos.x)) ||
            ((myShip.currentDirection == 90) && (myShip.mapLocation.y < targetPos.y)) ||
            ((myShip.currentDirection == 180) && (myShip.mapLocation.x > targetPos.x)) ||
            ((myShip.currentDirection == 270) && (myShip.mapLocation.y > targetPos.y)))
        {
            facingTarget = true;
        }

        if (!facingTarget)
        {
            isManeuvering = true;
            actionToTake = CommandActions.STEER;
            adjustmentLevel = SINGLE_DIRECTION_CHANGE;
        }
        // first off, check the distance from the other ship. We can't shoot it effectively if we aren't close enough
        else if (distanceToTarget != MIN_DIST_TO_ADD_SPEED)
        {
            if ((distanceToTarget > MIN_DIST_TO_ADD_SPEED) &&
                    (myShip.currentSpeed < MAX_SPEED))
            {
                isManeuvering = true;
                actionToTake = CommandActions.ADJUST_SPEED;
                adjustmentLevel = 1;
            }

            if ((distanceToTarget < MIN_DIST_TO_ADD_SPEED) &&
                    (myShip.currentSpeed > 0))
            {
                isManeuvering = true;
                actionToTake = CommandActions.ADJUST_SPEED;
                adjustmentLevel = -1;
            }

            // potential to adjust further but with OOC penalties bots play it safe
        }

        // speed comes first, if not, then check to see if a turn was requested
        if (!isManeuvering && (myShip.requestedFacing != -1))
        {
            isManeuvering = true;
            actionToTake = CommandActions.STEER;
            adjustmentLevel = myShip.requestedFacing;
        }

        return isManeuvering;

    } // end CheckManeuvers
}
