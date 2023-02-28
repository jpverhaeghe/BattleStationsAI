using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        SetShipCamera();

    } // end Start

    private void Update()
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
    }

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
}
