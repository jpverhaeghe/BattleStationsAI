using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineeringBot : GenericBot
{
    private enum EngineerActions
    {
        PUMP_ENGINES,
        TRANSFER_POWER,
        REPAIR,
        WAIT
    }

    // constant variables for this bot
    public const int ENGINE_CHANGE_BASE_DIFFICULTY = 8;         // the minimum difficulty for pumping an engine
    public const int MIN_POWER_LEVELS_TO_TRANSFER = 5;          // the minimum total power before performing a transfer

    // switched to a queue system...so not using this heuristic
    //private static float HELM_POWER_PREFERENCE = 0.2f;
    //private static float WEAPONS_POWER_PREFERENCE = 0.5f;
    //private static float SHIELDS_POWER_PREFERENCE = 1 - (HELM_POWER_PREFERENCE + WEAPONS_POWER_PREFERENCE);

    // private variables
    private EngineerActions actionToTake;
    private int adjustmentLevel;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // engineering bots profession is engineering, they can work on engineering modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        engineering = PROFESSION_SKILL_VALUE;
        myType = BotType.ENGINEERING;


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
        actionToTake = EngineerActions.WAIT;
        moduleToActOn = null;

        // booleans for decision making
        bool brokenModule = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int numBrokenModules = 0;
        int mostNumUsedMarkers = int.MaxValue;

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
                if (module.GetNumUsedMarkers() < mostNumUsedMarkers)
                {
                    moduleToActOn = module;
                    mostNumUsedMarkers = module.GetNumUsedMarkers();
                }
            }
        }

        AdjustShipPowerLevels();

        // regardless of previous actions, all modules are broken or
        // any modules are broken and we are not adjusting power, switch to repair
        if ((numBrokenModules >= myModules.Count) || ((actionToTake == EngineerActions.WAIT) && brokenModule) )
        {
            moduleToActOn = moduleNeedingRepairs;
            actionToTake = EngineerActions.REPAIR;
        }
        else if (actionToTake == EngineerActions.WAIT)
        {
            moduleToActOn = null;
        }

        // pause for a bit then move on (eventually will remove this when it is a turn based game)
        runningState = true;
        yield return new WaitForSeconds(1);
        currentState = BotStates.MOVE;
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
            case EngineerActions.PUMP_ENGINES:
                // Attempt to pump the engines and apply the power first request in the queue
                if (PerformActionCheck(actionDifficulty))
                {
                    GeneratedShip.ShipPowerAreas areaToAdjust = myShip.energyUpdateQueue.Dequeue();

                    myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)areaToAdjust, adjustmentLevel);
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Engines pumped successfully, added " + adjustmentLevel + " power to " + areaToAdjust.ToString() + "!");                    
                    //Debug.Log("Engines pumped successfully, added " + adjustmentLevel + " power to " + areaToAdjust.ToString() + "!");
                }

                // used marker is added on success or failure
                moduleToActOn.AddUsedMarkers(1);
                break;

            case EngineerActions.TRANSFER_POWER:
                // if the action succeeds, find the area to transfer to and pull from any of the others (based on bigger levels)
                if (PerformActionCheck(actionDifficulty))
                {
                    TransferPowerLevels();
                }
                break;

            case EngineerActions.REPAIR:
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

    private void AdjustShipPowerLevels()
    {
        //	Add energy based on priority(Queue system)
        //	 - Pump engines to add one level of power to any power level type
        //   - Base difficulty is 8 - +3 for every used marker
        //	 - Can add 3 to difficulty to add another power level(to any system)
        //	 - Add used counter to engine once pumped action is resolved(success or failure)
        //	Can transfer power at difficulty of 8 +3 for each additional power transferred - this action does not put a used marker on the engine
        // only adjust the power levels if we have a working module (set in the calling method)
        if (moduleToActOn != null)
        {
            // check to see what power levels need adjusting if any
            // TODO: (may just randomly add power later if this is too slow)
            if (myShip.energyUpdateQueue.Count > 0)
            {
                // first try to pump the engines as more energy is better
                adjustmentLevel = 1;

                // depending on difficulty, pump engines by one or more - filling the areas of the queue
                actionDifficulty = ENGINE_CHANGE_BASE_DIFFICULTY;
                actionDifficulty += moduleToActOn.GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;
                actionDifficulty -= engineering;

                int difficulty = AttemptHigherDifficulty(actionDifficulty);
                if (actionDifficulty != difficulty)
                {
                    adjustmentLevel++;
                }

                // if the difficulty is too difficult, perhaps transferring instead if available as it ignores used markers
                if (actionDifficulty > MAX_DIFFICULTY_TO_TEST)
                {
                    // only transfer if the combined energy levels are high enough
                    int currentEnergyTotal = 0;

                    for (int energyType = 0; energyType < myShip.energySystemLevels.Length; energyType++)
                    {
                        currentEnergyTotal += myShip.energySystemLevels[energyType];
                    }

                    if (currentEnergyTotal >= MIN_POWER_LEVELS_TO_TRANSFER)
                    {
                        actionDifficulty = ENGINE_CHANGE_BASE_DIFFICULTY;
                        actionDifficulty -= engineering;

                        /*difficulty = AttemptHigherDifficulty(actionDifficulty);
                        if (actionDifficulty != difficulty)
                        {
                            adjustmentLevel++;
                        }*/

                        if (actionDifficulty <= MAX_DIFFICULTY_TO_TEST)
                        {
                            actionToTake = EngineerActions.TRANSFER_POWER;
                        }
                    }
                }
                else
                {
                    actionToTake = EngineerActions.PUMP_ENGINES;
                }
            }
            // may need to just add a level to an area by default, as it will be crucial later
        }

    } // AdjustShipPowerLevels

    /// <summary>
    /// If there are energy levels available, transfer form one. 
    /// Shields is the first to take from, then helm, last is weapons
    /// </summary>
    private void TransferPowerLevels()
    {
        GeneratedShip.ShipPowerAreas areaToAdjust = myShip.energyUpdateQueue.Dequeue();
        GeneratedShip.ShipPowerAreas areaToTake = GeneratedShip.ShipPowerAreas.SHIELDS;
        bool transferAllowed = true;

        switch (areaToAdjust)
        {
            case GeneratedShip.ShipPowerAreas.HELM:
                if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.SHIELDS] < adjustmentLevel)
                {
                    areaToTake = GeneratedShip.ShipPowerAreas.WEAPONS;
                    if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS] < adjustmentLevel)
                    {
                        transferAllowed = false;
                    }
                }
                break;

            case GeneratedShip.ShipPowerAreas.WEAPONS:
                if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.SHIELDS] < adjustmentLevel)
                {
                    areaToTake = GeneratedShip.ShipPowerAreas.HELM;
                    if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.HELM] < adjustmentLevel)
                    {
                        transferAllowed = false;
                    }
                }
                break;

            case GeneratedShip.ShipPowerAreas.SHIELDS:
                areaToTake = GeneratedShip.ShipPowerAreas.HELM;
                if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.HELM] < adjustmentLevel)
                {
                    areaToTake = GeneratedShip.ShipPowerAreas.WEAPONS;
                    if (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS] < adjustmentLevel)
                    {
                        transferAllowed = false;
                    }
                }
                break;

            default:
                break;
        }

        if (transferAllowed)
        {
            myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)areaToAdjust, adjustmentLevel);
            myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)areaToTake, -adjustmentLevel);

            myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, adjustmentLevel + " power transfered from " + areaToTake.ToString() + " to " + areaToAdjust.ToString() + "!");
            //Debug.Log(adjustmentLevel + " power transfered from " + areaToTake.ToString() + " to " + areaToAdjust.ToString() + "!");
        }

    } // end TransferPowerLevels
}