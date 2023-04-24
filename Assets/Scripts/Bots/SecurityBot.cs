using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Cannon, RoomData.ModuleType.MissileBay };

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
        // - Is there enough power to fire - we want a minimum value for to fire a cannon which is immediate hullDamage
        // - Is there clear line of sight (for cannons - if a cannon is the only thing available)
        // - Is there a chance of success

        // default action will be to wait
        actionToTake = SecurityActions.WAIT;
        BotStates nextState = BotStates.MOVE;
        int currentShipWeaponsLevel = myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS];

        // booleans for decision making
        bool brokenModule = false;
        bool isFiring = false;

        // go through the modules to see if one is working and set the available module types
        RoomInfo moduleNeedingRepairs = null;
        int numBrokenModules = 0;

        // track modules for firing
        RoomInfo cannonToFire = null;
        RoomInfo missileToFire = null;

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
                if ( (cannonToFire == null) && (module.moduleType == RoomData.ModuleType.Cannon))
                {
                    cannonToFire = module;
                }

                if ((missileToFire == null) && (module.moduleType == RoomData.ModuleType.MissileBay))
                {
                    missileToFire = module;
                }
            }
        }

        // only go to fire a weapon if there is energy
        if (currentShipWeaponsLevel > MIN_POWER_LEVEL_TO_FIRE)
        {
            isFiring = FireWeapon(cannonToFire, missileToFire);
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
            actionToTake = SecurityActions.REPAIR;
            nextState = BotStates.MOVE;
        }
        else if (actionToTake == SecurityActions.WAIT)
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
            case SecurityActions.FIRE_CANNON:
                // you then roll two die 6 and if the roll is >= to difficulty it is a success 
                // nothing happens on a failure (just a miss)
                if (PerformActionCheck(actionDifficulty))
                {
                    // apply damage and reduce shield level of target by one for the hit
                    int damage = GetCannonDamage();
                    myShip.shipManagerScript.botTargetPractice.hullDamage += damage;
                    myShip.shipManagerScript.botTargetPractice.shieldLevel -= 1;
                    if (myShip.shipManagerScript.botTargetPractice.shieldLevel < 0)
                        myShip.shipManagerScript.botTargetPractice.shieldLevel = 0;
                    myShip.shipManagerScript.UpdateBotStatusText("Cannon fired with success and did " + damage + " damage!");
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
                    myShip.shipManagerScript.botTargetPractice.hullDamage += hullDamage;
                    myShip.shipManagerScript.botTargetPractice.shieldLevel -= 1;
                    if (myShip.shipManagerScript.botTargetPractice.shieldLevel < 0)
                        myShip.shipManagerScript.botTargetPractice.shieldLevel = 0;
                    myShip.shipManagerScript.UpdateBotStatusText("Missile fired with success and did " + hullDamage + " damage!");
                    //Debug.Log("Missile fired with success and did " + hullDamage + " damage!");
                }
                // if a missile firing fails, then it does damage to our ship!
                else
                {
                    //myShip.hullDamage += hullDamage;
                    myShip.shipManagerScript.UpdateHullDamage(myShip.shipID, hullDamage);
                    myShip.shipManagerScript.UpdateBotStatusText("!!!Missile failed to fire, it exploded and did " + hullDamage + " damage to your ship!!!");
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

    private bool FireWeapon(RoomInfo cannonToFire, RoomInfo missileToFire)
    {
        bool isFiring = false;

        // first check to see if either weapon system is available
        if ((cannonToFire != null) || (missileToFire != null))
        {
            // This chooses the cannon if it was available (otherwise it must be the missile
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
                Vector2Int targetPos = myShip.shipManagerScript.botTargetPractice.mapLocation;
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
                actionDifficulty = myShip.shipManagerScript.botTargetPractice.distance;
                actionDifficulty += myShip.shipManagerScript.botTargetPractice.speed * SPEED_MULTIPLIER;
            }
            else
            {
                actionDifficulty = MISSILE_BASE_DIFFICULTY;
            }

            actionDifficulty += myModules[currentModule].GetNumUsedMarkers() * ADD_OR_USED_MULTIPLIER;
            actionDifficulty -= combat;

            // only fire if there is a chance to succeed within our tolerance
            if (actionDifficulty < MAX_DIFFICULTY_TO_TEST)
            {
                if (fireCannon)
                {
                    moduleToActOn = cannonToFire;
                    actionToTake = SecurityActions.FIRE_CANNON;
                }
                else
                {
                    moduleToActOn = missileToFire;
                    actionToTake = SecurityActions.FIRE_MISSILE;
                }

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

        // cannon hullDamage is based on a number of dice rolled equal to the ships weapon level + the targets shield level
        int numDiceToRoll = (myShip.energySystemLevels[(int)GeneratedShip.ShipPowerAreas.WEAPONS] + 
                             myShip.shipManagerScript.botTargetPractice.shieldLevel);

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
        for (int i = 0; i < myShip.shipManagerScript.botTargetPractice.shieldLevel; i++)
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
