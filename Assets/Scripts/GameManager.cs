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
    [Header("Camearas for switching bot perspecitves")]
    [SerializeField] Camera shipCamera;
    //[SerializeField] GameObject shipCamButton;                    // will be re-purposed to switch bots, may need two

    [Header("Elements to ship creation and updating")]
    [SerializeField] ShipManager shipManager;

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
        StartGame();
        SetShipCamera();

    } // end Start

    /// <summary>
    /// Update is called once per game loop, this returns the cursor to the screen
    /// </summary>
    void Update()
    {
        // Added a timer for now, eventually this will call the bot systems from the Ship Manager and they will perform actions
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
            shipManager.DoPhase1Setup();
        }

        // TODO: Move ships based on current speeds and direction

        // TODO: Move Missiles

        // TODO: Resolve Missles

        // TODO: Perform hero actions (bots for now - but one set of AI must go first)

        // TODO: Enemy Actions

        // Steady ship
        shipManager.SteadyShips();

        // TODO: Deal with fire, gas and life support issues

        // TODO: Character recovery

        CheckEndRound();

    } // end DoPhases

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
    /// Will re-purpose this to move forward in bots for follow mode
    /// </summary>
    public void SetShipCamera()
    {
        // set up the bot to follow at the beginning
        shipCamera.GetComponent<FollowBot>().SetBotToFollow(shipManager.GetBotToFollow() );

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
