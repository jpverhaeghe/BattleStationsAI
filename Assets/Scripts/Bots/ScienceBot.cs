using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceBot : GenericBot
{
    private enum ScienceActions
    {
        SCAN,
        LAUNCH_ECM,
        REQUEST_SHIELD_POWER,
        REPAIR,
        WAIT
    }
    // constant variables for this bot
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Science, RoomData.ModuleType.Hyperdrive };

    private const int MIN_POWER_LEVEL_TO_REQUEST = 2;               // the minimum power level - request more when it is below here
    private const int MAX_POWER_LEVEL_TO_REQUEST = 4;               // the maximum power level - don't request more when it is above here

    // private variables
    private ScienceActions actionToTake;
    private int adjustmentLevel;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // science bots profession is science, they can work on science modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        science = PROFESSION_SKILL_VALUE;
        myType = BotType.SCIENCE;

    } // end Start

    /// <summary>
    /// Update is called once per frame - for now just calling the base class
    /// </summary>
    void Update()
    {
        base.Update();

    } // end Update

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
        actionToTake = ScienceActions.WAIT;
        BotStates nextState = BotStates.MOVE;
        int currentShipShieldsLevel = myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.SHIELDS];

        // booleans for decision making
        bool brokenModule = false;
        bool isScanning = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int numBrokenModules = 0;

        // keep track of broken science bays, if all are broken, no more shields!
        int numScienceModules = 0;
        int numBrokenScienceModules = 0;

        foreach (RoomInfo module in myModules)
        {
            if (module.moduleType == RoomData.ModuleType.Science)
            {
                numScienceModules++;
            }

            if (module.IsBroken())
            {
                brokenModule = true;
                moduleNeedingRepairs = module;
                numBrokenModules++;

                if (module.moduleType == RoomData.ModuleType.Science)
                {
                    numBrokenScienceModules++;
                }
            }
            else
            {
                // only acting on science modules for now
                if (module.moduleType == RoomData.ModuleType.Science)
                { 
                    moduleToActOn = module; 
                }
            }
        }

        if (numScienceModules > numBrokenScienceModules)
        {
            isScanning = PerformScan();

            // else request energy if we are not firing and are not at our best power for weapons
            // (Cannon hullDamage is more effective with more power)
            if (!isScanning && (currentShipShieldsLevel < MAX_POWER_LEVEL_TO_REQUEST) &&
                (currentShipShieldsLevel < GeneratedShip.MAX_ENERGY_LEVEL))
            {
                actionToTake = ScienceActions.REQUEST_SHIELD_POWER;

                // this action can skip moving, as it is just a request to another ship bot
                nextState = BotStates.ACTING;
            }
        }
        else
        {
            // update the ship so it knows to not allow shields to increase in power
            // TODO: when adding second broken state - slagged - shields will lose all power
            myShip.allScienceBaysBroken = true;
        }

        // regardless of previous actions, all modules are broken or
        // any modules are broken and we are not adjusting power, switch to repair
        // (science repairs if all science modules are broken!)
        if ((numBrokenModules >= myModules.Count) || (numScienceModules <= numBrokenScienceModules) || 
            ((actionToTake == ScienceActions.WAIT) && brokenModule))
        {
            moduleToActOn = moduleNeedingRepairs;
            actionToTake = ScienceActions.REPAIR;
        }
        else if (actionToTake == ScienceActions.WAIT)
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
            case ScienceActions.SCAN:
                // Attempt to pump the engines and apply the power first request in the queue
                if (PerformActionCheck(actionDifficulty))
                {
                    myShip.shipManagerScript.UpdateNumScans(myShip.shipID, adjustmentLevel);
                    myShip.shipManagerScript.UpdateBotStatusText("Ship scans successfully adjusted by " + adjustmentLevel);
                    //Debug.Log("Ship scans successfully adjusted by " + adjustmentLevel);
                }

                // used marker is added on success or failure - no used markers on the helm
                moduleToActOn.AddUsedMarkers(1);
                break;

            case ScienceActions.LAUNCH_ECM:
                // TODO: Will add this later - when missiles get added
                // if the action succeeds, find the area to transfer to and pull from any of the others (based on bigger levels)
                if (PerformActionCheck(actionDifficulty))
                {
                    myShip.shipManagerScript.UpdateBotStatusText("ECM successfully launched");
                    //Debug.Log("ECM successfully launched");
                }

                // used marker is added on success or failure - no used markers on the helm
                moduleToActOn.AddUsedMarkers(1);
                break;

            case ScienceActions.REQUEST_SHIELD_POWER:
                myShip.energyUpdateQueue.Enqueue(GeneratedShip.ShipPowerAreas.SHIELDS);
                break;

            case ScienceActions.REPAIR:
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

    private bool PerformScan()
    {
        bool isScanning = false;

        // only scan if we don't have the maximum number already
        if (myShip.numStoredScans < myShip.shipSize)
        {
            // set it at one scan for now
            adjustmentLevel = 1;

            // calculate the difficulty - as scans are used for re-rolling checks, we want as many as we can have
            // Difficulty is distance to scanned object / 2 (rounded up) + 3 for each additional scan this action
            actionDifficulty = (int)Mathf.Ceil(myShip.shipManagerScript.botTargetPractice.distance / 2.0f);
            actionDifficulty += moduleToActOn.GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;

            // if we can scan more, we should - re-rolls are good!
            int difficulty = AttemptHigherDifficulty(actionDifficulty);
            if (actionDifficulty != difficulty)
            {
                adjustmentLevel++;
            }

            if (actionDifficulty <= MAX_DIFFICULTY_TO_TEST)
            {
                actionToTake = ScienceActions.SCAN;
                isScanning = true;
            }
        }

        return isScanning;

    } // PerformScan
}
