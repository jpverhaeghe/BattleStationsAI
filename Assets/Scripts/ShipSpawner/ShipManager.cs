using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomData;

public class ShipManager : MonoBehaviour
{
    // Constant values used by this script - these are some of the default ships for comparisson
    // Creates a layout for Redundant II (Hunan Scout) - Size 3 (9/3)
    private static RoomInfo[,] RedundantII = {
        {null,                  CannonUpInfo,           null},
        {null,                  MissileBayRightInfo,    null},
        {HelmUpInfo,            TeleporterUpInfo,       HyperdriveRightInfo},
        {LifeSupportInfo,       null,                   ScienceRightInfo},
        {EngineUpInfo,          null,                   EngineUpInfo},
    };

    // Creates layout for Valiant (Human Frigate) - Size 4 (12/3)
    private static RoomInfo[,] Valiant = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  null,                   LifeSupportInfo,        null,                   null},
        {null,                  ScienceLeftInfo,        TeleporterUpInfo,       MissileBayRightInfo,    null},
        {null,                  HyperdriveLeftInfo,     SickBayUpInfo,          LifeSupportInfo,        null},
        {CannonLeftInfo,        EngineUpInfo,           null,                   EngineUpInfo,           CannonRightInfo},
    };

    // Creates the Fearlight (Tentac Scout) - Size 3 (9/3)
    private static RoomInfo[,] Fearlight = {
        {null,                  HelmUpInfo,             null},
        {ScienceDownInfo,       TeleporterUpInfo,       CannonUpInfo},
        {HyperdriveDownInfo,    null,                   MissileBayRightInfo},
        {EngineUpInfo,          EngineUpInfo,           LifeSupportInfo},
    };

    // Creates the Claw - Size 3 (9/3)
    private static RoomInfo[,] TentacScout = {
        {null,                  ScienceUpInfo,          null},
        {EngineRightInfo,       HyperdriveLeftInfo,     CannonUpInfo},
        {EngineRightInfo,       null,                   MissileBayRightInfo},
        {EngineRightInfo,       LifeSupportInfo,        HelmDownInfo},
    };

    // Create Xeloxian scout - Size 3 (9/3)
    private static RoomInfo[,] XeloxianScout = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  null,                   LifeSupportInfo,        null,                   null},
        {CannonLeftInfo,        EngineUpInfo,           TeleporterUpInfo,       EngineUpInfo,           EngineUpInfo},
        {null,                  null,                   HyperdriveRightInfo,    null,                   null},
        {null,                  null,                   ScienceUpInfo,          null,                   null},
    };

    // Creates silicoid scout - Size 3 (10/3)
    private static RoomInfo[,] SilicoidScout = {
        {null,                  LifeSupportInfo,        null,                   HyperdriveRightInfo,    null},
        {null,                  MissileBayLeftInfo,     HelmUpInfo,             ScienceUpInfo,          null},
        {CannonLeftInfo,        EngineUpInfo,           null,                   EngineUpInfo,           EngineUpInfo},
    };

    // Creates a canosian scout - Size 3 (9/3)
    private static RoomInfo[,] CanosianScout = {
        {null,                  null,                   HelmUpInfo,             null,                   null},
        {null,                  MissileBayUpInfo,       LifeSupportInfo,        ScienceLeftInfo,        null},
        {CannonLeftInfo,        EngineUpInfo,           EngineUpInfo,           HyperdriveLeftInfo,     CargoBayInfo},
    };

    // Creates starbase layout - Size 3 (9/3)
    private static RoomInfo[,] Starbase = {
        {CannonUpInfo,          CargoBayInfo,           MissileBayRightInfo},
        {LifeSupportInfo,       SickBayUpInfo,          ScienceUpInfo},
        {EngineUpInfo,          EngineUpInfo,           EngineUpInfo},
    };

    // Creates the starbas layout facing to the right (probably can remove) - Size 3 (9/3)
    private static RoomInfo[,] StarbaseR = {
        {EngineRightInfo,       LifeSupportInfo,        CannonRightInfo},
        {EngineRightInfo,       SickBayDownInfo,        CargoBayInfo},
        {EngineRightInfo,       ScienceRightInfo,       MissileBayDownInfo},
    };

    //Is a list of the different ship options
    private List<RoomInfo[,]> shipList = new List<RoomInfo[,]>
    {
        RedundantII, Valiant, Fearlight, TentacScout, XeloxianScout, SilicoidScout, CanosianScout, Starbase, 
    };

    // private constant variables
    private const float HELM_CENTER_OFFSET = 2.5f;

    // Serialized fields used by this script
    //[SerializeField] Camera shipCamera;                                 // may be used to move the camera around
    [SerializeField] ShipLayoutGenerator shipLayoutGeneratorScript;     // a link to the ship layout generator to call when generate ship is pressed
    [SerializeField] TMP_Dropdown prebuiltShipList;                     // a list of the prebuilt ships

    // private variables used by this script
    private RoomSpawner roomSpawner;                                    // A refrence to the class roomSpawner    
    private GameObject shipObject;                                     // An empty gameObject that stores all the spawned gameObjects to keep things clean and easy to find
    public Vector3 shipHelmPos;                                         // holds the helm player position for walking through the ship

    /// <summary>
    /// Initializes variables for this ship manager
    /// </summary>
    private void Start()
    {
        // initialize the list of ships for accessing later
        shipObject = null;

        // assigns the two RoomSpawner script for later use
        roomSpawner = gameObject.GetComponent<RoomSpawner>();

    } // end Start

    /// <summary>
    /// Generate a random ship at the origin of the scene
    /// </summary>
    public void GenerateShip()
    {
        // for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
        ClearShip();

        // Get the room info layout and create the ship
        RoomInfo[,] ship = shipLayoutGeneratorScript.GenerateShipLayout();
        CreateShip(ship, 0, 0);

    } // end GenerateShip

    /// <summary>
    /// Creates a ship from the pre-built list at the origin of the scene
    /// </summary>
    public void CreateShip()
    {
        // for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
        ClearShip();

        // choose one from the drop down
        int shipType = prebuiltShipList.value;              //Random.Range(0, shipList.Count);
        CreateShip(shipList[shipType], 0, 0);

    } // end Create ship at origine

    /// <summary>
    /// This method starts the instanition of a ship based
    /// </summary>
    /// <param name="xPos">Where the ship should be placed in the world on the X axis</param>
    /// <param name="zPos">Where the ship should be placed in the world on the Z axis</param>
    public void CreateShip(RoomInfo[,] ship, float xPos, float zPos)
    {
        // create a ship in the list to store the data in for later
        GameObject shipObject = new GameObject("Spawned Ship");
        shipObject.transform.position = new Vector3(xPos, 0, zPos);
        this.shipObject = shipObject;

        // build the ship
        BuildShip(ship, xPos, zPos);

    } // end CreateShip

    /// <summary>
    /// Returns the ship helm coordinates
    /// </summary>
    /// <returns></returns>
    public Vector3 GetShipHelmPos()
    {
        return shipHelmPos;

    } // end GetShipHelmPos

    /// <summary>
    /// Build the ship using the room information and positional values in game
    /// </summary>
    /// <param name="ship">The ship as a room information layout array</param>
    /// <param name="worldPos_x">The x world position for the top left of the ship</param>
    /// <param name="worldPos_z">The z world position for the top left of the ship</param>
    private void BuildShip(RoomInfo[,] ship, float worldPos_x, float worldPos_z)
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

                    // if the room is the helm, then set up the helm position
                    if (room.moduleType == ModuleType.Helm)
                    {
                        shipHelmPos = new Vector3(roomPos_x + HELM_CENTER_OFFSET, 0, roomPos_z - HELM_CENTER_OFFSET);
                    }

                    // instantiates the room objects based on the strings in the arrays 
                    roomSpawner.BuildRoom(shipObject, room, roomPos_x, roomPos_z);
                }
            }
        }

    } // end BuildShip

    /// <summary>
    /// for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
    /// </summary>
    private void ClearShip()
    {
        // for now remvoe the old ship (if there is one) as this method should only be called from the generate ship button
        if (shipObject != null)
        {
            Destroy(shipObject);
        }

        shipHelmPos = new Vector3();


    } // end ClearShip

}
