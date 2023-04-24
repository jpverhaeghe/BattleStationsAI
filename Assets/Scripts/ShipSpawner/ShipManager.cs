using AlanZucconi.AI.PF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using static RoomData;

public class ShipManager : MonoBehaviour
{
    // for testing purposes - create an enemy ship struct to hold data for bots to use (eventually will be another ship)
    public struct EnemyShip
    {
        public int distance;                // the distance from our current location (will use wold position later - or hex map locations instead)
        public int hullDamage;              // only for damage purposes and bot decisions - will be converted to ship modules and how many are slagged
        public int speed;                   // the speed of a ship can be 0 to 5
        public int shieldLevel;             // the shield level of this ship
        public int direction;               // 360 with positive z 0, turning is in 60 degree chunks (cartesian)

        public Vector2Int mapLocation;
    }

    public static int ENEMY_SHIP_MAX_HULL_DAMAGE = 100;
    public static int MAX_HEX_RANGE = 50;

    private string[] energyLabels = { "Helm", "Guns", "Shld"};

    // Serialized fields used by this script
    [Header("Ship Generation Elements")]
    [SerializeField] GameManager gameManager;
    [SerializeField] ShipLayoutGenerator shipLayoutGeneratorScript;     // a link to the ship layout generator to call when generate ship is pressed
    [SerializeField] TMP_Dropdown prebuiltShipList;                     // a list of the prebuilt ships
    [SerializeField] public GameObject[] botPrefabs;                    // a link to an the bot prefabs for adding to a ship

    // TODO: Think of a way to re-use these for each ship as they go through the AI
    // - perhaps update a text box with a name of the ship above them
    [Header("HUD Elements to keep track of bot status")]
    [SerializeField] TMP_Text botRollText;
    [SerializeField] TMP_Text botStatusText;

    [Header("HUD Elements to keep track of hero ship data")]
    [SerializeField] TMP_Text shipDirectionText;
    [SerializeField] TMP_Text shipDamageText;
    [SerializeField] TMP_Text shipSpeedText;
    [SerializeField] TMP_Text shipOOCText;
    [SerializeField] TMP_Text[] shipEnergyText;
    [SerializeField] TMP_Text shipScansText;

    [Header("HUD Elements for enemy ship")]
    [SerializeField] TMP_Text enemyDirectionText;
    [SerializeField] TMP_Text enemyDamageText;
    [SerializeField] TMP_Text enemySpeedText;
    [SerializeField] TMP_Text enemyOOCText;
    [SerializeField] TMP_Text[] enemyEnergyText;
    [SerializeField] TMP_Text enemyScansText;
    [SerializeField] TMP_Text enemyDistText;

    public EnemyShip botTargetPractice = new EnemyShip();

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

        botTargetPractice.distance = 15;             // ships are stationary for now - this is missile hits in three round distance (if I add missiles)
        botTargetPractice.hullDamage = 0;
        botTargetPractice.direction = 180;
        botTargetPractice.speed = 0;
        botTargetPractice.shieldLevel = 2;

        botTargetPractice.mapLocation = new Vector2Int(10, 15);

    } // end Awake

    // Temporary to update enemy ship stats. Will eventually use ship systems for second ship
    private void Update()
    {
        enemySpeedText.text = "Spd = " + botTargetPractice.speed;
        enemyDamageText.text = "Dmg = " + botTargetPractice.hullDamage;
        enemyEnergyText[(int)GeneratedShip.ShipPowerAreas.SHIELDS].text = "Shld = " + botTargetPractice.shieldLevel;

        int distance = botTargetPractice.distance;

        if (shipObjects.Count > 0)
        {
            Vector2Int currentShipLoc = shipObjects[0].GetComponent<GeneratedShip>().mapLocation;
            distance = GetDistanceBetweenShips(currentShipLoc, botTargetPractice.mapLocation);
            botTargetPractice.distance = distance;
        }

        enemyDistText.text = "Enemy Dist: " + distance;
    }

    /// <summary>
    /// Initialiazes the ship and the rounds
    /// </summary>
    public void InitializeShip()
    {
        //GenerateShip();
        GenerateShip(ShipLayoutGenerator.ShipSize.Frigate);

    } // end InitializeShip

    /// <summary>
    /// Does the intial set up for the round in phase one for all ships
    /// </summary>
    public void DoPhase1Setup()
    {
        for (int shipId = 0; shipId < shipObjects.Count; shipId++)
        {
            GeneratedShip currentShip = shipObjects[shipId].GetComponent<GeneratedShip>();

            // TODO: Energy levels can only go up if there is an engine working
            // all energy levels must be at least 1 at the beginning of the round
            for (int energySystem = 0; energySystem < currentShip.energySystemLevels.Length; energySystem++)
            {
                if (currentShip.energySystemLevels[energySystem] < 1)
                {
                    UpdateEnergy(shipId, energySystem, 1);
                }
            }
        }

    } // DoPhase1Setup

    /// <summary>
    /// Move the generated ships along their direction in the hypothetical map
    /// TODO: Make the world wrap around or leave it or maybe not (ships always chase each other)
    /// </summary>
    public void MoveShips()
    {
        // go through all the ships and move them forward (if at edges leave them there)
        foreach (GameObject ship in shipObjects)
        {
            GeneratedShip shipScript = ship.GetComponent<GeneratedShip>();

            shipScript.MoveShip();

            if (shipScript.shipID == 0) {
                // temporary for bot practice - move the enemy closer
                botTargetPractice.distance -= shipScript.currentSpeed;

                botTargetPractice.distance = Math.Abs(botTargetPractice.distance);
            }
        }


    } // end MoveShips

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
        for (int shipId = 0; shipId < shipObjects.Count; shipId++)
        {
            GeneratedShip currentShip = shipObjects[shipId].GetComponent<GeneratedShip>();

            // adjust the levels per the rules (all energy and speed are reduced by 1)
            UpdateSpeed(shipId, -1);

            for (int energySystem = 0; energySystem < currentShip.energySystemLevels.Length; energySystem++) 
            {
                UpdateEnergy(shipId, energySystem, -1);
            }

            // Remove counters on any modules (engineering, helm, weapons, etc.)
            currentShip.ClearUsedMarkers();
        }

    } // ShipRoundCleanUp

    /// <summary>
    /// Updates the direction of the ship with to the new value
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="directionChange">The value to change the hull damage by</param>
    public void UpdateShipDirection(int shipID, int directionChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateShipDirection(directionChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipDirectionText.text = "Dir = " + shipObjects[shipID].GetComponent<GeneratedShip>().currentDirection.ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemyDirectionText.text = "Dir = " + shipObjects[shipID].GetComponent<GeneratedShip>().currentDirection.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateShipDirection: ShipID " + shipID + " does not exist");
        }

    } // end UpdateShipDirection

    /// <summary>
    /// Updates the hull damage with to the new value, never less than zero
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="hullDamage">The value to change the hull damage by</param>
    public void UpdateHullDamage(int shipID, int hullDamage)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateHullDamage(hullDamage);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipDamageText.text = "Dmg = " + shipObjects[shipID].GetComponent<GeneratedShip>().hullDamage.ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemyDamageText.text = "Dmg = " + shipObjects[shipID].GetComponent<GeneratedShip>().hullDamage.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateHullDamage: ShipID " + shipID + " does not exist");
        }

    } // end UpdateHullDamage

    /// <summary>
    /// Updates the speed with to the new value
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
                shipSpeedText.text = "Spd = " + shipObjects[shipID].GetComponent<GeneratedShip>().currentSpeed.ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemySpeedText.text = "Spd = " + shipObjects[shipID].GetComponent<GeneratedShip>().currentSpeed.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateShipSpeed: ShipID " + shipID + " does not exist");
        }

    } // end UpdateSpeed

    /// <summary>
    /// Updates the OOC level to the new value
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="oocChange">The value to change the hull damage by</param>
    public void UpdateOutOfControl(int shipID, int oocChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateOutOfControl(oocChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipOOCText.text = "OOC = " + shipObjects[shipID].GetComponent<GeneratedShip>().outOfControlLevel.ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemyOOCText.text = "OOC = " + shipObjects[shipID].GetComponent<GeneratedShip>().outOfControlLevel.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateHullDamage: ShipID " + shipID + " does not exist");
        }

    } // end UpdateOutOfControl

    /// <summary>
    /// Updates the helm energy level with to the new value
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="energySystem">The energy system to update</param>
    /// <param name="energyChange">The value to change the helm energy by</param>
    public void UpdateEnergy(int shipID, int energySystem, int energyChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateEnergy(energySystem, energyChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipEnergyText[energySystem].text = energyLabels[energySystem] + " = " + 
                    shipObjects[shipID].GetComponent<GeneratedShip>().energySystemLevels[energySystem].ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemyEnergyText[energySystem].text = energyLabels[energySystem] + " = " +
                    shipObjects[shipID].GetComponent<GeneratedShip>().energySystemLevels[energySystem].ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateEnergy: ShipID " + shipID + " does not exist");
        }

    } // end UpdateEnergy

    /// <summary>
    /// Updates the speed with to the new value
    /// TODO: When speed is over 4 - the ship should take damage
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="scanChange">The value to change the speed by</param>
    public void UpdateNumScans(int shipID, int scanChange)
    {
        // only adjust if the ship id is in the list, otherwise it is invalid
        if ((shipID >= 0) && (shipID < shipObjects.Count))
        {
            shipObjects[shipID].GetComponent<GeneratedShip>().UpdateNumScans(scanChange);

            // update the HUD text if this is the hero ship (ship ID 0)
            if (shipID == 0)
            {
                shipScansText.text = "Scn = " + shipObjects[shipID].GetComponent<GeneratedShip>().numStoredScans.ToString();
            }
            // or enemy ship (ship ID 1)
            else if (shipID == 1)
            {
                enemyScansText.text = "Scn = " + shipObjects[shipID].GetComponent<GeneratedShip>().numStoredScans.ToString();
            }
        }
        else
        {
            Debug.Log("ShipManager->UpdateNumScans: ShipID " + shipID + " does not exist");
        }

    } // end UpdateNumScans

    /// <summary>
    /// Generate a random ship at the origin of the scene
    /// </summary>
    //public void GenerateShip()
    public void GenerateShip(ShipLayoutGenerator.ShipSize shipType)
    {
        ClearShips();

        // Get the room info layout and create the ship - for now only one ship is built
        //RoomInfo[,] ship = shipLayoutGeneratorScript.GenerateShipLayout();
        RoomInfo[,] ship = shipLayoutGeneratorScript.GenerateShipLayout(shipType);
        CreateShip(ship, 0, 0);

    } // end GenerateShip

    /// <summary>
    /// Creates a ship from the pre-built list at the origin of the scene
    /// </summary>
    public void CreateShip()
    {
        ClearShips();

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
        // create the list of ship game objects if one does not already exist
        if (shipObjects == null)
        {
            shipObjects = new List<GameObject>();
        }

        // create a ship in the list to store the data in for later
        string shipObjectName = "EnemyShip";

        // if this is the first ship, give it a unique name
        if (shipObjects.Count == 0)
        {
            shipObjectName = "HeroShip";
        }

        GameObject shipObject = new GameObject(shipObjectName);
        Vector3 shipWorldPos = new Vector3(xPos, 0, zPos);
        shipObject.transform.position = shipWorldPos;

        // add the ship component that keeps track of an individual ship status
        shipObject.AddComponent<GeneratedShip>();

        // as we add to the end, the id of the ship will be the current count
        int shipID = shipObjects.Count;
        this.shipObjects.Add(shipObject);

        // build the ship
        //Vector3 helmPos = BuildShip(shipId, ship, xPos, zPos);
        BuildShip(shipID, ship, xPos, zPos);
        shipObjects[shipID].GetComponent<GeneratedShip>().SetupShip(this, ship, shipWorldPos, /*helmPos,*/ shipID, currentSpawnShipSize);

        // if the ship ID is zero, set the camera to follow the bots on that ship
        gameManager.SetShipCamera();

    } // end CreateShip

    /// <summary>
    /// Returns the ship helm coordinates
    /// </summary>
    /// <param name="shipID">The id of the ship to access</param>
    /// <returns>The vector 3 position of where to place a player in the helm</returns>
    /*public Vector3 GetShipHelmPos(int shipId)
    {
        return shipObjects[shipId].GetComponent<GeneratedShip>().shipHelmPos;

    } // end GetShipHelmPos*/

    /// <summary>
    /// Gets the bot of the hero ship so we can follow along with the camera
    /// TODO: Right now it only gets the first bot, need to add a way to get different bots
    /// </summary>
    /// <returns>The bot to follow</returns>
    public GameObject GetBotToFollow()
    {
        return shipObjects[0].GetComponent<GeneratedShip>().GetBotToFollow();

    } // GetBotToFollow

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
    /// Debug system to show what bots are doing, may change rapidly
    /// </summary>
    /// <param name="textToDisplay">text to update</param>
    public void UpdateBotRollText(string textToDisplay)
    {
        botRollText.text = textToDisplay;

    }// end UpdateBotRollText

    /// <summary>
    /// Debug system to show what bots are doing, may change rapidly
    /// </summary>
    /// <param name="textToDisplay">text to update</param>
    public void UpdateBotStatusText(string textToDisplay)
    {
        botStatusText.text = textToDisplay;

    }// end UpdateBotStatusText

    public int GetDistanceBetweenShips(Vector2Int shipA, Vector2Int shipB)
    {
        int distance = (int)Math.Abs(Math.Sqrt(Math.Pow((shipB.x - shipA.x), 2) + Math.Pow((shipB.y - shipA.y), 2)));
        return distance;
    }

    /// <summary>
    /// Build the ship using the room information and positional values in game
    /// </summary>
    /// <param name="shipID">The id of the ship in the ship objects list</param>
    /// <param name="ship">The ship as a room information layout array</param>
    /// <param name="worldPos_x">The x world position for the top left of the ship</param>
    /// <param name="worldPos_z">The z world position for the top left of the ship</param>
    ///// <returns>A vector3 with the ship's helm coordinates</returns>
    //private Vector3 BuildShip(int shipId, RoomInfo[,] ship, float worldPos_x, float worldPos_z)
    private void BuildShip(int shipID, RoomInfo[,] ship, float worldPos_x, float worldPos_z)
    {
        //Vector3 helmPos = new Vector3(0,0,0);
        int numLifeSupports = 0;

        // set up the grid for bot pathing while building the ship - need to pass it to BuildRoom and have it passed back
        GameObject currentShip = shipObjects[shipID];
        GeneratedShip currentGeneratedShip = currentShip.GetComponent<GeneratedShip>();
        currentGeneratedShip.shipPathingSystem = new Grid2D(new Vector2Int(ship.GetLength(0) * RoomSpawner.ROOM_HEIGHT, 
                                                                           ship.GetLength(1) * RoomSpawner.ROOM_WIDTH));

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

                    // set the world position of this room
                    room.SetRoomWorldPos(new Vector3(roomPos_x, 0, roomPos_z));

                    // if the room is the helm, then set up the helm position
                    /*if (room.moduleType == ModuleType.Helm)
                    {
                        helmPos.x = roomPos_x + TILE_CENTER_OFFSET;
                        helmPos.z = roomPos_z - TILE_CENTER_OFFSET;
                    }*/

                    // instantiates the room objects based on the strings in the arrays 
                    roomSpawner.BuildRoom(shipObjects[shipID], room, roomRow, roomCol, roomPos_x, roomPos_z);

                    // add up the number of life supports so we can store it
                    if (room.moduleType == ModuleType.LifeSupport)
                    {
                        numLifeSupports++;
                    }
                }
                // make the entire room a wall if it is null
                else
                {
                    for (int row = 0; row < RoomSpawner.ROOM_HEIGHT; row++)
                    {
                        for (int col = 0; col < RoomSpawner.ROOM_WIDTH; col++)
                        {
                            currentGeneratedShip.shipPathingSystem.SetWall(new Vector2Int(roomRow + row, roomCol + col));
                        }
                    }
                }
            }
        }

        // store the number of life supports for later use by the ship
        currentGeneratedShip.numLifeSupports = numLifeSupports;

        //return helmPos;

    } // end BuildShip

    /// <summary>
    /// Removes all current ships this manager is dealing with
    /// </summary>
    private void ClearShips()
    {
        // only need to clear ships if the list exists
        if (shipObjects != null)
        {
            // for now remove the old ships (if there are any) as this method should only be called from the generate ship buttons
            for (int i = 0; i < shipObjects.Count; i++)
            {
                ClearShip(i);
            }

            // clear out the ship list as well
            shipObjects.Clear();
        }

    } // end ClearShip

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
