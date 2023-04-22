using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceBot : GenericBot
{
    // constant variables for this bot
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Science, RoomData.ModuleType.Hyperdrive };

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // science bots profession is science, they can work on science modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        science = PROFESSION_SKILL_VALUE;

    } // end Start

    /// <summary>
    /// Update is called once per frame - for now just calling the base class
    /// </summary>
    void Update()
    {
        base.Update();
        
    } // end Update
}
