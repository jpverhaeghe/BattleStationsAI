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
    public RoomTiles[,] roomTiles;

    /// <summary>
    /// the constructor for this class
    /// </summary>
    /// <param name="roomName">The name of the room type</param>
    /// <param name="roomType">The room type</param>
    /// <param name="roomTiles">The room tile layout</param>
    public RoomInfo(string roomName, RoomType roomType, RoomTiles[,] roomTiles)
    {
        this.roomName = roomName;
        this.roomType = roomType;
        this.roomTiles = roomTiles;

    } // end RoomInfo constructor

}
