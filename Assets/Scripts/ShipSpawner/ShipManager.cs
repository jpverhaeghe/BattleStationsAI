using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static RoomData;

public class ShipManager : MonoBehaviour
{
    // private constant variables
    private const float HELM_CENTER_OFFSET = 2.5f;

    // public variables used by the ship manager for keeping track of the current ships state
    /*public int shipSize;                                                // used to store the current generated ship size
    public int currentSpeed;                                            // the current speed of the ship
    public int outOfControlLevel;                                       // how out of control the ship is, affects bot skills
    public int helmEnergyLevel;                                         // the energy amount in the ships drive systems (turning, etc.)
    public int weaponEnergyLevel;                                       // the energy amount in the weapons systems
    public int shieldEnergyLevel;                                       // the energy amount in the shields systems*/

    // Serialized fields used by this script
    [Header("Ship Generation Elements")]
    [SerializeField] ShipLayoutGenerator shipLayoutGeneratorScript;     // a link to the ship layout generator to call when generate ship is pressed
    [SerializeField] TMP_Dropdown prebuiltShipList;                     // a list of the prebuilt ships

    // TODO: Think of a way to re-use these for each ship as they go through the AI
    // - perhaps update a text box with a name of the ship above them
    [Header("HUD Elements to keep track of ship data")]
    [SerializeField] TMP_Text shipSpeedText;
    [SerializeField] TMP_Text shipOutOfControlText;
    [SerializeField] TMP_Text shipHelmText;
    [SerializeField] TMP_Text shipWeaponsText;
    [SerializeField] TMP_Text shipShieldText;

    // private variables used by this script
    private RoomSpawner roomSpawner;                                    // A refrence to the class roomSpawner    
    private List<GameObject> shipObjects;                               // GameObject list that stores all the spawned gameObjects to keep things easy to find
    private int currentSpawnShipSize;                                   // A temporary variable to store the ship size until the object is created


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

        // create the list of ship game objects
        shipObjects = new List<GameObject>();

    } // end Awake

    /// <summary>
    /// Initialiazes the ship and the rounds
    /// </summary>
    public void InitializeShip()
    {
        GenerateShip();

    } // end InitializeShip

    /// <summary>
    /// Does the intial set up for the round in phase one for all ships
    /// </summary>
    public void DoPhase1Setup()
    {
        for (int i = 0; i < shipObjects.Count; i++)
        {
            GeneratedShip currentShip = shipObjects[i].GetComponent<GeneratedShip>();

            // TODO: Energy levels can only go up if there is an engine working
            // all energy levels must be at least 1 at the beginning of the round
            if (currentShip.helmEnergyLevel < 1)
            {
                UpdateHelmEnergy(i, 1);
            }

            if (currentShip.weaponEnergyLevel < 1)
            {
                UpdateWeaponsEnergy(i, 1);
            }

            if (currentShip.shieldEnergyLevel < 1)
            {
                UpdateShieldEnergy(i, 1);
            }
        }

    } // DoPhase1Setup

    /// <summary>
    /// The stead the ships part of a phase removes one OOC from each ship, so just go through the list
    /// </summary>
    public void SteadyShips()
    {
        for (int i = 0; i < shipObjects.Count; i++)
        {
            UpdateOutOfControl(i, -1);
        }

    } // SteadyShips

    /// <summary>
    /// Performs the necessary adjustments for a ship at the end of the round
    /// </summary>
    public void ShipRoundCleanUp()
    {
        for (int i = 0; i < shipObjects.Count; i++)
        {
            // adjust the levels per the rules (all energy and speed are reduced by 1)
            UpdateSpeed(i, -1);
            UpdateHelmEnergy(i, -1);
            UpdateWeaponsEnergy(i, -1);
            UpdateShieldEnergy(i, -1);
        }

        // TODO: Remove counters on any modules (engineering, helm, weapons, etc.)

    } // ShipRoundCleanUp

    /// <summary>
    /// Updates the speed with to the new value, never less than zero
    /// TODO: When speed is over 4 - the ship should take damage
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="speedChange">The value to change the speed by</param>
    public void UpdateSpeed(int shipID, int speedChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateSpeed(speedChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipSpeedText.text = "Speed = " + shipObjects[shipID].GetComponent<GeneratedShip>().currentSpeed.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateShipSpeed: ShipID " + shipID + " does not exist");
        }

    } // end UpdateSpeed

    /// <summary>
    /// Updates the Out of Control factor (OOC) with to the new value, never less than zero
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="speedChange">The value to change the OOC by</param>
    public void UpdateOutOfControl(int shipID, int oocChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateOutOfControl(oocChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipOutOfControlText.text = "OOC = " + shipObjects[shipID].GetComponent<GeneratedShip>().outOfControlLevel.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateOutOfControl: ShipID " + shipID + " does not exist");
        }

    } // end UpdateOutOfControl

    /// <summary>
    /// Updates the helm energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="helmEnergyChange">The value to change the helm energy by</param>
    public void UpdateHelmEnergy(int shipID, int helmEnergyChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateHelmEnergy(helmEnergyChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {

                shipHelmText.text = "Helm = " + shipObjects[shipID].GetComponent<GeneratedShip>().helmEnergyLevel.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateHelmEnergy: ShipID " + shipID + " does not exist");
        }

    } // end UpdateHelmEnergy

    /// <summary>
    /// Updates the weapons energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="weaponsEnergyChange">The value to change the weapons energy by</param>
    public void UpdateWeaponsEnergy(int shipID, int weaponsEnergyChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateWeaponsEnergy(weaponsEnergyChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {

                shipWeaponsText.text = "Weapons = " + shipObjects[shipID].GetComponent<GeneratedShip>().weaponEnergyLevel.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateWeaponsEnergy: ShipID " + shipID + " does not exist");
        }

    } // end UpdateWeaponsEnergy

    /// <summary>
    /// Updates the shield energy level with to the new value, never less than zero
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="shieldEnergyChange">The value to </param>
    public void UpdateShieldEnergy(int shipID, int shieldEnergyChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateShieldEnergy(shieldEnergyChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {

                shipShieldText.text = "Shields = " + shipObjects[shipID].GetComponent<GeneratedShip>().shieldEnergyLevel.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateShieldEnergy: ShipID " + shipID + " does not exist");
        }

    } // end UpdateShieldEnergy

    /// <summary>
    /// Generate a random ship at the origin of the scene
    /// </summary>
    public void GenerateShip()
    {
        // for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
        for (int i = 0; i < shipObjects.Count; i++)
        {
            ClearShip(i);
        }

        // Get the room info layout and create the ship - for now only one ship is built
        RoomInfo[,] ship = shipLayoutGeneratorScript.GenerateShipLayout();
        CreateShip(ship, 0, 0);

    } // end GenerateShip

    /// <summary>
    /// Creates a ship from the pre-built list at the origin of the scene
    /// </summary>
    public void CreateShip()
    {
        // for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
        for (int i = 0; i < shipObjects.Count; i++)
        {
            ClearShip(i);
        }

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
        string shipObjectName = "EnemyShip";

        // if this is the first ship, give it a unique name
        if (shipObjects.Count == 0)
        {
            shipObjectName = "HeroShip";
        }

        GameObject shipObject = new GameObject(shipObjectName);
        shipObject.transform.position = new Vector3(xPos, 0, zPos);

        // add the ship component that keeps track of an individual ship status
        shipObject.AddComponent<GeneratedShip>();

        // as we add to the end, the id of the ship will be the current count
        int shipID = shipObjects.Count;
        shipObject.GetComponent<GeneratedShip>().shipID = shipID;

        // store the ship size too
        shipObject.GetComponent<GeneratedShip>().shipSize = currentSpawnShipSize;
        this.shipObjects.Add(shipObject);

        // build the ship
        BuildShip(shipID, ship, xPos, zPos);

    } // end CreateShip

    /// <summary>
    /// Returns the ship helm coordinates
    /// </summary>
    /// <param name="shipID">The id of the ship to access</param>
    /// <returns>The vector 3 position of where to place a player in the helm</returns>
    public Vector3 GetShipHelmPos(int shipID)
    {
        return shipObjects[shipID].GetComponent<GeneratedShip>().shipHelmPos;

    } // end GetShipHelmPos

    /// <summary>
    /// Sets the ship at the ID value in the list to the given size
    /// </summary>
    /// <param name="shipID"></param>
    /// <param name="size"></param>
    public void SetShipSize(int size)
    {
        currentSpawnShipSize = size;

    } // end SetShipSize

    /// <summary>
    /// Build the ship using the room information and positional values in game
    /// </summary>
    /// <param name="shipID">The id of the ship in the ship objects list</param>
    /// <param name="ship">The ship as a room information layout array</param>
    /// <param name="worldPos_x">The x world position for the top left of the ship</param>
    /// <param name="worldPos_z">The z world position for the top left of the ship</param>
    private void BuildShip(int shipID, RoomInfo[,] ship, float worldPos_x, float worldPos_z)
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
                        shipObjects[shipID].GetComponent<GeneratedShip>().shipHelmPos = 
                            new Vector3(roomPos_x + HELM_CENTER_OFFSET, 0, roomPos_z - HELM_CENTER_OFFSET);
                    }

                    // instantiates the room objects based on the strings in the arrays 
                    roomSpawner.BuildRoom(shipObjects[shipID], room, roomPos_x, roomPos_z);
                }
            }
        }

    } // end BuildShip

    /// <summary>
    /// for now remove the old ship (if there is one) as this method should only be called from the generate ship buttons
    /// </summary>
    private void ClearShip(int shipID)
    {
        GameObject shipToRemove = shipObjects[shipID];

        // for now remvoe the old ship (if there is one) as this method should only be called from the generate ship button
        if (shipToRemove != null)
        {
            // We can't remove the ship from the list as other ships rely on the placement in the list as an ID
            // TODO: This is not very efficient and could cause issues - find a way to clean up the list as we remove
            // - Is this necessary?? We won't have that many ships and when we load the level it should clear them anyway?
            shipObjects[shipID] = null;
            Destroy(shipToRemove);
        }

    } // end ClearShip

}
