//using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomData;

public class ShipLayoutGenerator : MonoBehaviour
{
    // list of ship sizes available
    // NOTE: Size of ships is num rooms divided by 3 - maximum num rooms for a size category is: (size + 1) * 3 - 1
    // Ships are no smaller than 2 in size (minimum of 6 rooms), so have an offset variable
    public enum ShipSize
    {
        Skiff,          // Size should be 2 - 6 to 8 rooms
        Scout,          // Size should be 3 - 9 to 11 rooms
        Frigate,        // Size should be 4 - 12 to 14 rooms
        Destroyer,      // Size should be 5 - 15 to 17 rooms
        Cruiser,        // Size should be 6 - 18 to 20 rooms
        Dreadnought,    // Size should be 7 - 21 to 23 rooms
    }

    [System.Flags]
    public enum ExitDirection
    {
        None = 0,
        Up = 1 << None,
        Right = 1 << Up,
        Down = 1 << Right,
        Left = 1 << Down,
        All = Up | Right | Down | Left,
    }

    public struct PotentialRoomPos
    {
        public int rowPos;
        public int colPos;
    }

    // constant variables for this script
    public static int MIN_SHIP_SIZE = 2;                    // ship offset for the enum
    public static int NUM_ROOMS_PER_SIZE = 3;               // the number of rooms per ship size (minimum)
    private static int ENGINE_SHIP_DIVISOR = 2;             // the number we divide the ship size by to determine the minimum number of engines
    private static int LIFE_SUPPORT_SHIP_DIVISOR = 2;       // the number we divide the ship size by to determine the minimum number of life support rooms

    // Serialized fields used by this script
    //[SerializeField] ShipManager shipManagerScript;
    [SerializeField] TMP_Dropdown shipSizes;

    // private variables used by this script
    private List<ModuleType> requiredShipModules;           // a list of the required ship modules - does this need to be local?
    private List<ModuleType> placedShipModules;             // a list of the placed ship modules - does this need to be local?
    private Queue<PotentialRoomPos> roomsToFill;            // a list of rooms to fill, first in first out
    private ShipSize shipSizeEnum;                          // a variable to hold the enumeration in for populating the dropdown list
    RoomInfo[,] shipLayout;                                 // the array that will store the ship layout
    private int shipHeight;                                 // the height of the ship array
    private int shipWidth;                                  // the width of the ship array
    private int shipSize;                                   // used to store the current generated ship size
    private int numEnginesNeeded;                           // keeps track of the number of engines needed by this craft as engines are placed after all other modules

    // module specific variables
    private int numModulesToPlace;                          // keeps track of the number of modules placed
    private int uniqueModulesPlaced;                        // to keep track of the modules placed so we can make sure at least one of each type that is not required
    private int helmRowPos;                                 // the position of the helm so we can make sure not to put modules in front of it
    private int helmColPos;                                 // the position of the helm so we can make sure not to put modules in front of it

    // Debug variables to make it easier to debug this code visually
    //private RoomSpawner roomSpawner;                        // for testing purposes so we can see the ship being created room by room
    //private GameObject shipParentObject;                    // to attach the rooms to
    //public bool debugIsOn;

    /// <summary>
    /// Start is called before the first frame update to set up variables, etc. for this script
    /// </summary>
    void Start()
    {
        // set up the drop down list for the ship sizes used by this script
        PopulateDropDownWithEnum(shipSizes, shipSizeEnum);

        // set up the data structures for this script
        requiredShipModules = new List<ModuleType>();
        placedShipModules = new List<ModuleType>();
        roomsToFill = new Queue<PotentialRoomPos>();

        // debug code
        //roomSpawner = GameObject.Find("ShipSpawner").GetComponent<RoomSpawner>();

    } // end Start

    /// <summary>
    /// Turns the debug mode on
    /// </summary>
    /*public void ToggleDebug()
    {
        debugIsOn = !debugIsOn;
    }*/

    /// <summary>
    /// Generates a ship layout based on data from the UI - such as the size drop down
    /// </summary>
    /// <returns>RoomInfo[,] - the generated layout in a double array format of RoomInfo</returns>
    //public RoomInfo[,] GenerateShipLayout()
    public RoomInfo[,] GenerateShipLayout()
    {
        // debug code
        /*if (debugIsOn)
        {
            if (shipParentObject != null)
            {
                Destroy(shipParentObject);
            }

            // create a ship in the list to store the data in for later
            shipParentObject = new GameObject("Ship");
            shipParentObject.transform.position = new Vector3(0, 0, 0);
        }*/

        // TODO: Add race as an option so we can change building based on the rules of each race
        //       For example: Tentac - External modules ignore facing restrictions and have 360 degree firing arc.

        // clear up the list to keep track of modules placed and rooms to fill
        placedShipModules.Clear();
        uniqueModulesPlaced = 0;
        roomsToFill.Clear();

        // set up the ship dimensions
        SetShipDimensions();

        // create a list of the modules that must be in the ship - will remove from this list if a module of that type is placed
        SetRequiredRooms();

        // to start building the ship we will add a helm as there is only one, then build out modules, ending with engines
        PlaceHelmModule();

        // debug information
        Debug.Log("Ship Layout is " + shipHeight + " rows and " + shipWidth + " cols: Helm is at rowPos " + helmRowPos + " and colPos " + helmColPos);

        // go through the helm exits one by one and build out the next room, subtracting one from the num rooms for the ship
        PlaceModules();

        // now place engine modules
        PlaceEngineModules();

        // testing for ships that have less than the minimum number of rooms (just recall the Generate method)
        if (numModulesToPlace > 0)
        {
            LogRequiredModuleList();
            GenerateShipLayout();
            //GenerateShipLayout(debug);
        }

        // if debug mode is on, clean up as we leave
        /*if (debugIsOn)
        {
            if (shipParentObject != null)
            {
                Destroy(shipParentObject);
            }
        }*/

        // return the layout generated
        return shipLayout;
    }

    /// <summary>
    /// Places the room modules from the helm rooom based on its exits
    /// </summary>
    private void PlaceModules()
    {
        // go through the exits one by one until all but the engine modules are placed or there are no more exits
        while ((roomsToFill.Count > 0) && (numModulesToPlace > numEnginesNeeded))
        {
            PotentialRoomPos potentialRoomPos = roomsToFill.Dequeue();
            RoomInfo roomToPlace = PlaceModule(potentialRoomPos.rowPos, potentialRoomPos.colPos);

            if (roomToPlace != null)
            {
                // if the module type is not an engine or helm and not in the already placed list, then add one to the unique modules to place
                if ( (roomToPlace.moduleType < ModuleType.Engine) && !placedShipModules.Contains(roomToPlace.moduleType))
                {
                    uniqueModulesPlaced++;
                }

                // check to see if it is in the list of require modules and remove it if it is
                requiredShipModules.Remove(roomToPlace.moduleType);
                placedShipModules.Add(roomToPlace.moduleType);
                numModulesToPlace--;

                // add the exits to the queue if there are any
                // up
                if (roomToPlace.roomTiles[0, EXIT_POS] == RoomTiles.Empty)
                {
                    AddRoomToPlace((potentialRoomPos.rowPos - 1), potentialRoomPos.colPos);
                }

                // right
                if (roomToPlace.roomTiles[EXIT_POS, ROOM_TILES_LAST_INDEX] == RoomTiles.Empty)
                {
                    AddRoomToPlace(potentialRoomPos.rowPos, (potentialRoomPos.colPos + 1));
                }

                // down
                if (roomToPlace.roomTiles[ROOM_TILES_LAST_INDEX, EXIT_POS] == RoomTiles.Empty)
                {
                    AddRoomToPlace((potentialRoomPos.rowPos + 1), potentialRoomPos.colPos);
                }

                // left
                if (roomToPlace.roomTiles[EXIT_POS, 0] == RoomTiles.Empty)
                {
                    AddRoomToPlace(potentialRoomPos.rowPos, (potentialRoomPos.colPos - 1));
                }

                // add the module to the ship layout 
                shipLayout[potentialRoomPos.rowPos, potentialRoomPos.colPos] = roomToPlace;
                //DebugDrawModule(roomToPlace, potentialRoomPos.rowPos, potentialRoomPos.colPos);
            }
        }

    } // end PlaceModules

    /// <summary>
    /// Places a module based on where it cam
    /// </summary>
    /// <param name="currentRowPos"></param>
    /// <param name="currentColPos"></param>
    private RoomInfo PlaceModule(int currentRowPos, int currentColPos)
    {
        // set up the potential room data to be null by default
        RoomInfo roomToPlace = null;

        // don't place a module if it is obscuring the helm and check to see if the room has already been placed
        if (!CheckObscureHelm(currentRowPos, currentColPos) && (shipLayout[currentRowPos, currentColPos] == null))
        {
            // choose a random module - using the value of Engine as we don't randomize helm and engines are done after all other modules
            // Engine is the module type just at the end of the normal placeable modules so we can just use it's value
            ModuleType moduleType = (ModuleType)Random.Range(0, (int)ModuleType.Engine);

            // trying to keep modules from duplicating, so we have at least one of each if there is room
            if (uniqueModulesPlaced < (int)ModuleType.Engine) 
            {
                // Certain modules only need one like to preference things like cargo bays, weapons, etc.
                // we can preference certain modules by breaking this loop if they are randomly generated
                // TODO: May need to this after all unique ones are placed
                while ((moduleType != ModuleType.Cannon) && (moduleType != ModuleType.CargoBay) && (moduleType != ModuleType.MissileBay) && 
                        placedShipModules.Contains(moduleType))
                {
                    moduleType = (ModuleType)Random.Range(0, (int)ModuleType.Engine);
                }
            }

            // verification that the module must be chosen from the required list here before moving on
            // not sure if less than should be here, but just in case adding it so we don't go below
            if (numModulesToPlace <= requiredShipModules.Count)
            {
                // go until we get a type that is in the required list
                while (!requiredShipModules.Contains(moduleType))
                {
                    // choose another random module - subtracting two as we don't randomize helm and engines are done after all other modules
                    moduleType = (ModuleType)Random.Range(0, (int)ModuleType.Engine);
                }
            }

            roomToPlace = GetRoomWithFacing(moduleType, currentRowPos, currentColPos);
        }

        return roomToPlace;

    } // end PlaceModule

    /// <summary>
    /// Places the helm module into the ship layout at a random position always facing up as we can rotate the ship
    /// </summary>
    private void PlaceHelmModule()
    {
        // for now just choose a random position in the ship layout array
        // TODO: may need to change this to keep it out of the center of the array
        helmRowPos = Random.Range(0, shipHeight);
        helmColPos = Random.Range(0, shipWidth);

        // set the default helm facing to up
        RoomInfo helm = RoomData.HelmUpInfo;

        // place the helm
        shipLayout[helmRowPos, helmColPos] = helm;
        //DebugDrawModule(helm, helmRowPos, helmColPos);

        // remove the helm from the list of modules and log the helm as placed
        requiredShipModules.Remove(ModuleType.Helm);
        placedShipModules.Add(ModuleType.Helm);
        numModulesToPlace--;

        // add the exits to the room to place queue (this way we have a more natural placement of rooms)
        // as we are only placing upward facing helms, they have three exits (right, down and left)
        AddRoomToPlace(helmRowPos, helmColPos + 1);
        AddRoomToPlace(helmRowPos + 1, helmColPos);
        AddRoomToPlace(helmRowPos, helmColPos - 1);

    } // end PlaceHelmModule

    /// <summary>
    /// Places the remaing required engine modules at the ends of the ship (hopefully the bottom)
    /// </summary>
    private void PlaceEngineModules()
    {
        // all engines are placed facing the direction of the helm, so just get a pointer to the data that we can add for each one
        RoomInfo engineToPlace;

        // go through each engine and try to place them at the back of the ship, starting at the center and working out
        // based on where the helm is
        for (int engineNum = 0; engineNum < numEnginesNeeded; engineNum++)
        {
            // clear out engine data to place the next one
            engineToPlace = null;
            int[] enginePos = new int[2];

            // find a position as far away from the helm that is attached to the ship

            // we go from the bottom up here and find the first empty space we can add an engine too
            int row = shipHeight - 1;

            while ((engineToPlace == null) && (row >= 0))
            {
                engineToPlace = CheckEngineColPlacement(row, enginePos);
                row--;
            }

            // place the engine if space was found
            if (engineToPlace != null)
            {
                shipLayout[enginePos[0], enginePos[1]] = engineToPlace;
                //DebugDrawModule(engineToPlace, enginePos[0], enginePos[1]);

                // remove an engine from the list of modules and log it as placed
                requiredShipModules.Remove(ModuleType.Engine);
                placedShipModules.Add(ModuleType.Engine);
                numModulesToPlace--;
            }
        }

    } // end PlaceEngineModules

    /// <summary>
    /// Goes through a column in the ship array and checks to see if there is room to place an engine, starting at the center
    /// and moving out in both directgions
    /// </summary>
    /// <param name="rowPos">The rowPos of the ship layout to look at</param>
    /// <param name="enginePos">An array that will contain the engine positions if a placement was found</param>
    /// <returns></returns>
    private RoomInfo CheckEngineColPlacement(int rowPos, int[] enginePos)
    {
        RoomInfo engineToPlace = null;
        int colIncrementor = 1;
        int widthTest = shipWidth / 2;

        // colPos starting placement will depend on the width to get all since we start moving right,
        // so if the width of the array is even, we need to start at one less and test against that value
        if (shipWidth % 2 == 0)
        {
            widthTest--;
        }

        int colPos = widthTest;

        while ((engineToPlace == null) && ((colPos >= 0) && (colPos < shipWidth)))
        {
            // set up the test variables to make it easier to read
            int testRowPos = rowPos;
            int testColPos = colPos;

            // check to see if this space doesn't obscure the helm and is available 
            if (!CheckObscureHelm(rowPos, colPos) && (shipLayout[rowPos, colPos] == null))
            {
                // we can only place an engine if it is next to another module
                // we only need to look up, right and left as engine modules always get placed with down facing
                // up 
                testRowPos -= 1;
                if ( (testRowPos >= 0) && (shipLayout[testRowPos, testColPos] != null))
                {
                    engineToPlace = GetRoomWithFacing(ModuleType.Engine, rowPos, colPos);
                }

                // right - only if it didn't get placed above
                if (engineToPlace == null) 
                {
                    // must reset the position then go right one
                    testRowPos = rowPos;
                    testColPos++;

                    if ((testColPos < shipWidth) && (shipLayout[testRowPos, testColPos] != null))
                    {
                        engineToPlace = GetRoomWithFacing(ModuleType.Engine, rowPos, colPos);
                    }
                }

                // left 
                if (engineToPlace == null)
                {
                    // must reset the position then go left one
                    testRowPos = rowPos;
                    testColPos = colPos - 1;

                    if ((testColPos >= 0) && (shipLayout[testRowPos, testColPos] != null))
                    {
                        engineToPlace = GetRoomWithFacing(ModuleType.Engine, rowPos, colPos);
                    }
                }
            }

            // if there was a potential engine placement
            if (engineToPlace != null)
            {
                // if the facing is not down, we can't place the engine here so make it null
                if (engineToPlace.roomFacing != RoomFacing.Down)
                {
                    engineToPlace = null;
                }
                // set the engine position values to this spot
                else
                {
                    enginePos[0] = rowPos;
                    enginePos[1] = colPos;
                }
            }

            // update the column position alternating between left and right from center
            if (colPos <= widthTest)
            {
                colPos += colIncrementor;
            }
            else
            {
                colPos -= colIncrementor;
            }

            colIncrementor++;
        }

        return engineToPlace;

    } // end CheckEngineColPlacement


    /// <summary>
    /// Adds potential rooms to the queue of rooms to place if the room would be in bounds
    /// </summary>
    /// <param name="rowPos">the rowPos position of the room to place</param>
    /// <param name="colPos">the column position of the room to place</param>
    private void AddRoomToPlace(int rowPos, int colPos)
    {
        // check to see if the room is in bounds
        if ((rowPos >= 0) && (rowPos < shipHeight) && (colPos >= 0) && (colPos < shipWidth))
        {
            // only add the room location to the queue if something is not aleady there
            if (shipLayout[rowPos, colPos] == null)
            {
                // create the room
                PotentialRoomPos roomToAdd = new PotentialRoomPos();
                roomToAdd.rowPos = rowPos;
                roomToAdd.colPos = colPos;

                // add the room to the queue
                roomsToFill.Enqueue(roomToAdd);
            }
        }

    } // end AddRoomToPlace

    /// <summary>
    /// Gets the room to place if the room can be placed with appropriate facing
    /// </summary>
    /// <param name="moduleType">The the module type we are going to place</param>
    /// <param name="currentRowPos">The current rowPos position to place this module in the ship array</param>
    /// <param name="currentColPos">The current column position to place this module in the ship array</param>
    /// <returns></returns>
    private RoomInfo GetRoomWithFacing(ModuleType moduleType, int currentRowPos, int currentColPos)
    {
        // assume everything is facing the direction of the helm for now and grab the room we want to place
        RoomFacing currentFacing = RoomFacing.Up;

        // set default for engines to be down so they are facing the same way as the helm
        if (moduleType == ModuleType.Engine)
        {
            currentFacing = RoomFacing.Down;
        }

        // set up the room to place - gets set to null if it can't be placed
        RoomInfo roomToPlace = roomsByModules[(int)moduleType][(int)currentFacing];

        // some room modules must be external facing such as cannons, cloaking, engines, helm, missile bays, mine layer
        //  - Cargo bays do not require external facing, however some will need it to put certain modules into them
        //  - Helm was placed first and is not looked at here
        //  - Engines must face the way the helm is facing
        //  - All external facing modules but helm have walls on the exit that should face the outside
        if (roomToPlace.externalFacing)
        {
            ExitDirection exitsAvailable = ExitDirection.None;
            bool onEdge = false;

            // if there are rooms around, make sure we are not putting a wall to them
            // if we are in bounds, but not on the edge
            if ((currentRowPos - 1) >= 0)
            {
                // if there isn't a module there, we can face that way
                if (shipLayout[currentRowPos - 1, currentColPos] == null)
                {
                    exitsAvailable |= ExitDirection.Up;
                }
            }
            // if we are on the edge, then just face that direction
            else
            {
                // engines must take precidence when on the edge for facing down, so don't change them
                if (moduleType != ModuleType.Engine)
                {
                    currentFacing = RoomFacing.Up;
                    onEdge = true;
                }
            }

            // Down - check only if we didn't hit an edge or are an engine
            if (!onEdge)
            {
                // if we are in bounds, but not on the edge
                if ((currentRowPos + 1) < shipHeight)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos + 1, currentColPos] == null)
                    {
                        exitsAvailable |= ExitDirection.Down;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    currentFacing = RoomFacing.Down;
                    onEdge = true;
                }
            }

            // Right - check only if we didn't come from there
            if (!onEdge)
            {
                // if we are in bounds, but not on the edge
                if ((currentColPos + 1) < shipWidth)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos, currentColPos + 1] == null)
                    {
                        exitsAvailable |= ExitDirection.Right;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    // engines must take precidence when on the edge for facing down, so don't change them
                    if (moduleType != ModuleType.Engine)
                    {
                        currentFacing = RoomFacing.Right;
                        onEdge = true;
                    }
                }
            }

            // Left - check only if we didn't come from there
            if (!onEdge)
            {
                // if we are in bounds, but not on the edge
                if ((currentColPos - 1) >= 0)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos, currentColPos - 1] == null)
                    {
                        exitsAvailable |= ExitDirection.Left;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    // engines must take precidence when on the edge for facing down, so don't change them
                    if (moduleType != ModuleType.Engine)
                    {
                        currentFacing = RoomFacing.Left;
                        onEdge = true;
                    }
                }
            }

            // change the room direction if the room was on an edge 
            if (onEdge)
            {
                roomToPlace = roomsByModules[(int)moduleType][(int)currentFacing];
            }
            // if there are exits available then face one of them
            else if (exitsAvailable != ExitDirection.None)
            {
                // if there is an exit up, default to that
                if (exitsAvailable.HasFlag(ExitDirection.Up))
                {
                    roomToPlace = roomsByModules[(int)moduleType][(int)RoomFacing.Up];
                }
                // then down
                else if (exitsAvailable.HasFlag(ExitDirection.Down))
                {
                    roomToPlace = roomsByModules[(int)moduleType][(int)RoomFacing.Down];
                }
                // next is right
                else if (exitsAvailable.HasFlag(ExitDirection.Right))
                {
                    roomToPlace = roomsByModules[(int)moduleType][(int)RoomFacing.Right];
                }
                // then left
                else if (exitsAvailable.HasFlag(ExitDirection.Left))
                {
                    roomToPlace = roomsByModules[(int)moduleType][(int)RoomFacing.Left];
                }
                // this is here just in case, but it should not get here if there were no exits
                else
                {
                    roomToPlace = null;
                }
            }
            // if we get here and have a facing the same as the exit - then we can't place the room either
            else
            {
                roomToPlace = null;
            }
        }

        // the number of invalid exits (empty space is valid)
        ExitDirection exitsBlocked = CheckExitsBlocked(currentRowPos, currentColPos);

        // if there is a blocking room, we can't place this module
        if (exitsBlocked != ExitDirection.None)
        {
            roomToPlace = null;
        }

        return roomToPlace;

    } // end GetRoomWithFacing

    /// <summary>
    /// Checks to see if placed room would obscure the helm module
    /// </summary>
    /// <param name="currentRowPos">The room rowPos position attempting to be placed</param>
    /// <param name="currentColPos">The room colPos position attempting to be placed</param>
    /// <returns>true if the helm visibility would be obscured, false if not</returns>
    private bool CheckObscureHelm(int currentRowPos, int currentColPos)
    {
        bool helmObscured = false;

        if ((currentColPos == helmColPos) && (currentRowPos < helmRowPos))
        {
            helmObscured = true;
        }

        return helmObscured;
    }

    /// <summary>
    /// Goes through potential rooms and checks to see if it would be blocked
    /// </summary>
    /// <param name="currentRowPos">The ship layout rowPos position to test</param>
    /// <param name="currentColPos">The ship layout column positin to test</param>
    /// <returns></returns>
    public ExitDirection CheckExitsBlocked(int currentRowPos, int currentColPos)
    {
        ExitDirection exitsBlocked = ExitDirection.None;

        // Up
        // if we are in bounds, but not on the edge
        if ((currentRowPos - 1) >= 0)
        {
            // if there ist a module there, it has external facing that is facing us, then exit is blocked
            if ((shipLayout[currentRowPos - 1, currentColPos] != null) &&
                (shipLayout[currentRowPos - 1, currentColPos].externalFacing &&
                (shipLayout[currentRowPos - 1, currentColPos].roomFacing == RoomFacing.Down)))
            {
                exitsBlocked |= ExitDirection.Up;
            }
        }

        // Right
        // if we are in bounds, but not on the edge
        if ((currentColPos + 1) < shipWidth)
        {
            // if there ist a module there, it has external facing that is facing us, then exit is blocked
            if ((shipLayout[currentRowPos, currentColPos + 1] != null) &&
                (shipLayout[currentRowPos, currentColPos + 1].externalFacing &&
                (shipLayout[currentRowPos, currentColPos + 1].roomFacing == RoomFacing.Left)))
            {
                exitsBlocked |= ExitDirection.Right;
            }
        }

        // Down
        // if we are in bounds, but not on the edge
        if ((currentRowPos + 1) < shipHeight)
        {
            // if there ist a module there, it has external facing that is facing us, then exit is blocked
            if ((shipLayout[currentRowPos + 1, currentColPos] != null) &&
                (shipLayout[currentRowPos + 1, currentColPos].externalFacing &&
                (shipLayout[currentRowPos + 1, currentColPos].roomFacing == RoomFacing.Up)))
            {
                exitsBlocked |= ExitDirection.Down;
            }
        }

        // Left
        // if we are in bounds, but not on the edge
        if ((currentColPos - 1) >= 0)
        {
            // if there ist a module there, it has external facing that is facing us, then exit is blocked
            if ((shipLayout[currentRowPos, currentColPos - 1] != null) &&
                (shipLayout[currentRowPos, currentColPos - 1].externalFacing &&
                (shipLayout[currentRowPos, currentColPos - 1].roomFacing == RoomFacing.Right)))
            {
                exitsBlocked |= ExitDirection.Left;
            }
        }

        return exitsBlocked;

    } // end CheckExitsBlocked

    /// <summary>
    /// Sets up the requried rooms for the ship being genereated
    /// </summary>
    private void SetRequiredRooms()
    {
        // clear the required modules list
       requiredShipModules.Clear();

        // All ships must have:
        //  - a Helm (start with this) that is forward facing and on an edge, no rooms can be placed in front of the helm
        //  - Engine that is rear facing, no rooms can be placed in behind an engine - perhaps min number for ship size
        //  - Life Support, where there is 1 for every 4 crew members - base this on size
        //  - Science Bay
        //  - Hyperdrive

        // add a helm, science and hyperdrive as you only need one
        requiredShipModules.Add(ModuleType.Helm);
        requiredShipModules.Add(ModuleType.Hyperdrive);
        requiredShipModules.Add(ModuleType.Science);

        // both engines and life support are based on ship size
        numEnginesNeeded = shipSize / ENGINE_SHIP_DIVISOR;

        for (int i = 0; i < numEnginesNeeded; i++)
        {
            requiredShipModules.Add(ModuleType.Engine);
        }

        int numLifeSupportNeeded = shipSize / LIFE_SUPPORT_SHIP_DIVISOR;

        for (int i = 0; i < numLifeSupportNeeded; i++)
        {
            requiredShipModules.Add(ModuleType.LifeSupport);
        }

        //LogRequiredModuleList();

    } // end SetRequiredRooms

    /// <summary>
    /// Randomizes the ship dimensions based on the size and number of rooms the ship can have
    /// </summary>
    private void SetShipDimensions()
    {
        // need to randomize layout based on size (the double array to hold the modules) - start with true random
        // basing it on the maximum number of rooms in the ship - don't need to hit this, must be size * NUM_ROOMS_PER_SIZE rooms at a min
        shipSize = shipSizes.value + MIN_SHIP_SIZE;
        int maxNumRooms = (shipSize + 1) * NUM_ROOMS_PER_SIZE - 1;

        // We want a minimum number for the width and height (2 for now) and by dividing the maxNumRooms by 2 then adding one to get different size configurations
        shipHeight = Random.Range(MIN_SHIP_SIZE, (maxNumRooms / 2) + 1);
        shipWidth = Random.Range(MIN_SHIP_SIZE, (maxNumRooms / 2) + 1);

        int currentMaxNumShipRooms = (shipHeight * shipWidth);

        // if there aren't enough rooms, then we need to increase the size until there are
        while (currentMaxNumShipRooms < maxNumRooms)
        {
            if (shipHeight < shipWidth)
            {
                shipHeight++;
            }
            else
            {
                shipWidth++;
            }

            currentMaxNumShipRooms = (shipHeight * shipWidth);
        }

        // alternatively if there are too many rooms (the array is too large) bring it down in size (for now just using 2 * max rooms)
        while (currentMaxNumShipRooms > (maxNumRooms * 2) )
        {
            if (shipHeight > shipWidth)
            {
                shipHeight--;
            }
            else
            {
                shipWidth--;
            }

            currentMaxNumShipRooms = (shipHeight * shipWidth);
        }

        // set the ship layout array to the new size
        shipLayout = new RoomInfo[shipHeight, shipWidth];

        // reset the number of modules placed (a random value for the number to place based on the number of rooms)
        numModulesToPlace = Random.Range( (shipSize * NUM_ROOMS_PER_SIZE), (maxNumRooms + 1) );

    } // end SetShipDimensions

    /// <summary>
    /// You can populate any dropdown with any enum with this method
    /// Borrowed from this Unity Forums answer: https://answers.unity.com/questions/1804875/how-to-pass-a-dropdown-ui-value-to-an-enum-in-anot.html
    /// </summary>
    /// <param name="dropdown">The UI dropdown to populate</param>
    /// <param name="targetEnum">The enumerated type variable to populate it with</param>
    private void PopulateDropDownWithEnum(TMP_Dropdown dropdown, System.Enum targetEnum)
    {
        // get the type of enum
        System.Type enumType = targetEnum.GetType();

        // create a list to put into the options
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        // populate thi newOptions list
        for (int i = 0; i < System.Enum.GetNames(enumType).Length; i++)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(System.Enum.GetName(enumType, i)));
        }

        // clear the option list and add the enum options
        dropdown.ClearOptions();
        dropdown.AddOptions(newOptions);

    } // end PopulateDropDownWithEnum

    /// <summary>
    /// Debug code - used to print out the required module list to the console
    /// </summary>
    private void LogRequiredModuleList()
    {
        string output = "";

        foreach (ModuleType module in requiredShipModules)
        {
            output += module.ToString() + " ";
        }

        Debug.Log(output);

    } // end LogRequiredModuleList

    /*private void DebugDrawModule(RoomInfo room, int roomRow, int roomCol)
    {
        // only draw the module if debug is on
        if (debugIsOn)
        {
            // calculate the world position for this room to send down

            // as rooms can be offset from other rooms based on location in the ship, set the roomPos_z to the current rowPos times the height of a room
            float roomPos_z = 0 - (roomRow * RoomSpawner.ROOM_HEIGHT);

            // as rooms can be offset from other rooms based on location in the ship, set the roomPos_x to the current colPos times the width of a room
            float roomPos_x = 0 + (roomCol * RoomSpawner.ROOM_WIDTH);

            // instantiates the room objects based on the strings in the arrays 
            roomSpawner.BuildRoom(shipParentObject, room, roomPos_x, roomPos_z);
        }

    } // end DebugDrawModule*/
}
