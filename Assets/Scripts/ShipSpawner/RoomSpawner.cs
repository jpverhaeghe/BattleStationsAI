using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomData;

public class RoomSpawner : MonoBehaviour
{
    public enum WallOrientation
    {
        North,
        East,
        South,
        West
    }

    // Constant values for the ship room sizes
    public static int ROOM_HEIGHT = 5;                                  // the height in meters of the room 
    public static int ROOM_WIDTH = 5;                                   // the width in meters of the rooms
    private static int TILE_HEIGHT = 1;                                 // the height in meters of a tile 
    private static int TILE_WIDTH = 1;                                  // the width in meters of a tile
    private static float EAST_WALL_OFFSET = 1f;                         // The offset for the east wall when it is at the right or bottom of the tile
    private static float SOUTH_WALL_OFFSET = 0.9f;                      // The offset for the south wall when it is at the right or bottom of the tile
    private static float WEST_WALL_OFFSET = 0.1f;                       // The offset for the west wall when it is at the right or bottom of the tile

    // Serialized fields for this objec that show up in the editor
    [Header("Tile prefabs and data for creating rooms")]
    [SerializeField] List<GameObject> tileObjects;                      // the tiles that will be used to build the rooms
    [SerializeField] GameObject floorPrefab;                            // the floor tile prefab
    [SerializeField] GameObject wallPrefab;                             // the wall prefab to put up room boundaries
    [SerializeField] Material[] tileMaterials;                              // room type tileMaterials to apply to the room tiles based on the room type
    [SerializeField] Texture floorTileTexture;                          // the texture to apply to floor tiles
    [SerializeField] Texture starTileTexture;                           // the texture to apply to star tiles


    /// <summary>
    /// Constructs of the room at the given location
    /// </summary>
    /// <param name="parentShipObject">The parent object to attach the room to</param>
    /// <param name="room">The array that has the tile information for this room</param>
    /// <param name="roomPos_x">the world position tilePos_x for the ship</param>
    /// <param name="roomPos_z">the world position tilePos_z for the ship</param>
    public void BuildRoom(GameObject parentShipObject, RoomInfo room, float roomPos_x, float roomPos_z)
    {
        // go through each row of this room
        for (int roomRow = 0; roomRow < room.roomTiles.GetLength(0); roomRow++)
        {
            // and in each row, go through each column
            for (int roomCol = 0; roomCol < room.roomTiles.GetLength(1); roomCol++)
            {
                // check to see if we are on an edge and create a wall if need be
                // Get the room tile type at this position in the array
                RoomTiles tileType = room.roomTiles[roomRow, roomCol];

                // set up the positions for the tiles - subtract as walls don't actually take a tile but are listed in the array so we can have doors
                // (walls and empty fix issues with positions as they are always on the edges)
                float currentTilePos_z = roomPos_z - ( (roomRow - 1) * TILE_HEIGHT);
                float currentTilePos_x = roomPos_x + ( (roomCol - 1) * TILE_WIDTH);

                switch (tileType)
                {
                    // floor tiles can be normal floor or stars - floors use different textures
                    case RoomTiles.Floor:
                        GameObject floorTile = InstantiateTile(parentShipObject, room.roomType, floorPrefab, currentTilePos_x, currentTilePos_z);

                        // get the child object (the geometry) and set the texture for this tile
                        Renderer floorTileRenderer = floorTile.GetComponentInChildren<Renderer>();
                        floorTileRenderer.material.mainTexture = floorTileTexture;
                        break;

                    // floor tiles can be normal floor or stars - stars use different textures
                    case RoomTiles.Star:
                        // Instantiated both the tile and the star but the star on y value higher
                        GameObject starTile = InstantiateTile(parentShipObject, room.roomType, floorPrefab, currentTilePos_x, currentTilePos_z);

                        // get the child object (the geometry) and set the texture for this tile
                        Renderer starTileRenderer = starTile.GetComponentInChildren<Renderer>();
                        starTileRenderer.material.mainTexture = starTileTexture;
                        break;

                    // floor tiles can be normal floor or stars
                    case RoomTiles.Area:
                        InstantiateTile(parentShipObject, room.roomType, tileObjects[0], currentTilePos_x, currentTilePos_z);
                        break;

                    // floor tiles can be normal floor or stars
                    case RoomTiles.Wall:
                        WallOrientation wallOrientation;

                        // walls only appear on the edges
                        if (roomRow == 0)
                        {
                            wallOrientation = WallOrientation.North;
                        }
                        else if (roomCol == 0)
                        {
                            wallOrientation = WallOrientation.West;
                        }
                        else if (roomRow == room.roomTiles.GetLength(0) - 1)
                        {
                            wallOrientation = WallOrientation.South;
                        }
                        else
                        {
                            wallOrientation = WallOrientation.East;
                        }

                        InstantiateWall(parentShipObject, wallOrientation, currentTilePos_x, currentTilePos_z);
                        break;

                    // default is to do nothing (RoomTiles.Empty areas)
                    default:
                        break;
                }
            }
        }

    } // end BuildRoom

    /// <summary>
    /// Instantiates a tile object with the correct room color based on room type
    /// </summary>
    /// <param name="parentObject">the object to link this tile to</param>
    /// <param name="roomType">the type of room that is being created</param>
    /// <param name="tile">the tile object to spawn</param>
    /// <param name="tilePos_x">the tilePos_x position in the world to spawn this tile</param>
    /// <param name="tilePos_z">the tilePos_z position in the world to spawn this tile</param>
    /// <returns></returns>
    private GameObject InstantiateTile(GameObject parentObject, RoomType roomType, GameObject tile, float tilePos_x, float tilePos_z)
    {
        // instantiates the gmae object and saves it as a gameObeject
        GameObject tileObject = Instantiate(tile, new Vector3(tilePos_x, 0, tilePos_z), Quaternion.identity);

        // Sets the spawned gameObject to be a child of the empty gameObject
        tileObject.transform.SetParent(parentObject.transform);

        // make it so it sets up the correct tileMaterials based on room type
        Renderer tileRenderer = tileObject.GetComponentInChildren<Renderer>();
        tileRenderer.material = tileMaterials[(int)roomType];

        return tileObject;

    } // end InstantiateTile

    /// <summary>
    /// Instantiates a wall at the correct position, taking into account that walls are part of a tile at the edges of the room
    /// </summary>
    /// <param name="parentObject">the object to link this wall to</param>
    /// <param name="wallDir">the direction of the wall to adjust position and rotation</param>
    /// <param name="x">the tilePos_x position - adjusted here as array edges are not part of the room</param>
    /// <param name="z">the tilePos_z position - adjusted here as array edges are not part of the room</param>
    private void InstantiateWall(GameObject parentObject, WallOrientation wallDir, float x, float z)
    {
        // set up the position and orientations for this wall object
        Vector3 wallPos;
        Quaternion wallRot = Quaternion.identity;

        switch (wallDir)
        {
            // if on the east side, need to rotate 90 and move to the right by the offset and adjust for col position
            case WallOrientation.East:
                wallPos = new Vector3(x - TILE_WIDTH + EAST_WALL_OFFSET, 0, z);
                wallRot = Quaternion.Euler(0, 90, 0);
                break;

            // if on the south side, need to rotate 90 and move to the down by the offset and adjust for row position
            case WallOrientation.South:
                wallPos = new Vector3(x, 0, z + TILE_HEIGHT - SOUTH_WALL_OFFSET);
                break;

            // if the wall is on the west side then just rotate 90 and adjust for col position
            case WallOrientation.West:
                wallPos = new Vector3(x + TILE_WIDTH + WEST_WALL_OFFSET, 0, z);
                wallRot = Quaternion.Euler(0, 90, 0);
                break;

            // the default is the north where we just need to set the position and adjust for row position
            default:
                wallPos = new Vector3(x, 0, z - TILE_HEIGHT);
                break;
        }

        // instantiate the wall at the given position
        GameObject newObject = Instantiate(wallPrefab, wallPos, wallRot);

        // Add the wall to the parent object so we can move it easier later
        newObject.transform.SetParent(parentObject.transform);

    } // end InstantiateWall

}