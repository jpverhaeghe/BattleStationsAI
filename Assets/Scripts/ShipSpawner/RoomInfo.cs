using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomData;

public class RoomInfo
{
    // used to keep track of terminals in the game
    public struct TerminalInfo
    {
        public Vector2Int mapLocation;
        public Renderer starTileRenderer;
    }

    // This classes public variables used by other classes
    // TODO: Make these private and use getters/setters (probably just need getters)
    public string roomName;
    public RoomType roomType;
    public RoomFacing roomFacing;
    public ModuleType moduleType;
    public RoomTiles[,] roomTiles;
    public Vector2Int roomGridPos;
    public bool externalFacing;

    private List<TerminalInfo> terminalLocations = new List<TerminalInfo>();
    private Vector3 roomWorldPos;
    private int numUsedMarkers = 0;
    private bool broken = false;
    private bool[] occupiedTerminals;

    /// <summary>
    /// the constructor for this class
    /// </summary>
    /// <param name="roomName">The name of the room type</param>
    /// <param name="roomType">The room type</param>
    /// <param name="roomTiles">The room tile layout</param>
    public RoomInfo(string roomName, RoomType roomType, RoomFacing roomFacing, ModuleType moduleType, RoomTiles[,] roomTiles, bool externalFacing)
    {
        this.roomName = roomName;
        this.roomType = roomType;
        this.roomFacing = roomFacing;
        this.moduleType = moduleType;
        this.roomTiles = roomTiles;
        this.externalFacing = externalFacing;

    } // end RoomInfo constructor

    /// <summary>
    /// Gets all the terminal locations for this module - for pathfinding
    /// </summary>
    /// <returns>A list of all current terminal locations in this room</returns>
    public List<TerminalInfo> GetTerminalLoacations() 
    {  
        return terminalLocations;

    } // end GetTerminalLoacations

    /// <summary>
    /// Returns the terminal location at the index for this module - for pathfinding
    /// If the index is out of bounds, returns the first location to not break pathfinding
    /// </summary>
    /// <param name="terminalIndex">The index of the terminal location</param>
    /// <returns>A list of all current terminal locations in this room</returns>
    public TerminalInfo GetTerminalLoacation(int terminalIndex)
    {
        if ((terminalIndex >= 0) && (terminalIndex < terminalLocations.Count))
        {
            return terminalLocations[terminalIndex];
        }
        else
        {
            return terminalLocations[0];
        }

    } // end GetTerminalLoacation

    /// <summary>
    /// Adds a terminal location to this room - for pathfinding
    /// </summary>
    /// <param name="mapLocation">the terminal location on the Grid2D</param>
    /// <param name="starTileRenderer">the renderer for the star tile that is this terminal location</param>
    public void AddTerminalLocation(Vector2Int mapLocation, Renderer starTileRenderer)
    {
        TerminalInfo terminalInfo = new TerminalInfo();
        terminalInfo.mapLocation = mapLocation;
        terminalInfo.starTileRenderer = starTileRenderer;
        terminalLocations.Add(terminalInfo);

    } // AddTerminalLocation

    /// <summary>
    /// Gets the world position of this room
    /// </summary>
    /// <returns>The Vector 3 containing the world position for this room</returns>
    public Vector3 GetRoomWorldPos()
    { 
        return roomWorldPos;

    } // end GetRoomWorldPos

    /// <summary>
    /// Sets the world position of this room (the top left corner)
    /// </summary>
    /// <param name="worldPos">The world position to set</param>
    public void SetRoomWorldPos(Vector3 worldPos)
    {
        roomWorldPos = worldPos;

    } // end SetRoomWorldPos

    /// <summary>
    /// Gets the current number of used markers on this room - for calculating difficulty of an action
    /// </summary>
    /// <returns>Returns the number of used markers associated with this room</returns>
    public int GetNumUsedMarkers()
    {
        return numUsedMarkers;

    } // end GetNumUsedMarkers


    /// <summary>
    /// Increases the number of used markers in this room by the amount given - when certain tasks are done in this room
    /// </summary>
    /// <param name="numMarkers">the amount to increase the used markers by</param>
    public void AddUsedMarkers(int numMarkers)
    {
        numUsedMarkers += numMarkers;

    } // end AddUsedMarkers

    /// <summary>
    /// Clears the number of used markers for this room - at the end of a round
    /// </summary>
    public void ClearUsedMarkers() 
    { 
        numUsedMarkers = 0;

    } // end ClearUsedMarkers

    /// <summary>
    /// Returns if this module is broken (system not fully implemented yet)
    /// </summary>
    /// <returns>true if the module is broken, false if not</returns>
    public bool IsBroken() 
    { 
        return broken;

    } // IsBroken

    /// <summary>
    /// Repairs the module
    /// </summary>
    public void BreakModule()
    {
        broken = true;

    } // end BreakModule

    /// <summary>
    /// Repairs the module
    /// </summary>
    public void RepairModule()
    {
        broken = false;

    } // end RepairModule

    /// <summary>
    /// Sets up the boolean array that lets bots know if terminals are occupied
    /// </summary>
    public void InitOccupied()
    {
        occupiedTerminals = new bool[terminalLocations.Count];

    } // end InitOccupied

    /// <summary>
    /// Goes through the terminal spots and grabs the first open one, -1 if all occupied
    /// </summary>
    /// <returns>The first open terminal id or -1 if none available</returns>
    public int GetUnoccupiedTerminal()
    {
        // go through the terminals and get the first unoccupied one
        for (int i = 0; i < terminalLocations.Count; i++)
        {
            if (!occupiedTerminals[i])
            {
                return i;
            }
        }

        return -1;

    } // end GetUnoccupiedTerminal

    /// <summary>
    /// Checks to see if this module has all terminals fully in use
    /// </summary>
    /// <returns>true if all terminals full, false otherwise</returns>
    public bool IsFullyOccupied()
    {
        return (GetUnoccupiedTerminal() == -1);

    } // end IsFullyOccupied

    /// <summary>
    /// Checks to see if a specific terminal ID is in use
    /// </summary>
    /// <param name="terminalID">The terminal ID to check</param>
    /// <returns>true if a bot is using the spot, false otherwise (or if the id is invalid)</returns>
    public bool IsTerminalOccupied(int terminalID)
    {
        if (terminalID < terminalLocations.Count)
        {
            return (occupiedTerminals[terminalID]);
        }

        return false;

    } // end IsTerminalOccupied

    /// <summary>
    /// Sets the terminal specified as occupied - does nothing if the id was invalid
    /// <param name="terminalID">The terminal ID to set</param>
    /// </summary>
    public void SetTerminalOccupied(int terminalID)
    {
        if (terminalID < terminalLocations.Count)
        {
            occupiedTerminals[terminalID] = true;
        }

    } // end SetTerminalOccupied

}
