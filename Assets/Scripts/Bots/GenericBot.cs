using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBot : MonoBehaviour
{
    public enum BotType
    {
        COMMAND,
        ENGINEERING,
        SCIENCE,
        WEAPONS,
        OPERATIONS
    }

    // constant values used by other classes (specifically sub-classes)
    public static int PROFESSION_SKILL_VALUE = 3;
    public static int NON_PROFESSION_SKILL_VALUE = 1;

    // public variables to be accessed by outside scripts

    // protected values used by this class and its sub-classes 
    // TODO: if we need these outward facing, then make public
    protected GeneratedShip myShip;                                 // a link back to the ship we are tied to
    protected int athletics;                                        // used for movement
    protected int combat;                                           // used for combat - weapons officer profession
    protected int engineering;                                      // used for combat - engineering officer profession
    protected int piloting;                                         // used for combat - command officer profession
    protected int science;                                          // used for combat - science officer profession

    protected int moveSpeed = 4;                                    // the amount of squares a bot can move - same for all bots

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // generic bots profession will be athletics, they can move around faster but are not good at any stations
        athletics = PROFESSION_SKILL_VALUE;
        combat = NON_PROFESSION_SKILL_VALUE;
        engineering = NON_PROFESSION_SKILL_VALUE;
        piloting = NON_PROFESSION_SKILL_VALUE;
        science = NON_PROFESSION_SKILL_VALUE;

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets the ship script that this bot is associated with
    /// </summary>
    /// <param name="parentShip">a link to the GeneratedShip script for this bot</param>
    public void SetShip(GeneratedShip parentShip)
    {
        myShip = parentShip;

    } // end SetShip

}
