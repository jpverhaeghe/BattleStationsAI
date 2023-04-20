using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // private constant variables
    private const int NUM_PHASES = 6;                               // the number of phases in a round
    private const int TIME_PER_PHASE = 3;                           // time to wait between phases in seconds

    // Serialized fields for this script
    [Header("Camearas for switching perspectives")]
    [SerializeField] Camera shipCamera;
    [SerializeField] Camera playerCamera;

    [Header("Elements to move player to helm location")]
    [SerializeField] ShipManager shipManager;
    [SerializeField] GameObject player;

    [Header("UI Elements to turn on and off")]
    [SerializeField] GameObject shipCamButton;
    [SerializeField] GameObject playerCamButton;
    [SerializeField] GameObject cursorToggleInfo;

    [Header("HUD Elements to keep track current round and phase")]
    [SerializeField] TMP_Text currentRoundText;
    [SerializeField] TMP_Text currentPhaseText;

    // private variables used by this script
    private int currentPhase;                                       // the current phase of the game (six phases in each round)
    private int currentRound;                                       // the current round of the game (some missions will be timed by rounds)

    private float timer;                                            // a timer to slow down the AI so we can see it

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        SetShipCamera();
        StartGame();

    } // end Start

    /// <summary>
    /// Update is called once per game loop, this returns the cursor to the screen
    /// </summary>
    void Update()
    {
        // if we are in player camera mode, and the C button is pressed, toggle the cursor
        if (playerCamera.enabled && Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = !Cursor.visible;

            // change the cursor lock state based on cursor visibility
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // TODO: This will happen every update, perhaps a timer to keep it from going too fast?
        timer += Time.deltaTime;

        if (timer > TIME_PER_PHASE)
        {
            timer = 0;
            DoPhases();
        }

    } // end Update

    /// <summary>
    /// Sets up and starts the game
    /// </summary>
    public void StartGame()
    {
        shipManager.InitializeShip();

        // set up the first round and phase information (not zero based)
        currentPhase = 0;
        UpdatePhase();

        currentRound = 0;
        UpdateRound();

    } // end StartGame

    private void DoPhases()
    {
        // Phase 1: if we are in phase 1, there is a little additional set up
        if (currentPhase == 1)
        {
            DoPhase1Setup();
        }

        // TODO: Move ships based on current speeds and direction

        // TODO: Move Missiles

        // TODO: Resolve Missles

        // TODO: Perform hero actions (bots for now - but one set of AI must go first)

        // TODO: Enemy Actions

        // Steady ship
        shipManager.UpdateOutOfControl(-1);

        // TODO: Deal with fire, gas and life support issues

        // TODO: Character recovery

        CheckEndRound();

    } // end DoPhases

    /// <summary>
    /// Does the intial set up for the round in phase one
    /// </summary>
    private void DoPhase1Setup()
    {
        // TODO: Energy levels can only go up if there is an engine working
        // all energy levels must be at least 1 at the beginning of the round
        if (shipManager.helmEnergyLevel < 1)
        {
            shipManager.UpdateHelmEnergy(1);
        }

        if (shipManager.weaponEnergyLevel < 1)
        {
            shipManager.UpdateWeaponsEnergy(1);
        }

        if (shipManager.shieldEnergyLevel < 1)
        {
            shipManager.UpdateShieldEnergy(1);
        }

    } // DoPhase1Setup

    /// <summary>
    /// Checks to see if the round has ended, and if so ends the round and sets up the next or increases to the next phase
    /// </summary>
    private void CheckEndRound()
    {
        // if we just finished the last phase, then reset the phase to 1 and increase the rounds
        if (currentPhase >= NUM_PHASES)
        {
            currentPhase = 0;
            UpdatePhase();
            UpdateRound();

            // have the ship manager do its own round cleanup
            shipManager.ShipRoundCleanUp();
        }
        else
        {
            UpdatePhase();
        }

    } // end CheckEndRound

    /// <summary>
    /// Sets the camera up for a walk through the ship
    /// </summary>
    public void WalkThroughShip()
    {
        Vector3 currentHelmPos = shipManager.GetShipHelmPos();


        // if the transform is not at the origin, we are good to teleport to the ship
        if (currentHelmPos != new Vector3())
        {
            //currentHelmPos.y += 1;
            player.transform.position = currentHelmPos;
            player.SetActive(true);

            // swap the cameras
            playerCamera.enabled = true;
            shipCamera.enabled = false;

            // turn off the player cam button
            playerCamButton.SetActive(false);
            cursorToggleInfo.SetActive(true);

            // turn on the ship cam button
            shipCamButton.SetActive(true);

            // by default unlocke the cursor and hide it so players can move freely to begin with
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

    } // end WalkThroughShip

    /// <summary>
    /// Sets the camera back to the ship view
    /// </summary>
    public void SetShipCamera()
    {
        // set up the main camera as the ship camera to begin with
        shipCamera.enabled = true;
        playerCamera.enabled = false;

        // turn on the player cam button
        playerCamButton.SetActive(true);
        cursorToggleInfo.SetActive(false);

        // turn off the ship cam button
        shipCamButton.SetActive(false);

        // de-activate the player
        player.SetActive(false);

    } // end SetShipCamera

    /// <summary>
    /// Increases the round number and updates the UI with the current round data
    /// </summary>
    private void UpdateRound()
    {
        currentRound++;
        currentRoundText.text = "Round = " + currentRound;

    } // UpdateRound

    /// <summary>
    /// Increases the phase number and updates the UI with the current phase data
    /// </summary>
    private void UpdatePhase()
    {
        currentPhase++;
        currentPhaseText.text = "Phase = " + currentPhase;

    } // UpdatePhase
}
