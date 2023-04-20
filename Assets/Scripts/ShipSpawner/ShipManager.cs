using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomData;

public class ShipManager : MonoBehaviour
{
    // TODO: Probably need to make the ship a seperate class that is managed by this class.

    // private constant variables
    private const float HELM_CENTER_OFFSET = 2.5f;

    // public variables used by the ship manager for keeping track of the current ships state
    public int shipSize;                                                // used to store the current generated ship size
    public int currentSpeed;                                            // the current speed of the ship
    public int outOfControlLevel;                                       // how out of control the ship is, affects bot skills
    public int helmEnergyLevel;                                         // the energy amount in the ships drive systems (turning, etc.)
    public int weaponEnergyLevel;                                       // the energy amount in the weapons systems
    public int shieldEnergyLevel;                                       // the energy amount in the shields systems

    // Serialized fields used by this script
    [Header("Ship Generation Elements")]
    [SerializeField] ShipLayoutGenerator shipLayoutGeneratorScript;     // a link to the ship layout generator to call when generate ship is pressed
    [SerializeField] TMP_Dropdown prebuiltShipList;                     // a list of the prebuilt ships

    [Header("HUD Elements to keep track of ship data")]
    [SerializeField] TMP_Text shipSpeedText;
    [SerializeField] TMP_Text shipOutOfControlText;
    [SerializeField] TMP_Text shipHelmText;
    [SerializeField] TMP_Text shipWeaponsText;
    [SerializeField] TMP_Text shipShieldText;

    // private variables used by this script
    private RoomSpawner roomSpawner;                                    // A refrence to the class roomSpawner    
    private GameObject shipObject;                                      // Eempty gameObject that stores all the spawned gameObjects to keep things easy to find
    private Vector3 shipHelmPos;                                        // holds the helm player position for walking through the ship

    
    //Is a list of the different ship options
    private List<RoomInfo[,]> shipList = new List<RoomInfo[,]>
    {
        RedundantII, Valiant, Fearlight, TentacScout, XeloxianScout, SilicoidScout, CanosianScout, Starbase,
    };

    /// <summary>
    /// Initializes variables for this ship manager
    /// </summary>
    private void Awake()
    {
        // assigns the RoomSpawner script for later use
        roomSpawner = gameObject.GetComponent<RoomSpawner>();

    } // end Awake

    /// <summary>
    /// Initialiazes the ship and the rounds
    /// </summary>
    public void InitializeShip()
    {
        GenerateShip();

    } // end InitializeShip

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

        shipSpeedText.text = "Speed = " + currentSpeed.ToString();

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

        shipOutOfControlText.text = "OOC = " + outOfControlLevel.ToString();

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

        shipHelmText.text = "Speed = " + helmEnergyLevel.ToString();

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

        shipWeaponsText.text = "Weapons = " + weaponEnergyLevel.ToString();

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

        shipShieldText.text = "Shields = " + shieldEnergyLevel.ToString();

    } // end UpdateShieldEnergy

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
