using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomData;
using static UnityEngine.Rendering.DebugUI.Table;

public class ShipManager : MonoBehaviour
{
    // NOTE: Size of ships is num modules divided by 3

    // Creates a layout for Redundant II (Hunan Scout) - Size 3 (9/3)
    public static RoomInfo[,] HumanScout = {
        {null,                  CannonUpInfo,           null},
        {null,                  MissileBayRightInfo,    null},
        {HelmUpInfo,            TeleporterUpInfo,       HyperdriveRightInfo},
        {LifeSupportInfo,       null,                   ScienceRightInfo},
        {EngineUpInfo,          null,                   EngineUpInfo},
    };

    // Creates layout for Valiant (Human Frigate) - Size 4 (12/3)
    public static RoomInfo[,] HumanFrigate = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  null,                   LifeSupportInfo,        null,                   null},
        {null,                  ScienceLeftInfo,        TeleporterUpInfo,       MissileBayRightInfo,    null},
        {null,                  HyperdriveLeftInfo,     SickBayUpInfo,          LifeSupportInfo,        null},
        {CannonLeftInfo,        EngineUpInfo,           null,                   EngineUpInfo,           CannonRightInfo},
    };

    // Creates the Fearlight - Size 3 (9/3)
    public static RoomInfo[,] Fearlight = {
        {null,                  HelmUpInfo,             null},
        {ScienceDownInfo,       TeleporterUpInfo,       CannonUpInfo},
        {HyperdriveDownInfo,    null,                   MissileBayRightInfo},
        {EngineUpInfo,          EngineUpInfo,           LifeSupportInfo},
    };

    // Creates the Claw - Size 3 (9/3)
    public static RoomInfo[,] TentacScout = {
        {null,                  ScienceUpInfo,          null},
        {EngineRightInfo,       HyperdriveLeftInfo,     CannonUpInfo},
        {EngineRightInfo,       null,                   MissileBayRightInfo},
        {EngineRightInfo,       LifeSupportInfo,        HelmDownInfo},
    };

    // Create Xeloxian scout - Size 3 (9/3)
    public static RoomInfo[,] XeloxianScout = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  null,                   LifeSupportInfo,        null,                   null},
        {CannonLeftInfo,        EngineUpInfo,           TeleporterUpInfo,       EngineUpInfo,           EngineUpInfo},
        {null,                  null,                   HyperdriveRightInfo,    null,                   null},
        {null,                  null,                   ScienceUpInfo,          null,                   null},
    };

    // creates silicoid scout - Size 3 (10/3)
    public static RoomInfo[,] SilicoidScout = {
        {null,                  LifeSupportInfo,        null,                   HyperdriveRightInfo,    null},
        {null,                  MissileBayLeftInfo,     HelmUpInfo,             ScienceUpInfo,          null},
        {CannonLeftInfo,        EngineUpInfo,           null,                   EngineUpInfo,           EngineUpInfo},
    };

    // Creates a canosian scout - Size 3 (9/3)
    public static RoomInfo[,] CanosianScout = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  MissileBayUpInfo,       LifeSupportInfo,        ScienceLeftInfo,        null},
        {CannonLeftInfo,        EngineUpInfo,           EngineUpInfo,           HyperdriveLeftInfo,     CargoBayInfo},
    };

    // Creates starbase layout - Size 3 (9/3)
    public static RoomInfo[,] Starbase = {
        {CannonUpInfo,          CargoBayInfo,           MissileBayRightInfo},
        {LifeSupportInfo,       SickBayUpInfo,          ScienceUpInfo},
        {EngineUpInfo,          EngineUpInfo,           EngineUpInfo},
    };

    // Creates the starbas layout facing to the right (probably can remove) - Size 3 (9/3)
    public static RoomInfo[,] StarbaseR = {
        {EngineRightInfo,       LifeSupportInfo,        CannonRightInfo},
        {EngineRightInfo,       SickBayDownInfo,        CargoBayInfo},
        {EngineRightInfo,       ScienceRightInfo,       MissileBayDownInfo},
    };

    //Is a list of the different ship options
    private List<RoomInfo[,]> shipList = new List<RoomInfo[,]>
    {
        HumanScout, HumanFrigate, Fearlight, TentacScout, XeloxianScout, SilicoidScout, CanosianScout, Starbase, StarbaseR, 
    };
    
    private RoomSpawner roomSpawner;                // A refrence to the class roomSpawner    
    private List<GameObject> shipObjects;           // An empty gameObejct that stores all the spawned gameObjects to keep things clean and easy to find
    private int currentShipID = 0;                  // The current ship id

    /// <summary>
    /// Initializes variables for this ship manager
    /// </summary>
    private void Start()
    {
        // initialize the list of ships for accessing later
        shipObjects = new List<GameObject>();

        // assigns the two RoomSpawner script for later use
        roomSpawner = gameObject.GetComponent<RoomSpawner>();

    } // end Start


    /// <summary>
    /// Creates a ship at the origin of the scene
    /// </summary>
    public void CreateShip()
    {
        // for now remvoe the old ship (if there is one) as this method should only be called from the generate ship button
        if (currentShipID > 0)
        {
            GameObject currentShip = shipObjects[--currentShipID];
            Destroy(currentShip);
            shipObjects.RemoveAt(currentShipID);
        }

        CreateShip(0, 0);

    } // end Create ship at origine

    /// <summary>
    /// This method starts the instanition of a ship based
    /// </summary>
    /// <param name="xPos">Where the ship should be in the world on the X axis</param>
    /// <param name="zPos">Where the ship should be in the world on the Z axis</param>
    // TODO: Look at making rooms child objects to see if we can use relative positioning? Then may not need to send xPos and zPos down
    public void CreateShip(float xPos, float zPos)
    {
        // create a ship in the list to store the data in for later
        GameObject shipObject = new GameObject("Ship" + currentShipID);
        shipObject.transform.position = new Vector3(xPos, 0, zPos);

        shipObjects.Add(shipObject);

        // for now, choose a random ship for testing
        int shipType = Random.Range(0, shipList.Count);

        // build a ship
        BuildShip(shipList[shipType], xPos, zPos);

        // increase the number of ships instantiated
        currentShipID++;

    } // end CreateShip

    public void BuildShip(RoomInfo[,] ship, float worldPos_x, float worldPos_z)
    {
        // build the ship rooms based on the room layout
        for (int roomRow = 0; roomRow < ship.GetLength(0); roomRow++) 
        {
            for (int roomCol = 0; roomCol < ship.GetLength(1); roomCol++)
            {
                // gets the room from the ship array
                RoomInfo room = ship[roomRow, roomCol];

                if (room != null)
                {
                    // calculate the world position for this room to send down

                    // as rooms can be offset from other rooms based on location in the ship, set the roomPos_z to the current row times the height of a room
                    float roomPos_z = worldPos_z - (roomRow * RoomSpawner.ROOM_HEIGHT);

                    // as rooms can be offset from other rooms based on location in the ship, set the roomPos_x to the current col times the width of a room
                    float roomPos_x = worldPos_x + (roomCol * RoomSpawner.ROOM_WIDTH);

                    // instantiates the room objects based on the strings in the arrays 
                    roomSpawner.BuildRoom(shipObjects[currentShipID], room, roomPos_x, roomPos_z);
                }
            }
        }
    }

}
