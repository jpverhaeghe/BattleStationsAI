using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.CargoBay, RoomData.ModuleType.LifeSupport };

    // constant values used by other classes (specifically sub-classes)
    public static int PROFESSION_SKILL_VALUE = 3;
    public static int NON_PROFESSION_SKILL_VALUE = 1;

    // public variables to be accessed by outside scripts

    // protected values used by this class and its sub-classes 
    // TODO: if we need these outward facing, then make public
    protected GeneratedShip myShip;                                 // a link back to the ship we are tied to
    protected List<RoomInfo> myModules;                             // The modules on the ship that this bot is responsible for
    protected int athletics;                                        // used for movement
    protected int combat;                                           // used for combat - weapons officer profession
    protected int engineering;                                      // used for combat - engineering officer profession
    protected int piloting;                                         // used for combat - command officer profession
    protected int science;                                          // used for combat - science officer profession

    protected List<Vector2Int> currentPath;                         // stores the path to move to
    protected BotStates currentState = BotStates.IDLE;              // sets up the default state for this bot
    protected bool runningState = false;                            // a boolean used to make sure we don't start multiple co-routines
    protected int moveSpeed = 4;                                    // the amount of squares a bot can move - same for all bots
    protected int currentModule = 0;                                // allows bots to move to differnt modules if they have more
    protected int currentTerminal = 0;                              // the terminal we are currently working at (may not need later)


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
                FindNextMoveLocation(myModules[currentModule]);
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
        gridPos.y = (int)(myShip.shipWorldOrigin.x + gameObject.transform.position.x);

        return gridPos;

    } // end GetGridPos

    /// <summary>
    /// Runs a generic idle state where it waits for a second before seting up a move state
    /// </summary>
    /// <returns>yields the system until done with the wait, then finishes the state</returns>
    private IEnumerator PerformIdleState()
    {
        runningState = true;
        yield return new WaitForSeconds(1);
        currentState = BotStates.MOVE;
        runningState = false;

    } // end PerformIdleState

    /// <summary>
    /// Finds a path to the next module terminal location for the given module
    /// </summary>
    /// <param name="moduleToMoveTo">The module to search for a terminal for pathing</param>
    private void FindNextMoveLocation(RoomInfo moduleToMoveTo)
    {
        // get the next terminal location for the given module
        Vector2Int moveLocation  = moduleToMoveTo.GetTerminalLoacation(currentTerminal);

        // increase the terminal location index and wrap around if it is larger than the number in the module
        currentTerminal++;

        // reset back to zero if the index is larger than the number of terminal locations
        if (currentTerminal >= moduleToMoveTo.GetTerminalLoacations().Count)
        {
            currentTerminal = 0;

            // some bots have more than one module they look after, have them roam to the next one
            currentModule++;

            // capping the module so we don't go out of bounds
            if (currentModule >= myModules.Count)
            {
                currentModule = 0;
            }
        }

        // find the path to the given destination
        currentPath = myShip.shipPathingSystem.BreadthFirstSearch(GetGridPos(), moveLocation);

        // set it to move to the new location
        currentState = BotStates.MOVING;

    } // end FindNextMoveLocation

    /// <summary>
    /// Moves to the next location one square at a time
    /// </summary>
    private IEnumerator MoveToLocation()
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
                //gameObject.transform.Translate(newPosition);

                // remove this part of the path as we have moved
                currentPath.RemoveAt(0);

                runningState = true;
                yield return new WaitForSeconds(0.25f);
                runningState = false;
            }
            // if not, then we must be there, time to move on
            else
            {
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
    private IEnumerator DoAction()
    {
        runningState = true;
        yield return new WaitForSeconds(0.5f);
        currentState = BotStates.IDLE;
        runningState = false;

    } // end DoAction

}
