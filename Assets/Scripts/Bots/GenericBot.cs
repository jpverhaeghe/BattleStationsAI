using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomInfo;

public class GenericBot : MonoBehaviour
{
    public enum BotType
    {
        COMMAND,
        ENGINEERING,
        SCIENCE,
        SECURITY,
        OPERATIONS
    }

    public enum BotStates
    {
        IDLE,
        MOVE,
        MOVING,
        ACTING,
    }

    // constant variables for this bot
    public static RoomData.ModuleType[][] botModuleTypes =
    {
        new RoomData.ModuleType[] { RoomData.ModuleType.Helm },                                         // Command
        new RoomData.ModuleType[] { RoomData.ModuleType.Engine },                                       // Engineering
        new RoomData.ModuleType[] { RoomData.ModuleType.Science, RoomData.ModuleType.Hyperdrive },      // Science
        new RoomData.ModuleType[] { RoomData.ModuleType.Cannon, RoomData.ModuleType.MissileBay },       // Security
        new RoomData.ModuleType[] { RoomData.ModuleType.CargoBay, RoomData.ModuleType.LifeSupport },    // Operations
    };

    // constant values used by other classes (specifically sub-classes)
    public static int PROFESSION_SKILL_VALUE = 3;                   // starting value for the profession the bot is trained in
    public static int NON_PROFESSION_SKILL_VALUE = 1;               // starting value for all other professions (untrained)
    public static int ADD_OR_USED_MULTIPLIER = 3;                   // the difficulty multiplier for each used marker on a module
    public static int MAX_DIFFICULTY_TO_TEST = 9;                   // the largest difficulty a bot will test against
    public static int MIN_VALUE_TO_ADD_EXTRA = 6;                   // the minimum level to try for moving/pumping more than 1 level
    public static int MAX_SPEED = 5;                                // the maximum safe speed for the ship
    public static int SINGLE_DIRECTION_CHANGE = 90;//60;            // each change in direction adds difficulty, one change is 90 degrees (hex would be 60)
    public static int REPAIR_DEFAULT_DIFFICULTY = 11;
    public const int NUM_DIE_SIDES = 6;

    // public variables to be accessed by outside scripts
    public List<RoomInfo> myModules;                                // The modules on the ship that this bot is responsible for

    // protected values used by this class and its sub-classes 
    // TODO: if we need these outward facing, then make public
    protected GeneratedShip myShip;                                 // a link back to the ship we are tied to
    protected BotType myType;

    // game statistics
    protected int athletics;                                        // used for movement
    protected int combat;                                           // used for combat - weapons officer profession
    protected int engineering;                                      // used for combat - engineering officer profession
    protected int piloting;                                         // used for combat - command officer profession
    protected int science;                                          // used for combat - science officer profession

    // AI systems
    protected List<Vector2Int> currentPath;                         // stores the path to move to
    protected BotStates currentState = BotStates.IDLE;              // sets up the default state for this bot
    protected bool runningState = false;                            // a boolean used to make sure we don't start multiple co-routines
    protected int moveSpeed = 4;                                    // the amount of squares a bot can move - same for all bots
    protected int currentModule = 0;                                // allows bots to move to differnt modules if they have more
    protected int currentTerminal = 0;                              // the terminal we are currently working at (may not need later)

    // action choices
    protected RoomInfo moduleToActOn;
    protected int actionDifficulty;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    protected void Start()
    {
        // generic bots profession will be athletics, they can move around faster but are not good at any stations
        athletics = PROFESSION_SKILL_VALUE;
        combat = NON_PROFESSION_SKILL_VALUE;
        engineering = NON_PROFESSION_SKILL_VALUE;
        piloting = NON_PROFESSION_SKILL_VALUE;
        science = NON_PROFESSION_SKILL_VALUE;
        myType = BotType.OPERATIONS;

    } // end Start

    /// <summary>
    /// Update is called once per frame - used for doing generic bot actions based on a basic state machine
    /// Will use heuristics in other bots to determine what states to switch too, using Utility Theory
    /// </summary>
    protected void Update()
    {
        // for now, bots idle -> move -> moving until at location -> perform action -> repeat
        switch (currentState)
        {
            case BotStates.IDLE:
                if (!runningState)
                {
                    StartCoroutine("PerformIdleState");
                }
                break;

            case BotStates.MOVE:
                // for now just get the next terminal location for the first module
                FindNextMoveLocation();
                break;

            case BotStates.MOVING:
                if (!runningState)
                {
                    StartCoroutine("MoveToLocation");
                }
                break;

            case BotStates.ACTING:
                if (!runningState)
                {
                    StartCoroutine("DoAction");
                }
                break;

            default:
                break;
        }
        
    }

    /// <summary>
    /// Sets the ship script that this bot is associated with
    /// </summary>
    /// <param name="parentShip">a link to the GeneratedShip script for this bot</param>
    public void SetShip(GeneratedShip parentShip)
    {
        myShip = parentShip;

    } // end SetShip

    /// <summary>
    /// Adds the given module to the module list, creating it if it wasn't already set up
    /// </summary>
    /// <param name="newModule">The ship module to add</param>
    public void AddModule(RoomInfo newModule)
    {
        // set up the myModules list if it has not been created
        if (myModules == null)
        {
            myModules = new List<RoomInfo>();
        }

        // add the module for this bot to keep trakc of
        myModules.Add(newModule);

    } // AddModule

    /// <summary>
    /// Returns the grid position for this bot, so it can use path finding to get to its next location
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetGridPos()
    {
        Vector2Int gridPos = new Vector2Int();

        // calculate the grid position based off the world position and ship position
        // Z is the row and is negative, X is the column and is positive
        gridPos.x = (int)(myShip.shipWorldOrigin.z - gameObject.transform.position.z);
        gridPos.y = (int)(gameObject.transform.position.x - myShip.shipWorldOrigin.x);

        return gridPos;

    } // end GetGridPos

    /// <summary>
    /// Performs an action check for the bot given the difficulty of the task
    /// TODO: May need to make this a generic function for all characters - so may move to GameManager
    /// </summary>
    /// <param name="difficulty">the difficulty of the task</param>
    /// <returns>true if it was a success, false otherwise</returns>
    public bool PerformActionCheck(int difficulty)
    {
        // if the difficulty is above 2 x NUM_DIE_SIDES it fails automatically
        if (difficulty > (2 * NUM_DIE_SIDES))
        {
            myShip.shipManagerScript.UpdateBotRollText(myShip.shipID, this.myType + " had a automatic failure");
            return false;
        }
        else if (difficulty < 2)
        {
            myShip.shipManagerScript.UpdateBotRollText(myShip.shipID, this.myType + " had a automatic success");
            return true;
        }

        // roll the die here using random (will eventually pause game and roll die in game)
        // TODO: Display result and difficulty to screen
        int firstRoll = RollDie();
        int secondRoll = RollDie();
        int checkResult = firstRoll + secondRoll;

        // if we have scans for re-rolls and the roll would fail, re-roll the lowest die
        if ( (checkResult < difficulty) && (myShip.numStoredScans > 0))
        {
            if (firstRoll < secondRoll)
            {
                firstRoll = RollDie();
            }
            else
            {
                secondRoll = RollDie();
            }

            // remove the scan
            myShip.shipManagerScript.UpdateNumScans(myShip.shipID, -1);

            // adjust the roll (even if it is worse)
            checkResult = firstRoll + secondRoll;
        }

        myShip.shipManagerScript.UpdateBotRollText(myShip.shipID, "Bot performed a check: " + checkResult + " vs. difficulty of " + difficulty);
        //Debug.Log("Bot performed a check: " +  checkResult + " vs. difficulty of " + difficulty);
        return (checkResult >= difficulty);

    } // PerformActionCheck

    /// <summary>
    /// Rolls the die so it ends up 1 through num sides
    /// </summary>
    /// <returns>the randomized integer value</returns>
    public int RollDie()
    {
        return (Random.Range(0, NUM_DIE_SIDES) + 1);

    } // end RollDie

    /// <summary>
    /// Gets the distance to the current target
    /// </summary>
    /// <returns>The distance to the target or -1 if the target was invalid</returns>
    public int GetDistanceToTarget()
    {
        int distanceToTarget = -1;

        if (myShip.currentTarget != null)
        { 
            Vector2Int targetPos = myShip.currentTarget.mapLocation;
            distanceToTarget = myShip.shipManagerScript.GetDistanceBetweenShips(myShip.mapLocation, targetPos);
        }

        return distanceToTarget;

    } // end GetDistanceToTarget

    /// <summary>
    /// Attempts to increase the difficulty of changing engine energy levls if it is feasible
    /// </summary>
    /// <param name="currentDifficultyLevel">The current difficulty attempting</param>
    /// <returns>the updated difficulty</returns>
    protected int AttemptHigherDifficulty(int currentDifficultyLevel)
    {
        if (currentDifficultyLevel <= MIN_VALUE_TO_ADD_EXTRA)
        {
            currentDifficultyLevel += ADD_OR_USED_MULTIPLIER;
        }

        return currentDifficultyLevel;

    } // end AttemptHigherDifficulty

    /// <summary>
    /// Attempts to repair the given module
    /// </summary>
    /// <param name="moduleToActOn">The module to repair</param>
    protected void AttemptRepair(RoomInfo moduleToActOn)
    {
        // attempt a repair
        if (PerformActionCheck(REPAIR_DEFAULT_DIFFICULTY - engineering))
        {
            // repair succeeded so remove broken from the module
            moduleToActOn.RepairModule();
        }

    } //  AttemptRepair

    /// <summary>
    /// Runs a generic idle state where it waits for a second before seting up a move state
    /// </summary>
    /// <returns>yields the system until done with the wait, then finishes the state</returns>
    protected virtual IEnumerator PerformIdleState()
    {
        runningState = true;
        yield return new WaitForSeconds(1);
        currentState = BotStates.MOVE;
        runningState = false;

    } // end PerformIdleState

    /// <summary>
    /// Finds a path to the next module terminal location for the given module
    /// </summary>
    protected void FindNextMoveLocation()
    {
        // if we have an module to move to based on a selected action, move there
        if (moduleToActOn != null)
        {
            // get the next terminal location for the given module
            TerminalInfo terminalInfo = moduleToActOn.GetTerminalLoacation(currentTerminal);
            Vector2Int moveLocation = terminalInfo.mapLocation;

            // find the path to the given destination
            currentPath = myShip.shipPathingSystem.BreadthFirstSearch(GetGridPos(), moveLocation);
            
            // Make the star tile pulse while the bot is moving there
            terminalInfo.starTileRenderer.material.SetFloat("_pulse", 1.0f);
            gameObject.GetComponent<Renderer>().material.SetFloat("_pulse", 1.0f);

            // set it to move to the new location
            currentState = BotStates.MOVING;
        }
        // otherwise just wander from termninal to terminal (it is fun to watch them move around)
        else
        {
            bool newTerminalFound = FindTerminalToWanderTo();

            if (newTerminalFound)
            {
                TerminalInfo terminalInfo = myModules[currentModule].GetTerminalLoacation(currentTerminal);
                Vector2Int moveLocation = terminalInfo.mapLocation;

                // find the path to the given destination
                currentPath = myShip.shipPathingSystem.BreadthFirstSearch(GetGridPos(), moveLocation);

                // Make the star tile pulse while the bot is moving there
                terminalInfo.starTileRenderer.material.SetFloat("_pulse", 1.0f);
                gameObject.GetComponent<Renderer>().material.SetFloat("_pulse", 1.0f);

                // set it to move to the new location
                currentState = BotStates.MOVING;
            }
            else
            {
                currentState = BotStates.IDLE;
            }
        }

    } // end FindNextMoveLocation

    /// <summary>
    /// Moves to the next location one square at a time
    /// </summary>
    protected IEnumerator MoveToLocation()
    {
        // if there is a path, follow it
        if (currentPath != null)
        {
            // if there are still places to go
            if (currentPath.Count > 0)
            {
                // get the next location (always at index 0 as we remove them as we go)
                Vector2Int nextGridPosition = currentPath[0];
                Vector2Int currentGridPosition = GetGridPos();
                Vector2Int updatedGridPosition = currentGridPosition - nextGridPosition;
                Vector3 currentWorldPosition = gameObject.transform.position;
                Vector3 newPosition = new Vector3(currentWorldPosition.x - updatedGridPosition.y, 0, currentWorldPosition.z + updatedGridPosition.x);

                // TODO: Need to add wait if the area is blocked by another bot....what bots have priority?
                // may just want to set the position
                gameObject.transform.position = newPosition;

                // remove this part of the path as we have moved
                currentPath.RemoveAt(0);

                runningState = true;
                yield return new WaitForSeconds(0.25f);
                runningState = false;
            }
            // if not, then we must be there, time to move on
            else
            {
                // get the next terminal location for the given module
                RoomInfo moduleToMoveTo = myModules[currentModule];
                TerminalInfo terminalInfo = moduleToMoveTo.GetTerminalLoacation(currentTerminal);

                // if the bot was acting on a specific terminal, then use that one instead (not wandering)
                if (moduleToActOn != null)
                {
                    terminalInfo = moduleToActOn.GetTerminalLoacation(currentTerminal);
                }
                
                // stop the star tile from pulsing
                terminalInfo.starTileRenderer.material.SetFloat("_pulse", 0f);
                gameObject.GetComponent<Renderer>().material.SetFloat("_pulse", 0f);
                currentState = BotStates.ACTING;
            }
        }
        else
        {
            currentState = BotStates.ACTING;
        }
    
    } // end MoveToLocation

    /// <summary>
    /// Will perform a given action at the location
    /// - will need to set this up in idle perhaps - using heuristics to determine the best option for the bot at that time
    /// </summary>
    /// <returns>yields the system until done with the wait, then finishes the state<</returns>
    protected virtual IEnumerator DoAction()
    {
        // choose the action to perform (or not perform)
        runningState = true;
        yield return new WaitForSeconds(0.5f);
        currentState = BotStates.IDLE;
        runningState = false;

    } // end DoAction

    private bool FindTerminalToWanderTo()
    {
        bool newTerminalFound = false;

        // default to the current module and the next terminal in that module
        int newModule = currentModule;
        int newTerminal = (currentTerminal + 1);

        // go through the all the modules I have access to and see if one is available for me to wander to
        // if none are available (we have wrapped around), just leave me where I am and don't move
        while (!newTerminalFound && ((newModule != currentModule) || (newTerminal != currentTerminal)))
        {
            // reset back to zero if the index is larger than the number of terminal locations
            if (newTerminal >= myModules[newModule].GetTerminalLoacations().Count)
            {
                newTerminal = 0;

                // some bots have more than one module they look after, have them roam to the next one
                newModule++;

                // capping the module so we don't go out of bounds
                if (newModule >= myModules.Count)
                {
                    newModule = 0;
                }
            }

            // check to see if the terminal is occupied
            if (!myModules[newModule].IsTerminalOccupied(newTerminal) )
            {
                newTerminalFound = true;
            }
            else
            {
                // increase the terminal location index 
                newTerminal++; 
            }            
        }

        if (newTerminalFound) 
        { 
            currentModule = newModule;
            currentTerminal = newTerminal;
        }

        return newTerminalFound;

    } // end FindTerminalToWanderTo

}
