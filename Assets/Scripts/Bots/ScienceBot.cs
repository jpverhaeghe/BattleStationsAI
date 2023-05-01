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

    // constant variables for this bot - moved to data slider for user input
    //private const int MAX_POWER_LEVEL_TO_REQUEST = 4;               // the maximum power level - don't request more when it is above here

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
        moduleToActOn = null;
        BotStates nextState = BotStates.MOVE;
        int currentShipShieldsLevel = myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.SHIELDS];

        // booleans for decision making
        bool brokenModule = false;
        bool isScanning = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int moduleToRepairId = 0;
        int numBrokenModules = 0;
        int mostNumUsedMarkers = int.MaxValue;

        // keep track of broken science bays, if all are broken, no more shields!
        int numScienceModules = 0;
        int numBrokenScienceModules = 0;

        List<RoomInfo> potentialModules = new List<RoomInfo>();
        List<int> moduleIds = new List<int>();
        int moduleToMoveTo = currentModule;
        int terminalToMoveTo = currentTerminal;

        for (int moduleId = 0; moduleId < myModules.Count; moduleId++)
        {
            if (myModules[moduleId].moduleType == RoomData.ModuleType.Science)
            {
                numScienceModules++;
            }

            if (myModules[moduleId].IsBroken())
            {
                brokenModule = true;
                moduleNeedingRepairs = myModules[moduleId];
                moduleToRepairId = moduleId;
                numBrokenModules++;

                if (myModules[moduleId].moduleType == RoomData.ModuleType.Science)
                {
                    numBrokenScienceModules++;
                }
            }
            else
            {
                // only acting on science modules for now
                if (myModules[moduleId].moduleType == RoomData.ModuleType.Science)
                {
                    // go through each terminal to find the one we can act and preference those that are less used
                    if (myModules[moduleId].GetNumUsedMarkers() < mostNumUsedMarkers)
                    {
                        // put the lower used marker ones at the front of the list
                        potentialModules.Insert(0, myModules[moduleId]);
                        moduleIds.Insert(0, moduleId);
                        mostNumUsedMarkers = myModules[moduleId].GetNumUsedMarkers();
                    }
                    else
                    {
                        potentialModules.Add(myModules[moduleId]);
                        moduleIds.Add(moduleId);
                    }
                }
            }
        }

        // now go through all the potential modules (should be in best option wit used markers order) to see which one is not occupied
        // or is the one we are currently working on
        for (int listIndex = 0; listIndex < potentialModules.Count; listIndex++)
        {
            // only go through the modules that aren't full (except the one we are currently on) until one we can act on is found
            if (!potentialModules[listIndex].IsFullyOccupied() || (moduleIds[listIndex] == currentModule))
            {
                moduleToActOn = myModules[moduleIds[listIndex]];
                moduleToMoveTo = moduleIds[listIndex];
                terminalToMoveTo = myModules[moduleIds[listIndex]].GetUnoccupiedTerminal();
                break;
            }
        }

        // no need to move if we are in the current module, so stay put for actions
        if (moduleToMoveTo == currentModule)
        {
            terminalToMoveTo = currentTerminal;
        }

        if (numScienceModules > numBrokenScienceModules)
        {
            if (myShip.currentTarget != null)
            {
                isScanning = PerformScan();
            }

            // else request energy if we are not firing and are not at our best power for weapons
            // (Cannon hullDamage is more effective with more power)
            if (!isScanning && (currentShipShieldsLevel < gameManagerScript.scienceMinPowerLevel.value) &&
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
            terminalToMoveTo = 0;
            actionToTake = ScienceActions.REPAIR;
        }
        else if (actionToTake == ScienceActions.WAIT)
        {
            moduleToActOn = null;
        }

        // if there is a module we are acting on, we need to set module and terminal as occupied and release the one we are on
        if (moduleToActOn != null)
        {
            myModules[currentModule].SetTerminalOccupied(currentTerminal, false);

            if (actionToTake == ScienceActions.REPAIR)
            {
                currentModule = moduleToRepairId;
            }
            else
            {
                currentModule = moduleToMoveTo;
            }

            currentTerminal = terminalToMoveTo;
            myModules[currentModule].SetTerminalOccupied(currentTerminal, true);
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
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Ship scans successfully adjusted by " + adjustmentLevel);
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
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "ECM successfully launched");
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
        if ((moduleToActOn != null) && (myShip.numStoredScans < myShip.shipSize))
        {
            // set it at one scan for now
            adjustmentLevel = 1;

            // calculate the difficulty - as scans are used for re-rolling checks, we want as many as we can have
            // Difficulty is distance to scanned object / 2 (rounded up) + 3 for each additional scan this action
            actionDifficulty = (int)Mathf.Ceil(GetDistanceToTarget() / 2.0f);
            actionDifficulty += moduleToActOn.GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;

            // if we can scan more, we should - re-rolls are good!
            int difficulty = AttemptHigherDifficulty(actionDifficulty);
            if (actionDifficulty != difficulty)
            {
                adjustmentLevel++;
            }

            if (actionDifficulty <= gameManagerScript.botDifficultyCheckSlider.value)
            {
                actionToTake = ScienceActions.SCAN;
                isScanning = true;
            }
        }

        return isScanning;

    } // PerformScan
}
