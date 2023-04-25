using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomData;

public class RoomSpawner : MonoBehaviour
{
    // Constant values for the ship room sizes
    public static int ROOM_HEIGHT = 7;                                  // the height in meters of the room 
    public static int ROOM_WIDTH = 7;                                   // the width in meters of the rooms
    private static int TILE_HEIGHT = 1;                                 // the height in meters of a tile 
    private static int TILE_WIDTH = 1;                                  // the width in meters of a tile

    // Serialized fields for this objec that show up in the editor
    [Header("Tile prefabs and data for creating rooms")]
    [SerializeField] List<GameObject> tileObjects;                      // the tiles that will be used to build the rooms
    [SerializeField] GameObject floorPrefab;                            // the floor tile prefab
    [SerializeField] GameObject wallPrefab;                             // the wall prefab to put up room boundaries
    [SerializeField] Material[] floorMaterials;                         // room type tileMaterials to apply to the room tiles based on the room type
    [SerializeField] Material[] starMaterials;                          // room type tileMaterials to apply to the room tiles based on the room type
    [SerializeField] Texture floorTileTexture;                          // the texture to apply to floor tiles
    [SerializeField] Texture starTileTexture;                           // the texture to apply to star tiles

    /// <summary>
    /// Constructs of the room at the given location
    /// </summary>
    /// <param name="parentShipObject">The parent object to attach the room to</param>
    /// <param name="room">The array that has the tile information for this room</param>
    /// <param name="roomRow">the current room row, used to set walled areas in pathing system</param>
    /// <param name="roomCol">the current room column, used to set walled areas in pathing system</param>
    /// <param name="roomPos_x">the world position roomPos_x for the ship</param>
    /// <param name="roomPos_z">the world position roomPos_z for the ship</param>
    public void BuildRoom(GameObject parentShipObject, RoomInfo room, int roomRow, int roomCol, float roomPos_x, float roomPos_z)
    {
        // create an empty room object in the hiearchy to store the room data in an organized way
        GameObject roomObject = new GameObject(room.roomName);
        roomObject.transform.position = new Vector3(roomPos_x, 0, roomPos_z);

        // Sets the spawned gameObject to be a child of the empty gameObject
        roomObject.transform.SetParent(parentShipObject.transform);
        GeneratedShip currentGeneratedShip = parentShipObject.GetComponent<GeneratedShip>();

        // set the room grid position so it can be used later for bot placement
        int shipOffsetGridRow = (roomRow * ROOM_HEIGHT);
        int shipOffsetGridCol = (roomCol * ROOM_WIDTH);
        room.roomGridPos = new Vector2Int(shipOffsetGridRow, shipOffsetGridCol);

        // go through each row of this room
        for (int tileRow = 0; tileRow < room.roomTiles.GetLength(0); tileRow++)
        {
            // and in each row, go through each column
            for (int tileCol = 0; tileCol < room.roomTiles.GetLength(1); tileCol++)
            {
                // check to see if we are on an edge and create a wall if need be
                // Get the room tile type at this position in the array
                RoomTiles tileType = room.roomTiles[tileRow, tileCol];

                // set up the positions for the tiles
                float currentTilePos_z = roomPos_z - (tileRow  * TILE_HEIGHT);
                float currentTilePos_x = roomPos_x + (tileCol * TILE_WIDTH);

                // the current position of this tile in the walkable area graph
                int currentShipTileRow = shipOffsetGridRow + (tileRow);
                int currentShipTileCol = shipOffsetGridCol + (tileCol);

                switch (tileType)
                {
                    // floor tiles can be normal floor or stars - floors use different textures
                    case RoomTiles.Floor:
                        GameObject floorTile = InstantiateTile(roomObject, room.roomType, floorPrefab, currentTilePos_x, currentTilePos_z, false);

                        // get the child object (the geometry) and set the texture for this tile
                        Renderer floorTileRenderer = floorTile.GetComponentInChildren<Renderer>();
                        floorTileRenderer.material.mainTexture = floorTileTexture;
                        break;

                    // floor tiles can be normal floor or stars - stars use different textures
                    // TODO: Need to figure out how to save this data in the RommInfo (store it in the table so we don't have to calculat it?)
                    case RoomTiles.Star:
                        // Instantiated both the tile and the star but the star on y value higher
                        GameObject starTile = InstantiateTile(roomObject, room.roomType, floorPrefab, currentTilePos_x, currentTilePos_z, true);

                        // get the child object (the geometry) and set the texture for this tile
                        Renderer starTileRenderer = starTile.GetComponentInChildren<Renderer>();
                        starTileRenderer.material.SetTexture("_mainTexture", starTileTexture);

                        // store the star tile location in the roomInfo structure
                        room.AddTerminalLocation(new Vector2Int(currentShipTileRow, currentShipTileCol), starTileRenderer);
                        break;

                    // Non walkable area
                    // TODO: Add shaders here to make these more like the tiles
                    case RoomTiles.Area:
                        InstantiateTile(roomObject, room.roomType, tileObjects[0], currentTilePos_x, currentTilePos_z, false);

                        // for placement of the walkable areas in this ship walls are actually empty cells
                        currentGeneratedShip.shipPathingSystem.SetWall(new Vector2Int(currentShipTileRow, currentShipTileCol));
                        break;

                    // A wall, so not walkable
                    case RoomTiles.Wall:
                        InstantiateWall(roomObject, currentTilePos_x, currentTilePos_z);

                        // for placement of the walkable areas in this ship walls should not be walkable
                        currentGeneratedShip.shipPathingSystem.SetWall(new Vector2Int(currentShipTileRow, currentShipTileCol));
                        break;

                    // default is to do nothing (RoomTiles.Empty areas)
                    default:
                        // for placement of the walkable areas in this ship walls are actually empty cells
                        //currentGeneratedShip.shipPathingSystem.SetWall(new Vector2Int(currentShipTileRow, currentShipTileCol));
                        break;
                }
            }
        }

        // once all tiles are placed, we know how many terminals there are so create the array showing them as unoccupied
        room.InitOccupied();

    } // end BuildRoom

    /// <summary>
    /// Instantiates a tile object with the correct room color based on room type
    /// </summary>
    /// <param name="parentObject">the object to link this tile to</param>
    /// <param name="roomType">the type of room that is being created</param>
    /// <param name="tile">the tile object to spawn</param>
    /// <param name="tilePos_x">the tilePos_x position in the world to spawn this tile</param>
    /// <param name="tilePos_z">the tilePos_z position in the world to spawn this tile</param>
    /// <param name="isStarTile">if the tile is a star tile, then it uses a different material</param>
    /// TODO: Could this just be done with changing the shader on this material?
    /// <returns></returns>
    private GameObject InstantiateTile(GameObject parentObject, RoomType roomType, GameObject tile, float tilePos_x, float tilePos_z, bool isStarTile)
    {
        // instantiates the gmae object and saves it as a gameObeject
        GameObject tileObject = Instantiate(tile, new Vector3(tilePos_x, 0, tilePos_z), Quaternion.identity);

        // Sets the spawned gameObject to be a child of the empty gameObject
        tileObject.transform.SetParent(parentObject.transform);

        // make it so it sets up the correct tileMaterials based on room type
        Renderer tileRenderer = tileObject.GetComponentInChildren<Renderer>();

        if (isStarTile)
        {
            tileRenderer.material = starMaterials[(int)roomType];
        }
        else
        {
            tileRenderer.material = floorMaterials[(int)roomType];
        }

        return tileObject;

    } // end InstantiateTile

    /// <summary>
    /// Instantiates a wall at the correct position, taking into account that walls are part of a tile at the edges of the room
    /// </summary>
    /// <param name="parentObject">the object to link this wall to</param>
    /// <param name="x">the tilePos_x position - adjusted here as array edges are not part of the room</param>
    /// <param name="z">the tilePos_z position - adjusted here as array edges are not part of the room</param>
    private void InstantiateWall(GameObject parentObject, float x, float z)
    {
        // instantiate the wall at the given position
        GameObject newObject = Instantiate(wallPrefab, new Vector3(x, 0, z), Quaternion.identity);

        // Add the wall to the parent object so we can move it easier later
        newObject.transform.SetParent(parentObject.transform);

    } // end InstantiateWall

}