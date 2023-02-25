using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomData;

public class RoomInfo
{
    // This classes public variables used by other classes
    // TODO: Make these private and use getters/setters (probably just need getters)
    public string roomName;
    public RoomType roomType;
    public RoomFacing roomFacing;
    public ModuleType moduleType;
    public RoomTiles[,] roomTiles;
    public bool externalFacing;

    /// <summary>
    /// the constructor for this class
    /// </summary>
    /// <param name="roomName">The name of the room type</param>
    /// <param name="roomType">The room type</param>
    /// <param name="roomTiles">The room tile layout</param>
    public RoomInfo(string roomName, RoomType roomType, RoomFacing roomfacing, ModuleType moduleType, RoomTiles[,] roomTiles, bool externalFacing)
    {
        this.roomName = roomName;
        this.roomType = roomType;
        this.roomFacing = roomfacing;
        this.moduleType = moduleType;
        this.roomTiles = roomTiles;
        this.externalFacing = externalFacing;

    } // end RoomInfo constructor

}
