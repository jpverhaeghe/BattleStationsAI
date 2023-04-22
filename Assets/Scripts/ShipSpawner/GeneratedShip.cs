using AlanZucconi.AI.PF;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class GeneratedShip : MonoBehaviour
{
    // constants for this script
    private const float TILE_CENTER_OFFSET = 0.5f;
    private const int NUM_BOTS_PER_LIFE_SUPPORT = 4;
    private const int BOT_Y_OFFSET = 1;

    // public variables used by the ship manager for keeping track of the current ships state
    public RoomInfo[,] shipLayout;                      // the ship layout using room types
    public Grid2D shipPathingSystem;                    // the grid for the pathing system used by bots for this ship
    public Vector3 shipWorldOrigin;                     // the world position where the top left corner of the ship layout begins (for pathfinding)
    //public Vector3 shipHelmPos;                         // holds the helm player position for walking through the ship - may be able to remove!
    public int shipID;                                  // the id of the ship
    public int shipSize;                                // used to store the current generated ship size
    public int numLifeSupports;                         // exposing this so we don't have to re-count later

    public int currentSpeed;                            // the current speed of the ship
    public int outOfControlLevel;                       // how out of control the ship is, affects bot skills
    public int helmEnergyLevel;                         // the energy amount in the ships drive systems (turning, etc.)
    public int weaponEnergyLevel;                       // the energy amount in the weapons systems
    public int shieldEnergyLevel;                       // the energy amount in the shields systems

    public int hullIntegrity;                           // the ships hull integrity (eventually will depend on modules slagged)

    private ShipManager shipManagerScript;              // a link back to the ship manager so we can have access to the bot prefabs
    private List<GameObject> bots;                      // a list to the AI bot crew for this ship, will use methods to update them
    private int currentBotBeingFollowed;                // an index into the list to get an active bot to follow

    /// <summary>
    /// Initial set up when this ship is created
    /// </summary>
    /// <param name="shipManager">A link back to the ship manager that controls this ship</param>
    /// <param name="ship">The ship layout for use in finding modules</param>
    /// <param name="worldPos">The ship's world position (should be the 0,0 for the grid layout and pathfinding</param>
    ///// <param name="helmPos">The helm position for placing bots</param>
    /// <param name="shipID">The id of the ship inside the list of the Ship Manager - may not be needed</param>
    /// <param name="shipSize">The size of the ship for use in calculating number of bots on the ship</param>
    //public void SetupShip(ShipManager shipManager, RoomInfo[,] ship, Vector3 worldPos, Vector3 helmPos, int shipID, int shipSize)
    public void SetupShip(ShipManager shipManager, RoomInfo[,] ship, Vector3 worldPos, int shipID, int shipSize)
    {
        // store incoming data
        shipManagerScript = shipManager; 
        shipLayout = ship;
        shipWorldOrigin = worldPos;
        //shipHelmPos = helmPos;
        //shipHelmPos.y += BOT_Y_OFFSET;
        this.shipID = shipID;
        this.shipSize = shipSize;

        // set up base stats
        currentSpeed = 1;
        outOfControlLevel = 0;
        helmEnergyLevel = 0;
        weaponEnergyLevel = 0;
        shieldEnergyLevel = 0;

        // set up hull integrity for temp damage tracker (will move to core slagged modules)
        hullIntegrity = 5;

        PopulateShip();

    } // end SetupShip

    /// <summary>
    /// Performs the necessary adjustments for a ship at the end of the round
    /// </summary>
    public void ShipRoundCleanUp()
    {
        // adjust the levels per the rules (all energy and speed are reduced by 1)
        UpdateSpeed(-1);
        UpdateHelmEnergy(-1);
        UpdateWeaponsEnergy(-1);
        UpdateShieldEnergy(-1);

        // TODO: Remove counters on any modules (engineering, helm, weapons, etc.)

    } // ShipRoundCleanUp

    /// <summary>
    /// Updates the speed with to the new value, never less than zero
    /// TODO: When speed is over 4 - the ship should take damage
    /// </summary>
    /// <param name="speedChange">The value to change the speed by</param>
    public void UpdateSpeed(int speedChange)
    {
        currentSpeed += speedChange;

        // can't have a spped of less than zero
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }

    } // end UpdateSpeed

    /// <summary>
    /// Updates the Out of Control factor (OOC) with to the new value, never less than zero
    /// </summary>
    /// <param name="speedChange">The value to change the OOC by</param>
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
    /// Updates the helm energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="helmEnergyChange">The value to change the helm energy by</param>
    public void UpdateHelmEnergy(int helmEnergyChange)
    {
        helmEnergyLevel += helmEnergyChange;

        // can't have a spped of less than zero
        if (helmEnergyLevel < 0)
        {
            helmEnergyLevel = 0;
        }

    } // end UpdateHelmEnergy

    /// <summary>
    /// Updates the weapons energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="weaponsEnergyChange">The value to change the weapons energy by</param>
    public void UpdateWeaponsEnergy(int weaponsEnergyChange)
    {
        weaponEnergyLevel += weaponsEnergyChange;

        // can't have a spped of less than zero
        if (weaponEnergyLevel < 0)
        {
            weaponEnergyLevel = 0;
        }

    } // end UpdateWeaponsEnergy

    /// <summary>
    /// Updates the shield energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="shieldEnergyChange">The value to </param>
    public void UpdateShieldEnergy(int shieldEnergyChange)
    {
        shieldEnergyLevel += shieldEnergyChange;

        // can't have a spped of less than zero
        if (shieldEnergyLevel < 0)
        {
            shieldEnergyLevel = 0;
        }

    } // end UpdateShieldEnergy

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
        int numBots = (numLifeSupports * NUM_BOTS_PER_LIFE_SUPPORT);

        int placedBots = 0;

        // go through the ship and find the core rooms and place bots there first according to their station
        // - Helm - (Command: glorified pilot) - only one helm but secondary bots can go in other stations
        InstantiateBot(GenericBot.BotType.COMMAND, CommandBot.modules);
        placedBots++;

        // - Engine Room (Engineering) - at least one engine, but there can be other engineering rooms for secondary bots
        InstantiateBot(GenericBot.BotType.ENGINEERING, EngineeringBot.modules);
        placedBots++;

        // - Science Bay (Science) - there is at least one science bay, but there are other science rooms for secondary bots
        InstantiateBot(GenericBot.BotType.SCIENCE, ScienceBot.modules);
        placedBots++;

        // - First Weapon (Security) - TODO: fix ships to always have one weapon!
        InstantiateBot(GenericBot.BotType.SECURITY, SecurityBot.modules);
        placedBots++;

        // if after the first four bots are placed, and there are more bots, place them based random jobs and rooms
        if (placedBots < numBots)
        {
            // - operations are generic bots that can go anywhere
        }


        // set the value of the bot to follow by default on this ship, should be command to start
        currentBotBeingFollowed = 0;

    } // end PopulateShip

    private void InstantiateBot(GenericBot.BotType botType, RoomData.ModuleType[] moduleTypes)
    {
        // calculate the position for this bot (first room of the first module type
        RoomInfo startRoom = FindRoomInShip(moduleTypes[0]);
        Vector2Int startGridPos = startRoom.GetTerminalLoacation(0);

        // calculate the gridPosition of the room based of the ship origin position (it's world position for its Grid 0,0)
        Vector3 startPos = new Vector3(shipWorldOrigin.x + startGridPos.y + TILE_CENTER_OFFSET, BOT_Y_OFFSET, 
                                        shipWorldOrigin.z - startGridPos.x - TILE_CENTER_OFFSET);

        GameObject bot = Instantiate(shipManagerScript.botPrefabs[(int)botType], startPos, Quaternion.identity);
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
                if ((shipLayout[roomRow, roomCol] != null) && (shipLayout[roomRow, roomCol].moduleType == moduleToFind))
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
