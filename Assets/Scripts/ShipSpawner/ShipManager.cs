using AlanZucconi.AI.PF;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomData;

public class ShipManager : MonoBehaviour
{
    public static int COLLISION_DAMAGE = 5;
    public static int MAX_HEX_RANGE = 50;
    public const int BUFFER_BETWEEN_SHIPS = 10;
    public const int DAMAGE_TO_LOSE = 50;                               // the amount of damage a ship must take to be destroyed

    private string[] energyLabels = { "Helm", "Guns", "Shld"};

    // Serialized fields used by this script
    [Header("Ship Generation Elements")]
    [SerializeField] GameManager gameManager;
    [SerializeField] ShipLayoutGenerator shipLayoutGeneratorScript;     // a link to the ship layout generator to call when generate ship is pressed
    [SerializeField] public TMP_Dropdown botChatterChoice;              // allows user to choose what chatter is being shown
    [SerializeField] public TMP_InputField damageMaximum;               // allows user to choose what chatter is being shown
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
    [SerializeField] TMP_Text enemyDistText;

    [Header("HUD Elements for enemy ship")]
    [SerializeField] TMP_Text enemyDirectionText;
    [SerializeField] TMP_Text enemyDamageText;
    [SerializeField] TMP_Text enemySpeedText;
    [SerializeField] TMP_Text enemyOOCText;
    [SerializeField] TMP_Text[] enemyEnergyText;
    [SerializeField] TMP_Text enemyScansText;

    [Header("Audio clips for battle sounds")]
    [SerializeField] AudioClip[] battleSounds;

    // private variables used by this script
    private RoomSpawner roomSpawner;                                    // A refrence to the class roomSpawner    
    private List<GameObject> shipObjects;                               // GameObject list that stores all the spawned gameObjects to keep things easy to find
    private int currentSpawnShipSize;                                   // A temporary variable to store the ship size until the object is created
    private int enemyDistance;

    //Is a list of the different ship options
    /*private List<RoomInfo[,]> shipList = new List<RoomInfo[,]>
    {
        RedundantII, Valiant, Fearlight, TentacScout, XeloxianScout, SilicoidScout, CanosianScout, Starbase,
    };*/

    /// <summary>
    /// Initializes variables for this ship manager
    /// </summary>
    private void Awake()
    {
        // assigns the RoomSpawner script for later use
        roomSpawner = gameObject.GetComponent<RoomSpawner>();

    } // end Awake

    // Temporary to update enemy ship stats. Will eventually use ship systems for second ship
    private void Update()
    {
        // continually update distances of ships so we can see the calculations (should be the same)
        if (gameManager.simulationRunning)
        {
            if (shipObjects.Count > 1) {
                GeneratedShip heroShip = shipObjects[0].GetComponent<GeneratedShip>();
                GeneratedShip enemyShip = shipObjects[1].GetComponent<GeneratedShip>();

                int distToEnemy = GetDistanceBetweenShips(enemyShip.mapLocation, heroShip.mapLocation);

                enemyDistText.text = "Enemy Dist: " + distToEnemy;

                // if the distances are too close, move the ships away from each other as if they collided
                if (distToEnemy == 0)
                {
                    UpdateShipDirection(heroShip.shipID, heroShip.currentDirection - 180);
                    UpdateHullDamage(heroShip.shipID, COLLISION_DAMAGE);
                    UpdateSpeed(heroShip.shipID, 2);

                    UpdateShipDirection(enemyShip.shipID, enemyShip.currentDirection - 180);
                    UpdateHullDamage(enemyShip.shipID, COLLISION_DAMAGE);
                    UpdateSpeed(enemyShip.shipID, 2);
                }
            }
        }
    }

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
    /// Checks to see if a ship is destroyed and if so ends the simulation
    /// </summary>
    /// <returns></returns>
    public bool CheckEndSimulation()
    {
        int shipDestroyed = -1;

        if (shipObjects != null)
        {
            GeneratedShip heroShip = shipObjects[0].GetComponent<GeneratedShip>();
            GeneratedShip enemyShip = shipObjects[1].GetComponent<GeneratedShip>();

            int damageMax;
            bool success = int.TryParse(damageMaximum.text, out damageMax);

            if (!success) 
            { 
                damageMax = DAMAGE_TO_LOSE; 
            }

            // did the hero ship lose?
            if (heroShip.hullDamage >= damageMax)
            {
                shipDestroyed = 0;
                enemyShip.currentTarget = null;
            }
            // did the enemy ship lose?
            else if (enemyShip.hullDamage >= damageMax)
            {
                shipDestroyed = 1;
                heroShip.currentTarget = null;
            }
        }

        if (shipDestroyed >= 0)
        {
            // stop al messages and post a message
            botRollText.text = shipObjects[shipDestroyed].name + " was destroyed. Simulation ending!";
            botChatterChoice.value = 0;
            
            // Destroy the ship object that was destryoed
            ClearShip(shipDestroyed);
        }

        return (shipDestroyed >= 0);

    } //end CheckEndSimulation

    /// <summary>
    /// Resets all the ship data text to the original data so it doesn't look like the sim is still running after
    /// we end a simulation
    /// </summary>
    public void ResetShipDataText()
    {
        // Hero ship data text
        shipDirectionText.text = "Dir = " + 0;
        shipDamageText.text = "Dmg = " + 0;
        shipSpeedText.text = "Spd = " + 0;
        shipOOCText.text = "OOC = " + 0;
        
        for (int i = 0; i < shipEnergyText.Length; i++)
        {
            shipEnergyText[i].text = energyLabels[i] + " = " + 0;
        }

        shipScansText.text = "Scn = " + 0;
        enemyDistText.text = "Enemy Dist: " + 0;

        // enemy ship data text
        enemyDirectionText.text = "Dir = " + 0;
        enemyDamageText.text = "Dmg = " + 0;
        enemySpeedText.text = "Spd = " + 0;
        enemyOOCText.text = "OOC = " + 0;

        for (int i = 0; i < shipEnergyText.Length; i++)
        {
            enemyEnergyText[i].text = energyLabels[i] + " = " + 0;
        }

        enemyScansText.text = "Scn = " + 0;

        // the chatter data
        botRollText.text = "";
        botStatusText.text = "";

    } // end ResetShipDataText

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
    /// <param name="directionChange">The value to change the ship direction by</param>
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
    /// <param name="oocChange">The value to change the OOC by</param>
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
    /// Updates the given energy level with to the new value
    /// </summary>
    /// <param name="shipID">The index of the ship that is being adjusted in the list</param>
    /// <param name="energySystem">The energy system to update</param>
    /// <param name="energyChange">The value to change the energy by</param>
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
    /// Updates the number of scans with to the new value
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
    public void GenerateShips(int[] shipSizes, Vector2Int[] mapLocations)
    {
        ClearShips();

        // go through each ship type and put them in the world offset by the ship sizes starting at 0,0
        // TODO: Figure a way to split the screen?
        float worldPosX = 0;
        float worldPosZ = 0;

        for (int shipID = 0; shipID < shipSizes.Length; shipID++)
        {
            RoomInfo[,] ship = shipLayoutGeneratorScript.GenerateShipLayout(shipSizes[shipID]);
            CreateShip(ship, worldPosX, worldPosZ, mapLocations[shipID]);

            // adjust world size based on the previous ship size - moving it to the right 
            worldPosX += (ship.GetLength(1) * RoomSpawner.ROOM_WIDTH) + BUFFER_BETWEEN_SHIPS;
        }

        // Set the targets for the ships so they attack each other - just have them target the one down the line for now
        for (int shipID = 0; shipID < shipObjects.Count; shipID++)
        {
            int targetShipID = shipID + 1;

            // wrap around if we are at the end
            if (targetShipID >= shipObjects.Count)
            {
                targetShipID = 0;
            }

            shipObjects[shipID].GetComponent<GeneratedShip>().SetTarget(shipObjects[targetShipID].GetComponent<GeneratedShip>());
        }

    } // end GenerateShip

    /// <summary>
    /// Creates a ship from the pre-built list at the origin of the scene
    /// </summary>
    /*public void CreateShip()
    {
        ClearShips();

        // choose one from the drop down
        int shipType = prebuiltShipList.value;
        CreateShip(shipList[shipType], 0, 0);

    } // end Create ship at origin*/

    /// <summary>
    /// This method starts the instanition of a ship based
    /// </summary>
    /// <param name="ship">The ship layout for the ship to generate</param>
    /// <param name="xPos">Where the ship should be placed in the world on the X axis</param>
    /// <param name="zPos">Where the ship should be placed in the world on the Z axis</param>
    /// <param name="mapLocation">Where to place the ship in the psuedo map for combat</param>
    public void CreateShip(RoomInfo[,] ship, float xPos, float zPos, Vector2Int mapLocation)
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

        // grabbing the ship script as we need to initialize some data before generating the ship and to set it up before it is used
        GeneratedShip thisShipScript = shipObject.GetComponent<GeneratedShip>();

        // as we add to the end, the id of the ship will be the current count
        int shipID = shipObjects.Count;
        this.shipObjects.Add(shipObject);
        thisShipScript.SetupBotStuct();

        // build the ship - need to initialize the walls before building so they can be populated in the build process
        thisShipScript.walls = new List<GameObject>();
        BuildShip(shipID, ship, xPos, zPos);
        thisShipScript.SetupShip(this, ship, shipWorldPos, shipID, currentSpawnShipSize, mapLocation);

        // if the ship ID is zero, set the camera to follow the bots on that ship
        gameManager.SetShipCamera();

    } // end CreateShip

    /// <summary>
    /// Removes all current ships this manager is dealing with
    /// </summary>
    public void ClearShips()
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

    } // end ClearShips

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
    /// Used to show chatter based on ship level
    /// </summary>
    /// <param name="shipID">the ship id for chatter - hero is zero, enemy is 1</param>
    /// <returns></returns>
    public bool ShowChatter(int shipID)
    {
        bool showChatter = false;

        int chatterChoice = botChatterChoice.value;

        // only show if it is turned on
        if (chatterChoice > 0)
        {
            chatterChoice--;

            if (shipID == chatterChoice)
            {
                showChatter = true;
            }
        }

        return showChatter;

    } // ShowChatter

    /// <summary>
    /// Debug system to show what bots are doing, may change rapidly
    /// </summary>
    /// <param name="shipID">the id of the ship attempting to display data</param>
    /// <param name="textToDisplay">text to update</param>
    public void UpdateBotRollText(int shipID, string textToDisplay)
    {
        if (ShowChatter(shipID))
        {
            botRollText.text = textToDisplay;
        }

    }// end UpdateBotRollText

    /// <summary>
    /// Debug system to show what bots are doing, may change rapidly
    /// </summary>
    /// <param name="shipID">the id of the ship attempting to display data</param>
    /// <param name="textToDisplay">text to update</param>
    public void UpdateBotStatusText(int shipID, string textToDisplay)
    {
        if (ShowChatter(shipID))
        {
            botStatusText.text = textToDisplay;
        }

    }// end UpdateBotStatusText

    /// <summary>
    /// Plays the audio clip for the given id
    /// </summary>
    /// <param name="clipToPlay"></param>
    public void playAudioClip(int clipToPlay)
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(battleSounds[clipToPlay]);

    } // end playAudioClip

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
    private void BuildShip(int shipID, RoomInfo[,] ship, float worldPos_x, float worldPos_z)
    {
        //Vector3 helmPos = new Vector3(0,0,0);

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
                // This is a static room so let's create a new one that is non-static to replace it!
                RoomInfo staticRoom = ship[roomRow, roomCol];

                if (staticRoom != null)
                {
                    // creating a new room to use
                    RoomInfo room = new RoomInfo(staticRoom.roomName, staticRoom.roomType, staticRoom.roomFacing, staticRoom.moduleType,
                                                 staticRoom.roomTiles, staticRoom.externalFacing);
                    // calculate the world position for this room to send down

                    // as rooms can be offset from other rooms based on location in the ship, set the roomPos_z to the current row times the height of a room
                    float roomPos_z = worldPos_z - (roomRow * RoomSpawner.ROOM_HEIGHT);

                    // as rooms can be offset from other rooms based on location in the ship, set the roomPos_x to the current col times the width of a room
                    float roomPos_x = worldPos_x + (roomCol * RoomSpawner.ROOM_WIDTH);

                    // set the world position of this room
                    room.SetRoomWorldPos(new Vector3(roomPos_x, 0, roomPos_z));

                    // instantiates the room objects based on the strings in the arrays 
                    roomSpawner.BuildRoom(shipObjects[shipID], room, roomRow, roomCol, roomPos_x, roomPos_z);

                    // add up the number of life supports so we can store it
                    if (room.moduleType == ModuleType.LifeSupport)
                    {
                        currentGeneratedShip.botShipData.LifeSupportModules++;
                    }

                    // store the number of each room type for adding more bots to the ship
                    if (room.roomType == RoomType.Command) { currentGeneratedShip.botShipData.botRoomTypeCount[(int)GenericBot.BotType.COMMAND]++; }
                    if (room.roomType == RoomType.Engineering) { currentGeneratedShip.botShipData.botRoomTypeCount[(int)GenericBot.BotType.ENGINEERING]++; }
                    if (room.roomType == RoomType.Science) { currentGeneratedShip.botShipData.botRoomTypeCount[(int)GenericBot.BotType.SCIENCE]++; }
                    if (room.roomType == RoomType.Weapons) { currentGeneratedShip.botShipData.botRoomTypeCount[(int)GenericBot.BotType.SECURITY]++; }
                    if (room.roomType == RoomType.Operations) { currentGeneratedShip.botShipData.botRoomTypeCount[(int)GenericBot.BotType.OPERATIONS]++; }

                    ship[roomRow, roomCol] = room;
                }
                // make the entire room a wall if it is null
                else
                {
                    int shipOffsetGridRow = (roomRow * RoomSpawner.ROOM_HEIGHT);
                    int shipOffsetGridCol = (roomCol * RoomSpawner.ROOM_WIDTH);

                    for (int tileRow = 0; tileRow < RoomSpawner.ROOM_HEIGHT; tileRow++)
                    {
                        for (int tileCol = 0; tileCol < RoomSpawner.ROOM_WIDTH; tileCol++)
                        {
                            // the current position of this tile in the walkable area graph
                            int currentShipTileRow = shipOffsetGridRow + (tileRow);
                            int currentShipTileCol = shipOffsetGridCol + (tileCol);
                            currentGeneratedShip.shipPathingSystem.SetWall(new Vector2Int(currentShipTileRow, currentShipTileCol));
                        }
                    }
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
