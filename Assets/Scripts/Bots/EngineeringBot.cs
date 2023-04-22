using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineeringBot : GenericBot
{
    // constant variables for this bot
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Engine };

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // engineering bots profession is engineering, they can work on engineering modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        engineering = PROFESSION_SKILL_VALUE;

    } // end Start

    /// <summary>
    /// Update is called once per frame - for now just calling the base class
    /// </summary>
    void Update()
    {
        base.Update();

    } // end Update
}
