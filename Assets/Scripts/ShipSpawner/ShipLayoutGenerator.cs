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
    private int helmRowPos;                                 // the position of the helm so we can make sure not to put modules in front of it
    private int helmColPos;                                 // the position of the helm so we can make sure not to put modules in front of it

    //private ShipManager shipManager;                        // for testing purposes so we can see the ship being created room by room

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

    } // end Start

    /// <summary>
    /// Generates a ship layout based on data from the UI - such as the size drop down
    /// </summary>
    /// <returns>RoomInfo[,] - the layout in a double array format of RoomInfo</returns>
    public RoomInfo[,] GenerateShipLayout()
    //public RoomInfo[,] GenerateShipLayout(ShipManager shipManager)
    {
        // debug code
        //this.shipManager = shipManager;

        // TODO: Add race as an option so we can change building based on the rules of each race
        //       For example: Tentac - External modules ignore facing restrictions and have 360 degree firing arc.

        // clear up the list to keep track of modules placed and rooms to fill
        placedShipModules.Clear();
        roomsToFill.Clear();

        // set up the ship dimensions
        SetShipDimensions();

        // create a list of the modules that must be in the ship - will remove from this list if a module of that type is placed
        SetRequiredRooms();

        // to start building the ship we will add a helm as there is only one, then build out modules, ending with engines
        PlaceHelmModule();

        // debug information
        Debug.Log("Ship Layout is " + shipHeight + " rows and " + shipWidth + " cols: Helm is at row " + helmRowPos + " and col " + helmColPos);

        // go through the helm exits one by one and build out the next room, subtracting one from the num rooms for the ship
        PlaceModules();

        // now place engine modules
        PlaceEngineModules();

        // testing for ships that have less than the minimum number of rooms (just recall the Generate method)
        if (numModulesToPlace > 0)
        {
            LogRequiredModuleList();
            GenerateShipLayout();
            //GenerateShipLayout(shipManager);
        }

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

                // debug code to preview the ship as it is built
                //shipManager.ClearShip();
                //shipManager.CreateShip(shipLayout, 0, 0);
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
        // TODO: Certain modules only need one (unless we want redundancy - in case one goes down)
        //          - no need for two teleporters
        //          - do we need more life supports than required
        //          - would like to preference things like cargo bays, weapons, etc.

        // set up the potential room data to be null by default
        RoomInfo roomToPlace = null;

        // don't place a module if it is obscuring the helm and check to see if the room has already been placed
        if (!CheckObscureHelm(currentRowPos, currentColPos) && (shipLayout[currentRowPos, currentColPos] == null))
        {
            // choose a random module - subtracting two as we don't randomize helm and engines are done after all other modules
            ModuleType moduleType = (ModuleType)Random.Range(0, System.Enum.GetNames(typeof(ModuleType)).Length - 2);

            // trying to keep modules from duplicating too much
            // - want to use while loop, but if all modules in the list, it may get stuck in infinite loop
            //while (!placedShipModules.Contains(moduleType)) { }

            // verification that the module must be chosen from the required list here before moving on
            // not sure if less than should be here, but just in case adding it so we don't go below
            if (numModulesToPlace <= requiredShipModules.Count)
            {
                // go until we get a type that is in the required list
                while (!requiredShipModules.Contains(moduleType))
                {
                    // choose another random module - subtracting two as we don't randomize helm and engines are done after all other modules
                    moduleType = (ModuleType)Random.Range(0, System.Enum.GetNames(typeof(ModuleType)).Length - 2);
                }
            }

            roomToPlace = GetRoomWithFacing(moduleType, currentRowPos, currentColPos);
        }

        return roomToPlace;

    } // end PlaceModule

    /// <summary>
    /// Gets the room to place if the room can be placed with appropriate facing
    /// </summary>
    /// <param name="moduleType">The the module type we are going to place</param>
    /// <param name="currentRowPos">The current row position to place this module in the ship array</param>
    /// <param name="currentColPos">The current column position to place this module in the ship array</param>
    /// <returns></returns>
    private RoomInfo GetRoomWithFacing(ModuleType moduleType, int currentRowPos, int currentColPos)
    {
        // assume everything is facing the direction of the helm for now and grab the room we want to place
        RoomFacing currentFacing = RoomFacing.Up;
        RoomInfo roomToPlace = roomsByModules[(int)moduleType][(int)currentFacing];

        // some room modules must be external facing such as cannons, cloaking, engines, helm, missile bays, mine layer
        //  - Cargo bays do not require external facing, however some will need it to put certain modules into them
        //  - Helm was placed first and is not looked at here
        //  - Engines must face the way the helm is facing - but are placed last and not looked at here
        //  - All other external facing modules have walls on the exit that should face the outside
        if (roomToPlace.externalFacing)
        {
            bool facingFound = false;

            // if there are rooms around, make sure we are not putting a wall to them
            // if we are in bounds, but not on the edge
            if ((currentRowPos - 1) >= 0)
            {
                // if there isn't a module there, we can face that way
                if (shipLayout[currentRowPos - 1, currentColPos] == null)
                {
                    currentFacing = RoomFacing.Up;
                    facingFound = true;
                }
            }
            // if we are on the edge, then just face that direction
            else
            {
                currentFacing = RoomFacing.Up;
                facingFound = true;
            }

            // Right - check only if we didn't come from there
            if (!facingFound)
            {
                // if we are in bounds, but not on the edge
                if ((currentColPos + 1) < shipWidth)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos, currentColPos + 1] == null)
                    {
                        currentFacing = RoomFacing.Right;
                        facingFound = true;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    currentFacing = RoomFacing.Right;
                    facingFound = true;
                }
            }

            // Down - check only if we didn't come from there
            if (!facingFound)
            {
                // if we are in bounds, but not on the edge
                if ((currentRowPos + 1) < shipHeight)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos + 1, currentColPos] == null)
                    {
                        currentFacing = RoomFacing.Down;
                        facingFound = true;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    currentFacing = RoomFacing.Down;
                    facingFound = true;
                }
            }

            // Left - check only if we didn't come from there
            if (!facingFound)
            {
                // if we are in bounds, but not on the edge
                if ((currentColPos - 1) >= 0)
                {
                    // if there isn't a module there, we can face that way
                    if (shipLayout[currentRowPos, currentColPos - 1] == null)
                    {
                        currentFacing = RoomFacing.Left;
                        facingFound = true;
                    }
                }
                // if we are on the edge, then just face that direction
                else
                {
                    currentFacing = RoomFacing.Left;
                    facingFound = true;
                }
            }

            // change the room if the facing changed and isn't the exit direction
            if (facingFound)
            {
                roomToPlace = roomsByModules[(int)moduleType][(int)currentFacing];
            }
            // if we get here and have a facing the same as the exit - then we can't place the room either
            else 
            { 
                roomToPlace = null;
            }
        }
        // non external facing rooms need to have exit space in all directions
        else
        {
            // the number of invalid exits (empty space is valid)
            int numInvalidExits = 0;

            // Up
            // if we are in bounds, but not on the edge
            if ((currentRowPos - 1) >= 0)
            {
                // if there ist a module there, it has external facing that is facing us, then exit is blocked
                if ((shipLayout[currentRowPos - 1, currentColPos] != null) &&
                    (!shipLayout[currentRowPos - 1, currentColPos].externalFacing &&
                    (shipLayout[currentRowPos - 1, currentColPos].roomFacing != RoomFacing.Down)))
                {
                    numInvalidExits++;
                }
            }

            // Right
            // if we are in bounds, but not on the edge
            if ((currentColPos + 1) < shipWidth)
            {
                // if there ist a module there, it has external facing that is facing us, then exit is blocked
                if ((shipLayout[currentRowPos, currentColPos + 1] != null) &&
                    (!shipLayout[currentRowPos, currentColPos + 1].externalFacing &&
                    (shipLayout[currentRowPos, currentColPos + 1].roomFacing != RoomFacing.Left)))
                {
                    numInvalidExits++;
                }
            }

            // Down
            // if we are in bounds, but not on the edge
            if ((currentRowPos + 1) < shipHeight)
            {
                // if there ist a module there, it has external facing that is facing us, then exit is blocked
                if ((shipLayout[currentRowPos + 1, currentColPos] != null) &&
                    (!shipLayout[currentRowPos + 1, currentColPos].externalFacing &&
                    (shipLayout[currentRowPos + 1, currentColPos].roomFacing != RoomFacing.Up)))
                {
                    numInvalidExits++;
                }
            }

            // Left
            // if we are in bounds, but not on the edge
            if ((currentColPos - 1) >= 0)
            {
                // if there ist a module there, it has external facing that is facing us, then exit is blocked
                if ((shipLayout[currentRowPos, currentColPos - 1] != null) &&
                    (!shipLayout[currentRowPos, currentColPos - 1].externalFacing &&
                    (shipLayout[currentRowPos, currentColPos - 1].roomFacing != RoomFacing.Right)))
                {
                    numInvalidExits++;
                }
            }

            // if there is a blocking room, we can't place this module
            if (numInvalidExits > 0)
            {
                roomToPlace = null;
            }
        }

        return roomToPlace;

    } // end GetRoomWithFacing

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
        RoomInfo engineToPlace = roomsByModules[(int)ModuleType.Engine][(int)RoomFacing.Up];

        // go through each engine and try to place them at the back of the ship, starting at the center and working out
        // based on where the helm is
        for (int engineNum = 0; engineNum < numEnginesNeeded; engineNum++)
        {
            bool foundPlacement = false;
            int[] enginePos = new int[2];

            // find a position as far away from the helm that is attached to the ship

            // we go from the bottom up here and find the first empty space we can add an engine too
            int row = shipHeight - 1;

            while (!foundPlacement && (row >= 0))
            {
                foundPlacement = CheckEngineColPlacement(row, enginePos);
                row--;
            }

            // place the engine if space was found
            if (foundPlacement)
            {
                shipLayout[enginePos[0], enginePos[1]] = engineToPlace;

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
    /// <param name="row">The row of the ship layout to look at</param>
    /// <param name="enginePos">An array that will contain the engine positions if a placement was found</param>
    /// <returns></returns>
    private bool CheckEngineColPlacement(int row, int[] enginePos)
    {
        bool foundPlacement = false;

        int colIncrementor = 1;
        int widthTest = shipWidth / 2;

        // col starting placement will depend on the width to get all since we start moving right,
        // so if the width of the array is even, we need to start at one less and test against that value
        if (shipWidth % 2 == 0)
        {
            widthTest--;
        }

        int col = widthTest;

        int[] offsets = new int[2];

        while (!foundPlacement && ((col >= 0) && (col < shipWidth)))
        {
            if (shipLayout[row, col] != null)
            {
                // can we place below (there is room)
                offsets[0] = 1;
                foundPlacement = CheckEnginePlacement(row, col, offsets[0], offsets[1], RoomFacing.Up);

                // look right to see if we can add there (if we didn't find the location already)
                if (!foundPlacement)
                {
                    offsets[0] = 0;
                    offsets[1] = 1;
                    foundPlacement = CheckEnginePlacement(row, col, offsets[0], offsets[1], RoomFacing.Left);
                }

                // lastly look left as we can't put things above (if we didn't find the location already)
                if (!foundPlacement)
                {
                    offsets[0] = 0;
                    offsets[1] = -1;
                    foundPlacement = CheckEnginePlacement(row, col, offsets[0], offsets[1], RoomFacing.Right);
                }

                enginePos[0] = row + offsets[0];
                enginePos[1] = col + offsets[1];
            }

            if (col <= widthTest)
            {
                col += colIncrementor;
            }
            else
            {
                col -= colIncrementor;
            }

            colIncrementor++;
        }

        return foundPlacement;

    } // end CheckEngineColPlacement

    private bool CheckEnginePlacement(int row, int col, int rowOffset, int colOffset, RoomFacing testFacing)
    {
        // TODO: Do we need to look left and right to verify if we can place as well (may be moot if outward facing works well)
        bool foundPlacement = false;

        // create some variables to hold the updated row and column to check
        int newRow = (row + rowOffset);
        int newCol = (col + colOffset);

        // test to see if the updated value is not obscuring the helm and is in range (only one of these should actually change)
        if (!CheckObscureHelm(newRow, newCol) && 
            (newRow >= 0) && (newRow < shipHeight) && (newCol >= 0) && (newCol < shipWidth))
        {
            // check to see if the space is available, the module we are coming from doesn't have a wall this way
            if ((shipLayout[newRow, newCol] == null) &&
                 (!shipLayout[row, col].externalFacing ||
                  (shipLayout[row, col].roomFacing != testFacing)))
            {
                // lastly check that the module we are placing isn't being placed over an exit
                // (always faces the up - the helm's direction)
                // TODO: Need to make it so it looks at potential neighbors aren't external facing towards this room!

                // check in bounds (don't need to look below an edge case)
                if ((newRow + 1) < shipHeight - 1)
                {
                    // can only place here if there is no room below
                    if (shipLayout[(newRow + 1), newCol] == null)
                    {
                        foundPlacement = true;
                    }
                }
                // if on the edge, then just place it
                else if (newRow == (shipHeight - 1))
                {
                    foundPlacement = true;
                }
            }
        }

        return foundPlacement;

    } // end CheckEnginePlacement

    /// <summary>
    /// Adds potential rooms to the queue of rooms to place if the room would be in bounds
    /// </summary>
    /// <param name="rowPos">the row position of the room to place</param>
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
    /// Checks to see if placed room would obscure the helm module
    /// </summary>
    /// <param name="currentRowPos">The room row position attempting to be placed</param>
    /// <param name="currentColPos">The room col position attempting to be placed</param>
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
}
