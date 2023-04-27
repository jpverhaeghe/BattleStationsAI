using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBot : GenericBot
{
    private enum SecurityActions
    {
        FIRE_CANNON,
        FIRE_MISSILE,
        REQUEST_WEAPON_POWER,
        REPAIR,
        //RE_CONFIGURE_CANNON,           // may add later if time - will need to be in final game
        WAIT
    }

    // constant variables for this bot
    public const int SPEED_MULTIPLIER = 2;                          // the speed multiplier of the other ship
    public const int CANNON_FIRING_ARC = 180;                       // the area the cannon can hit from its facing (may not use)
    public const int MIN_POWER_LEVEL_TO_FIRE = 2;                   // the minimum power level we will need to fire a weapon
    public const int MAX_POWER_LEVEL_TO_REQUEST = 4;                // the maximum power level - don't request more when it is here
    public const int MISSILE_BASE_DIFFICULTY = 11;                  // the minimum difficulty for firing a missile

    public const float CANNON_PREFERENCE = 0.70f;
    public const float MISSILE_PREFERENCE = 1 - CANNON_PREFERENCE;  

    // private variables
    private SecurityActions actionToTake;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // security bots profession is combat, they can work on combat modules and general combat well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = PROFESSION_SKILL_VALUE;
        myType = BotType.SECURITY;

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
        // - Is there enough power to fire - we want a minimum value for to fire a cannon which is immediate hullDamage
        // - Is there clear line of sight (for cannons - if a cannon is the only thing available)
        // - Is there a chance of success

        // default action will be to wait
        actionToTake = SecurityActions.WAIT;
        moduleToActOn = null;
        BotStates nextState = BotStates.MOVE;
        int currentShipWeaponsLevel = myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS];

        // booleans for decision making
        bool brokenModule = false;
        bool isFiring = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int moduleToRepairId = 0;
        int numBrokenModules = 0;
        int mostNumUsedCannonMarkers = int.MaxValue;
        int mostNumUsedMissileMarkers = int.MaxValue;

        // track modules for firing
        List<RoomInfo> cannonsThatCanFire = new List<RoomInfo>();
        List<int> cannonsModuleIds = new List<int>();
        RoomInfo cannonToFire = null;
        int cannonModuleId = 0;
        int cannonTerminalId = 0;

        List<RoomInfo> missileThatCanFire = new List<RoomInfo>();
        List<int> missileModuleIds = new List<int>();
        RoomInfo missileToFire = null;
        int missileModuleId = 0;
        int missileTerminalId = 0;

        int moduleToMoveTo = currentModule;
        int terminalToMoveTo = currentTerminal;

        for (int moduleId = 0; moduleId < myModules.Count; moduleId++)
        {
            if (myModules[moduleId].IsBroken())
            {
                brokenModule = true;
                moduleNeedingRepairs = myModules[moduleId];
                moduleToRepairId = moduleId;
                numBrokenModules++;
            }
            else
            {
                // go through each terminal to find the one we can act and preference those that are less used
                if (myModules[moduleId].moduleType == RoomData.ModuleType.Cannon)
                {
                    if (myModules[moduleId].GetNumUsedMarkers() < mostNumUsedCannonMarkers)
                    {
                        // put the lower used marker ones at the front of the list
                        cannonsThatCanFire.Insert(0, myModules[moduleId]);
                        cannonsModuleIds.Insert(0, moduleId);
                        mostNumUsedCannonMarkers = myModules[moduleId].GetNumUsedMarkers();
                    }
                    else
                    {
                        cannonsThatCanFire.Add(myModules[moduleId]);
                        cannonsModuleIds.Add(moduleId);
                    }
                }

                if (myModules[moduleId].moduleType == RoomData.ModuleType.MissileBay)
                {
                    if (myModules[moduleId].GetNumUsedMarkers() < mostNumUsedMissileMarkers)
                    {
                        // put the lower used marker ones at the front of the list
                        missileThatCanFire.Insert(0, myModules[moduleId]);
                        missileModuleIds.Insert(0, moduleId);
                        mostNumUsedMissileMarkers = myModules[moduleId].GetNumUsedMarkers();
                    }
                    else
                    {
                        missileThatCanFire.Add(myModules[moduleId]);
                        missileModuleIds.Add(moduleId);
                    }
                }
            }
        }

        // only go to fire a weapon if there is energy
        if ((myShip.currentTarget != null) && (currentShipWeaponsLevel >= MIN_POWER_LEVEL_TO_FIRE))
        {
            // now go through all the potential cannon modules (should be in best option with used markers order) to see which one is not occupied
            // or is the one we are currently working on
            for (int listIndex = 0; listIndex < cannonsThatCanFire.Count; listIndex++)
            {
                // only go through the modules that aren't full (except the one we are currently on) until one we can act on is found
                if (!cannonsThatCanFire[listIndex].IsFullyOccupied() || (cannonsModuleIds[listIndex] == currentModule))
                {
                    cannonToFire = myModules[cannonsModuleIds[listIndex]];
                    cannonModuleId = cannonsModuleIds[listIndex];

                    // only update the terminal if it is different - all weapon modules only have one terminal,
                    // so it would not find an unoccupied terminal as this bot is in it!
                    if (cannonsModuleIds[listIndex] != currentModule)
                    {
                        cannonTerminalId = myModules[cannonsModuleIds[listIndex]].GetUnoccupiedTerminal();
                    }
                    break;
                }
            }

            // now go through all the potential missile modules (should be in best option with used markers order) to see which one is not occupied
            // or is the one we are currently working on
            for (int listIndex = 0; listIndex < missileThatCanFire.Count; listIndex++)
            {
                // only go through the modules that aren't full (except the one we are currently on) until one we can act on is found
                if (!missileThatCanFire[listIndex].IsFullyOccupied() || (missileModuleIds[listIndex] == currentModule))
                {
                    missileToFire = myModules[missileModuleIds[listIndex]];
                    missileModuleId = missileModuleIds[listIndex];

                    // only update the terminal if it is different - all weapon modules only have one terminal,
                    // so it would not find an unoccupied terminal as this bot is in it!
                    if (missileModuleIds[listIndex] != currentModule)
                    {
                        missileTerminalId = myModules[missileModuleIds[listIndex]].GetUnoccupiedTerminal();
                    }
                    break;
                }
            }

            isFiring = FireWeapon(cannonToFire, cannonModuleId, cannonTerminalId, missileToFire, missileModuleId, missileTerminalId);
        }

        // else request energy if we are not firing and are not at our best power for weapons
        // (Cannon hullDamage is more effective with more power)
        if (!isFiring && (currentShipWeaponsLevel < MAX_POWER_LEVEL_TO_REQUEST) &&
            (currentShipWeaponsLevel < GeneratedShip.MAX_ENERGY_LEVEL))
        {
            actionToTake = SecurityActions.REQUEST_WEAPON_POWER;

            // this action can skip moving, as it is just a request to another ship bot
            nextState = BotStates.ACTING;
        }

        // regardless of previous actions, if any modules are broken and we are not firing,
        // or all modules are broken, switch to repair
        if ( (numBrokenModules >= myModules.Count) || (!isFiring && brokenModule) )
        {
            moduleToActOn = moduleNeedingRepairs;
            moduleToMoveTo = moduleToRepairId;
            terminalToMoveTo = 0;
            actionToTake = SecurityActions.REPAIR;
            nextState = BotStates.MOVE;
        }
        else if (actionToTake == SecurityActions.WAIT)
        {
            moduleToActOn = null;
        }

        // if there is a module we are acting on, we need to set module and terminal as occupied and release the one we are on
        if (moduleToActOn != null)
        {
            // these values are updated in the FireWeapon method as it could be either a missile or cannon
            if (!isFiring)
            {
                myModules[currentModule].SetTerminalOccupied(currentTerminal, false);
                currentModule = moduleToMoveTo;
                currentTerminal = terminalToMoveTo;
            }

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
            case SecurityActions.FIRE_CANNON:
                // you then roll two die 6 and if the roll is >= to difficulty it is a success 
                // nothing happens on a failure (just a miss)
                if (PerformActionCheck(actionDifficulty))
                {
                    // apply damage and reduce shield level of target by one for the hit
                    int damage = GetCannonDamage();

                    myShip.shipManagerScript.UpdateHullDamage(myShip.currentTarget.shipID, damage);
                    myShip.shipManagerScript.UpdateEnergy(myShip.currentTarget.shipID, (int)GeneratedShip.ShipPowerAreas.SHIELDS, 1);

                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Cannon fired with success and did " + damage + " damage!");
                    //Debug.Log("Cannon fired with success and did " + damage + " damage!");
                }

                // used marker is added on success or failure
                moduleToActOn.AddUsedMarkers(1);
                myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)GeneratedShip.ShipPowerAreas.WEAPONS, -1);
                break;

            case SecurityActions.FIRE_MISSILE:
                // damage dealt - usually it would be based on number of modules hit and would be handled by missile objects
                // doing this here as we are using standard warhead and just doing hull damage for now
                int hullDamage = (RollDie() + RollDie());

                // if the action succeeds, fire the missile (assuming all are standard 2-die damage for now)
                // TODO: create missile object that is at speed 6 and moves towards target (for now just do Hull damage)
                if (PerformActionCheck(actionDifficulty))
                {
                    myShip.shipManagerScript.UpdateHullDamage(myShip.currentTarget.shipID, hullDamage);
                    myShip.shipManagerScript.UpdateEnergy(myShip.currentTarget.shipID, (int)GeneratedShip.ShipPowerAreas.SHIELDS, 1);
                    
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "Missile fired with success and did " + hullDamage + " damage!");
                    //Debug.Log("Missile fired with success and did " + hullDamage + " damage!");
                }
                // if a missile firing fails, then it does damage to our ship!
                else
                {
                    //myShip.hullDamage += hullDamage;
                    myShip.shipManagerScript.UpdateHullDamage(myShip.shipID, hullDamage);
                    myShip.shipManagerScript.UpdateBotStatusText(myShip.shipID, "!!!Missile failed to fire, it exploded and did " + hullDamage + " damage to your ship!!!");
                    //Debug.Log("!!!Missile faild to fire, it exploded and did " + hullDamage + " damage to your ship!!!");
                }

                // used marker is added on success or failure
                moduleToActOn.AddUsedMarkers(1);
                myShip.shipManagerScript.UpdateEnergy(myShip.shipID, (int)GeneratedShip.ShipPowerAreas.WEAPONS, -1);
                break;

            case SecurityActions.REQUEST_WEAPON_POWER:
                myShip.energyUpdateQueue.Enqueue(GeneratedShip.ShipPowerAreas.WEAPONS);
                break;

            case SecurityActions.REPAIR:
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

    /// <summary>
    /// Goes through and checks to see if we should fire a cannon or missile based on the heuristic of the preferred weapon
    /// The values for the heruistic are currenlty constants - TODO: Expose them to the user to play with?
    /// </summary>
    /// <param name="cannonToFire"></param>
    /// <param name="cannonModuleId"></param>
    /// <param name="cannonTerminalId"></param>
    /// <param name="missileToFire"></param>
    /// <param name="missileModuleId"></param>
    /// <param name="missileTerminalId"></param>
    /// <returns></returns>
    private bool FireWeapon(RoomInfo cannonToFire, int cannonModuleId, int cannonTerminalId,
                            RoomInfo missileToFire, int missileModuleId, int missileTerminalId)
    {
        bool isFiring = false;

        // first check to see if either weapon system is available
        if ((cannonToFire != null) || (missileToFire != null))
        {
            // This chooses the cannon if it was available (otherwise it must be the missile)
            bool fireCannon = (cannonToFire != null);

            // if both modules are available, then we must decide based on preference (overrides previous)
            if ((cannonToFire != null) && (missileToFire != null)) 
            {
                float weaponSelectionChance = Random.Range(0, 1f);

                fireCannon = (weaponSelectionChance > MISSILE_PREFERENCE);
            }

            if (fireCannon) 
            {
                // is the cannon facing the correct direction? (180 arc) compare our cannon facing vs the target location
                int cannonFacing = (myShip.currentDirection + ((int)cannonToFire.roomFacing * SINGLE_DIRECTION_CHANGE));

                // if the turn would put us over a full cirle, just subtract 360 to get the new facing
                if (cannonFacing >= 360)
                {
                    cannonFacing -= 360;
                }

                // for now we are using the targetbot, but will need to figure out target ship
                Vector2Int targetPos = myShip.currentTarget.mapLocation;
                bool canTarget = true;

                // since a cannon can shoot in 180 degree arc, we only need to look at one component per facing
                if (((cannonFacing == 0) && (myShip.mapLocation.x <= targetPos.x)) ||
                     ((cannonFacing == 90) && (myShip.mapLocation.y <= targetPos.y)) ||
                     ((cannonFacing == 180) && (myShip.mapLocation.x >= targetPos.x)) ||
                     ((cannonFacing == 270) && (myShip.mapLocation.y >= targetPos.y)))
                {
                    canTarget = false;
                }

                if (!canTarget)
                {
                    // determine facing change
                    myShip.requestedFacing = SINGLE_DIRECTION_CHANGE;
                    return isFiring;
                }

                // difficulty of cannon to succeed is (distance from ship + 2 x otherShipSpeed + 3 x num used markers on cannon - combat skill)
                actionDifficulty = GetDistanceToTarget();
                actionDifficulty += myShip.currentTarget.currentSpeed * SPEED_MULTIPLIER;
                actionDifficulty += cannonToFire.GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;
            }
            else
            {
                actionDifficulty = MISSILE_BASE_DIFFICULTY;
                actionDifficulty += missileToFire.GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;
            }

            // adjust for combat skill
            actionDifficulty -= combat;

            // only fire if there is a chance to succeed within our tolerance
            if (actionDifficulty < MAX_DIFFICULTY_TO_TEST)
            {
                int moduleToMoveTo;
                int terminalToMoveTo;

                if (fireCannon)
                {
                    moduleToActOn = cannonToFire;
                    moduleToMoveTo = cannonModuleId;
                    terminalToMoveTo = cannonTerminalId;
                    actionToTake = SecurityActions.FIRE_CANNON;                   
                }
                else
                {
                    moduleToActOn = missileToFire;
                    moduleToMoveTo = missileModuleId;
                    terminalToMoveTo = missileTerminalId;
                    actionToTake = SecurityActions.FIRE_MISSILE;
                }

                // no need to move if we are in the current module, so stay put for actions
                if (moduleToMoveTo == currentModule)
                {
                    terminalToMoveTo = currentTerminal;
                }
                
                // these need to be done here as we can only pass one variable back (such a small case to build a struct for)
                myModules[currentModule].SetTerminalOccupied(currentTerminal, false);
                currentModule = moduleToMoveTo;
                currentTerminal = terminalToMoveTo;

                isFiring = true;
            }
        }
        
        return isFiring;

    } // end FireWeapon

    /// <summary>
    /// Calculates the cannon damage
    /// </summary>
    /// <returns>the hull damage this cannon would cause</returns>
    private int GetCannonDamage()
    {
        int hullDamage = 0;

        // TODO: Add hit location to this so we can add damage markers to ships and damage bots
        // (may not do damage to bots due to time constraints)

        // get the target shield level
        int targetShieldLevels = myShip.currentTarget.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.SHIELDS];

        // cannon hullDamage is based on a number of dice rolled equal to the ships weapon level + the targets shield level
        int numDiceToRoll = (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS] + targetShieldLevels);

        List<int> damageRolls = new List<int>();

        // roll all the dice and store the results in a list
        for (int i = 0; i < numDiceToRoll; i++)
        {
            damageRolls.Add(RollDie());
        }

        // sort the list in descending order
        damageRolls.Sort();
        damageRolls.Reverse();

        // remove the highest numbers for the shield level
        for (int i = 0; i < targetShieldLevels; i++)
        {
            damageRolls.RemoveAt(0);
        }

        // For now we are just calculating all left over die as hull damage - keep it simple
        // TODO: apply damage to each module hit (hull damage == die value, values of 4,5,6 break the module)
        //  - if any die are left that did not pass through a module, then it is 1 hull damage per die
        foreach (int damage in damageRolls)
        {
            hullDamage += damage;
        }

        return hullDamage;
    }
}
