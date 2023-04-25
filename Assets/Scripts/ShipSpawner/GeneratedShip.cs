using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneratedShip : MonoBehaviour
{
    // the power areas available for ships
    public enum ShipPowerAreas
    {
        HELM,
        WEAPONS,                // guns in the game - I like weapons better
        SHIELDS
    }
    public struct BotInitData
    {
        public int[] botRoomTypeCount;          // modules in ship per bot type
        public int[] botTypeCount;              // number of bots per typetype
        public int LifeSupportModules;          // number of life supports on this ship
    }

    // constants for this script
    public static int MAX_ENERGY_LEVEL = 6;             // the maximum level an energy system can reach
    private const float TILE_CENTER_OFFSET = 0.5f;
    private const int NUM_BOTS_PER_LIFE_SUPPORT = 4;
    private const int BOT_Y_OFFSET = 1;

    // public variables used by the ship manager for keeping track of the current ships state
    public ShipManager shipManagerScript;               // a link back to the ship manager so we can have access to the bot prefabs
    public RoomInfo[,] shipLayout;                      // the ship layout using room types
    public Grid2D shipPathingSystem;                    // the grid for the pathing system used by bots for this ship
    public Vector3 shipWorldOrigin;                     // the world position where the top left corner of the ship layout begins (for pathfinding)
    public int shipID;                                  // the id of the ship
    public int shipSize;                                // used to store the current generated ship size
    public Vector2Int mapLocation;                      // the map location of this ship for determinig distance, facing etc.
    public GeneratedShip currentTarget;

    public int currentSpeed;                            // the current speed of the ship
    public int currentDirection;                        // the heading of the current ship - used for facing of cannons
    public int requestedFacing;                         // used for targeting (-1 means no request to turn)
    public int outOfControlLevel;                       // how out of control the ship is, affects bot skills
    public int hullDamage;                              // the ships hull integrity

    public bool allScienceBaysBroken;
    public int numStoredScans;

    //public bool[] energySystemRequests;
    public Queue<ShipPowerAreas> energyUpdateQueue;
    public int[] energySystemLevels;

    // private variables only seen by this script
    public BotInitData botShipData = new BotInitData();
    private List<GameObject> bots;                      // a list to the AI bot crew for this ship, will use methods to update them
    private int currentBotBeingFollowed;                // an index into the list to get an active bot to follow

    /// <summary>
    /// Initial set up when this ship is created
    /// </summary>
    /// <param name="shipManager">A link back to the ship manager that controls this ship</param>
    /// <param name="ship">The ship layout for use in finding modules</param>
    /// <param name="worldPos">The ship's world position (should be the 0,0 for the grid layout and pathfinding</param>
    /// <param name="shipID">The id of the ship inside the list of the Ship Manager - may not be needed</param>
    /// <param name="shipSize">The size of the ship for use in calculating number of bots on the ship</param>
    /// <param name="mapLocation">The location to place this ship on our hypothetical map (a 50x50 world)</param>
    public void SetupShip(ShipManager shipManager, RoomInfo[,] ship, Vector3 worldPos, 
                          int shipID, int shipSize, Vector2Int mapLocation)
    {
        // store incoming data
        shipManagerScript = shipManager; 
        shipLayout = ship;
        shipWorldOrigin = worldPos;
        this.shipID = shipID;
        this.shipSize = shipSize;

        // set up base stats (somewhere in the imaginary map, facing up at speed 1
        this.mapLocation = mapLocation;

        // a target to focus on - need to be set up after ships are created
        this.currentTarget = null;

        // basic stats to start and set up bot AI control data
        currentSpeed = 1;
        currentDirection = 0;
        requestedFacing = -1;
        outOfControlLevel = 0;
        hullDamage = 0;
        allScienceBaysBroken = false;
        numStoredScans = 0;

        // set up the energy systems for this ship (using an enum to make easier to manipulate by others)
        int energySystemNum = System.Enum.GetNames(typeof(ShipPowerAreas)).Length;
        energyUpdateQueue = new Queue<ShipPowerAreas>();
        energySystemLevels = new int[energySystemNum];

        for (int i = 0; i < energySystemLevels.Length; i++)
        {
            energySystemLevels[i] = 0;
        }

        PopulateShip();

    } // end SetupShip

    /// <summary>
    /// Initializes the struct that holds bot module type counts
    /// </summary>
    public void SetupBotStuct()
    {
        botShipData.botRoomTypeCount = new int[System.Enum.GetNames(typeof(GenericBot.BotType)).Length];
        botShipData.botTypeCount = new int[System.Enum.GetNames(typeof(GenericBot.BotType)).Length];
    }

    /// <summary>
    /// Sets the target for this ship to chase down
    /// </summary>
    /// <param name="shipToTarget">The ship script to be targeted</param>
    public void SetTarget(GeneratedShip shipToTarget)
    {
        currentTarget = shipToTarget;

    } // end SetTarget

    /// <summary>
    /// Moves the ship in the direction it is facing, stopping at map edges
    /// </summary>
    public void MoveShip()
    {
        // get the direction of the ship and convert it to a Vector2Int
        // can use ints here as we are only on cardinal directions (0, 90, 180 and 270)
        // TODO: When moving to HEX areas, direction changes are more complex (turn of 60)
        Vector2Int moveVector = new Vector2Int();
        // sin 180 is returning 0 when it should be 1, probably rounding error...doing it manually
        //moveVector.x = (int)Mathf.Cos(currentDirection);
        //moveVector.y = (int)Mathf.Sin(currentDirection);

        if (currentDirection == 0) { moveVector.x = 1; }
        else if (currentDirection == 90) { moveVector.y = 1; }
        else if (currentDirection == 180) { moveVector.x = -1; }
        else if (currentDirection == 270) { moveVector.y = -1; }

        moveVector *= currentSpeed;

        mapLocation += moveVector;

        // check edge cases
        if (mapLocation.x < 0)
        { 
            mapLocation.x = 0; 
        }

        if (mapLocation.x > ShipManager.MAX_HEX_RANGE)
        {
            mapLocation.x = ShipManager.MAX_HEX_RANGE;
        }

        if (mapLocation.y < 0)
        {
            mapLocation.y = 0;
        }

        if (mapLocation.y > ShipManager.MAX_HEX_RANGE)
        {
            mapLocation.y = ShipManager.MAX_HEX_RANGE;
        }

    } // end MoveShip

    /// <summary>
    /// Updates the direction of the ship
    /// </summary>
    /// <param name="directionChange">The value to change the direction by</param>
    public void UpdateShipDirection(int directionChange)
    {
        this.currentDirection += directionChange;

        // if we went negative, wrap it back to positive
        if (this.currentDirection <= 0)
        {
            this.currentDirection += 360;
        }
        // if it went too positive bring it back around
        else if (this.currentDirection >= 360)
        {
            this.currentDirection -= 360;
        }

    } // end UpdateShipDirection

    /// <summary>
    /// Updates the Hull damage
    /// </summary>
    /// <param name="hullDamage"></param>
    public void UpdateHullDamage(int hullDamage)
    {
        this.hullDamage += hullDamage;

        // can't have a damage of less than zero (values should be all positive...)
        if (this.hullDamage < 0)
        {
            this.hullDamage = 0;
        }

    } // end UpdateHullDamage

    /// <summary>
    /// Updates the speed with to the new value, never less than zero
    /// TODO: When speed is over 4 - the ship should take damage
    /// </summary>
    /// <param name="speedChange">The value to change the speed by</param>
    public void UpdateSpeed(int speedChange)
    {
        currentSpeed += speedChange;

        // can't have a speed of less than zero
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }

    } // end UpdateSpeed

    /// <summary>
    /// Updates the Out of Control factor (OOC) with to the new value, never less than zero
    /// </summary>
    /// <param name="oocChange">The value to change the OOC by</param>
    public void UpdateOutOfControl(int oocChange)
    {
        outOfControlLevel += oocChange;

        // can't have a spped of less than zero
        if (outOfControlLevel < 0)
        {
            outOfControlLevel = 0;
        }

    } // end UpdateOutOfControl

    /// <summary>
    /// Updates the given energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="energySystem">The energy system to update</param>
    /// <param name="energyChange">The value to change the energy by</param>
    public void UpdateEnergy(int energySystem, int energyChange)
    {
        // if it is shields and all science bays are broken, don't add power
        if ((energySystem != (int)ShipPowerAreas.SHIELDS) || !allScienceBaysBroken)
        {
            energySystemLevels[energySystem] += energyChange;

            // can't have a spped of less than zero
            if (energySystemLevels[energySystem] < 0)
            {
                energySystemLevels[energySystem] = 0;
            }
            else if (energySystemLevels[energySystem] > MAX_ENERGY_LEVEL)
            {
                energySystemLevels[energySystem] = MAX_ENERGY_LEVEL;
            }
        }

    } // end UpdateEnergy

    /// <summary>
    /// Updates the speed with to the new value, never less than zero
    /// </summary>
    /// <param name="scansChange">The value to change the speed by</param>
    public void UpdateNumScans(int scansChange)
    {
        numStoredScans += scansChange;

        // can't have a speed of less than zero
        if (numStoredScans < 0)
        {
            numStoredScans = 0;
        }
        // cap the top too
        if (numStoredScans > shipSize)
        {
            numStoredScans = shipSize;
        }

    } // end UpdateNumScans

    /// <summary>
    /// Clears all the used markers at the end of each round
    /// </summary>
    public void ClearUsedMarkers()
    {
        // go through all the modules in this ship and clear the used markers - happens at end of round
        foreach (GameObject bot in bots)
        {
            foreach (RoomInfo module in bot.GetComponent<GenericBot>().myModules)
            {
                module.ClearUsedMarkers();
            }
        }

    } // end ClearUsedMarkers

    /// <summary>
    /// Returns the bot that is currently being followed
    /// TODO: This should check to see if the bot exists and return another if it doesn't
    ///         or make it so it adjusts the bot accordingly (perhaps instead of prev and next that would call this)
    /// </summary>
    /// <returns></returns>
    public GameObject GetBotToFollow()
    {
        return (bots[currentBotBeingFollowed]);

    } // GetBotToFollow

    /// <summary>
    /// Populates a ship with the given bots for that ship base on number of life supports and room types
    /// </summary>
    /// <param name="shipID"></param>
    private void PopulateShip()
    {
        // create the list to store the bots
        bots = new List<GameObject>();

        // calculate how many bots this ship can have
        int numBots = (botShipData.LifeSupportModules * NUM_BOTS_PER_LIFE_SUPPORT);

        // need to know how many bots that have been placed and the type
        int botsPlaced = 0;

        // go through the ship and find the core rooms and place bots there first according to their station
        // - Helm - (Command: glorified pilot) - only one helm but secondary bots can go in other stations
        InstantiateBot((int)GenericBot.BotType.COMMAND, GenericBot.botModuleTypes[(int)GenericBot.BotType.COMMAND]);
        botsPlaced++;
        botShipData.botTypeCount[(int)GenericBot.BotType.COMMAND]++;

        // - Engine Room (Engineering) - at least one engine, but there can be other engineering rooms for secondary bots
        InstantiateBot((int)GenericBot.BotType.ENGINEERING, GenericBot.botModuleTypes[(int)GenericBot.BotType.ENGINEERING]);
        botsPlaced++;
        botShipData.botTypeCount[(int)GenericBot.BotType.ENGINEERING]++;

        // - Science Bay (Science) - there is at least one science bay, but there are other science rooms for secondary bots
        InstantiateBot((int)GenericBot.BotType.SCIENCE, GenericBot.botModuleTypes[(int)GenericBot.BotType.SCIENCE]);
        botsPlaced++;
        botShipData.botTypeCount[(int)GenericBot.BotType.SCIENCE]++;

        // - First Weapon (Security) - TODO: fix ships to always have one weapon!
        InstantiateBot((int)GenericBot.BotType.SECURITY, GenericBot.botModuleTypes[(int)GenericBot.BotType.SECURITY]);
        botsPlaced++;
        botShipData.botTypeCount[(int)GenericBot.BotType.SECURITY]++;

        // if after the first four bots are placed, and there are more bots, place them based random jobs and rooms
        while (botsPlaced < numBots)
        {
            // place extra bots based on modules they can use and how many we have already

            // get the ratio of bot type per modules and choose the one the the lowest number
            // Using the index of the array as the bot type identifier, so need it to be the same length as the prefab array
            float[] botRatioArray = new float[shipManagerScript.botPrefabs.Length];

            // Won't need to place a second command bot the extra option will be for operation bots later
            // - operations are generic bots that can go anywhere - not using in this game
            for (int i = 0; i < botRatioArray.Length; i++)
            {
                botRatioArray[i] = (float)botShipData.botTypeCount[i] / botShipData.botRoomTypeCount[i];
            }

            // starting at 1 as we aren't adding more command bots
            int lowestNumIndex = 1;
            int lowestRatioIndex = 1;
            int lowestNumBots = int.MaxValue;
            float lowestRatio = float.MaxValue;
            bool haveTie = false;

            // go through the bot placed array and find the bot that has the lowest number of those types
            // also find the smallest ratio - that will be the tie breaker bot type to place
            // stopping one early as the program is not using operation bots yet
            for (int botyType = 1; botyType < botRatioArray.Length - 1; botyType++) 
            { 
                if (botShipData.botTypeCount[botyType] <= lowestNumBots)
                {
                    if (botShipData.botTypeCount[botyType] == lowestNumBots)
                    {
                        haveTie = true;
                    }

                    lowestNumIndex = botyType;
                    lowestNumBots = botShipData.botTypeCount[botyType];
                }

                if (botRatioArray[botyType] < lowestRatio)
                {
                    lowestRatioIndex = botyType;
                    lowestRatio = botRatioArray[botyType];

                    // testing - if we have a tie ealier, it will still take the lowest ratio for all,
                    // breaking here should prevent that
                    if (haveTie)
                    {
                        break;
                    }
                }
            }

            // if there was a tie, then use the ratio
            if (haveTie)
            {
                InstantiateBot(lowestRatioIndex, GenericBot.botModuleTypes[lowestRatioIndex]);
                botShipData.botTypeCount[lowestRatioIndex]++;
            }
            else
            {
                InstantiateBot(lowestNumIndex, GenericBot.botModuleTypes[lowestNumIndex]);
                botShipData.botTypeCount[lowestNumIndex]++;
            }

            botsPlaced++;
        }

        // set the value of the bot to follow by default on this ship, should be command to start
        currentBotBeingFollowed = 0;

    } // end PopulateShip

    private void InstantiateBot(int botType, RoomData.ModuleType[] moduleTypes)
    {
        Vector2Int startGridPos = GetRoomStartGridLocation(moduleTypes);

        // calculate the gridPosition of the room based of the ship origin position (it's world position for its Grid 0,0)
        Vector3 startPos = new Vector3(shipWorldOrigin.x + startGridPos.y + TILE_CENTER_OFFSET, BOT_Y_OFFSET, 
                                        shipWorldOrigin.z - startGridPos.x - TILE_CENTER_OFFSET);

        GameObject bot = Instantiate(shipManagerScript.botPrefabs[botType], startPos, Quaternion.identity);
        GenericBot botScript = bot.GetComponent<GenericBot>();
        botScript.SetShip(this);

        // get the rooms associated with this bot
        for (int i = 0; i < moduleTypes.Length; i++)
        {
            List<RoomInfo> roomsForThisBot = FindRoomsInShip(moduleTypes[i]);

            foreach (RoomInfo module in roomsForThisBot) 
            {
                botScript.AddModule(module);
            }
        }

        // add the bot to the ship bot list
        bots.Add(bot);

        // set the bot parent to the ship game object so it is easier to find the crew
        bot.transform.SetParent(this.gameObject.transform);

    } // end InstantiateBot

    /// <summary>
    /// Gets the starting location of the Grid in the room for a bot based on modules it uses
    ///     TODO: Need to fix this for multiple bots of the same type...perhaps keep track of modules that have bots
    /// </summary>
    /// <param name="moduleTypes">The bots modules used by the bot</param>
    /// <returns>the Grid position for the module types given, defaults to center of life support module</returns>
    private Vector2Int GetRoomStartGridLocation(RoomData.ModuleType[] moduleTypes)
    {
        // calculate the position for this bot (first room of the first module type) - defaulting to life support
        RoomInfo startRoom = FindRoomInShip(RoomData.ModuleType.LifeSupport);

        // assume the center of the room (for life support)
        Vector2Int startGridPos = startRoom.roomGridPos;
        startGridPos.x += 3;
        startGridPos.y += 3;

        // now try to find a room that fits this bot
        bool startLocFound = false;
        int moduleIndex = 0;

        // find the first un-occupied room for this bot of its type (if all are occupied, go to operations)
        while (!startLocFound && (moduleIndex < moduleTypes.Length))
        {
            // TODO: fix this to use a while loop and increase the index incrementally until there are none left to check
            startRoom = FindUnoccupiedRoomInShip(moduleTypes[moduleIndex]);

            // If the room is available, we are done
            if (startRoom != null)
            {
                startLocFound = true;
            }
            // otherwise we are move to the next module for this bot
            else
            {
                moduleIndex++;
            }
        }

        if (startLocFound)
        {
            // The Room should not be fully occupied if we get here (unless we are in the default room)
            int terminalToUse = startRoom.GetUnoccupiedTerminal();

            // we check just in case
            if (terminalToUse >= 0)
            {
                startGridPos = startRoom.GetTerminalLoacation(terminalToUse).mapLocation;
                startRoom.SetTerminalOccupied(terminalToUse);
            }
        }
        // try going through operations modules?

        return startGridPos;

    } // end GetRoomStartGridLocation

    /// <summary>
    /// Goes through the ship layout and finds the first unoccupied room of the given type
    /// </summary>
    /// <param name="moduleToFind">The type of room to find</param>
    /// <returns>The found room or null if not found</returns>
    private RoomInfo FindUnoccupiedRoomInShip(RoomData.ModuleType moduleToFind)
    {
        // go through the ship layout until the ModuleType is found - finds the first one for now
        for (int roomRow = 0; roomRow < shipLayout.GetLength(0); roomRow++)
        {
            for (int roomCol = 0; roomCol < shipLayout.GetLength(1); roomCol++)
            {
                RoomInfo roomToCheck = shipLayout[roomRow, roomCol];

                if ((roomToCheck != null) && (roomToCheck.moduleType == moduleToFind))
                {
                    if (!roomToCheck.IsFullyOccupied())
                    {
                        return shipLayout[roomRow, roomCol];
                    }
                }
            }
        }

        return null;

    } // FindUnoccupiedRoomInShip

    /// <summary>
    /// Goes through the ship layout and finds the first room of the given type
    /// </summary>
    /// <param name="moduleToFind">The type of room to find</param>
    /// <returns>The found room or null if not found</returns>
    private RoomInfo FindRoomInShip(RoomData.ModuleType moduleToFind)
    {
        // go through the ship layout until the ModuleType is found - finds the first one for now
        for (int roomRow = 0; roomRow < shipLayout.GetLength(0); roomRow++)
        {
            for (int roomCol = 0; roomCol < shipLayout.GetLength(1); roomCol++)
            {
                RoomInfo roomToCheck = shipLayout[roomRow, roomCol];

                if ((roomToCheck != null) && (roomToCheck.moduleType == moduleToFind))
                {
                   return shipLayout[roomRow, roomCol];
                }
            }
        }

        return null;

    } // FindRoomInShip

    /// <summary>
    /// Goes through the ship layout and finds all the rooms in the ship with the given module type
    /// </summary>
    /// <param name="moduleType">the module type to find</param>
    /// <returns>A list of all the modules of the given type, empty if there were none</returns>
    private List<RoomInfo> FindRoomsInShip(RoomData.ModuleType moduleType)
    {
        List<RoomInfo> rooms = new List<RoomInfo>();

        // go through the ship layout until the ModuleType is found - finds all of the rooms of the given module typefor (int roomRow = 0; roomRow < shipLayout.GetLength(0); roomRow++)
        for (int roomRow = 0; roomRow < shipLayout.GetLength(0); roomRow++)
        {
            for (int roomCol = 0; roomCol < shipLayout.GetLength(1); roomCol++)
            {
                if ((shipLayout[roomRow, roomCol] != null) && (shipLayout[roomRow, roomCol].moduleType == moduleType))
                {
                    rooms.Add(shipLayout[roomRow, roomCol]);
                }
            }
        }

        return rooms;

    } // FindRoomsInShip

}
