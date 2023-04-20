using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedShip : MonoBehaviour
{
    // public variables used by the ship manager for keeping track of the current ships state
    public Vector3 shipHelmPos;                         // holds the helm player position for walking through the ship
    public int shipID;                                  // the id of the ship
    public int shipSize;                                // used to store the current generated ship size
    public int currentSpeed;                            // the current speed of the ship
    public int outOfControlLevel;                       // how out of control the ship is, affects bot skills
    public int helmEnergyLevel;                         // the energy amount in the ships drive systems (turning, etc.)
    public int weaponEnergyLevel;                       // the energy amount in the weapons systems
    public int shieldEnergyLevel;                       // the energy amount in the shields systems

    /// <summary>
    /// Initial set up when this ship is created
    /// </summary>
    private void Start()
    {
        currentSpeed = 1;
        outOfControlLevel = 0;
        helmEnergyLevel = 0;
        weaponEnergyLevel = 0;
        shieldEnergyLevel = 0;
    }

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

    } // end UpdateShieldEnergy
}
