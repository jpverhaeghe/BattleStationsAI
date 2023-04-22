using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBot : GenericBot
{
    // constant variables for this bot
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Helm };

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // command bots profession is piloting, they can control the ship from the helm well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        piloting = PROFESSION_SKILL_VALUE;

    } // end Start

    /// <summary>
    /// Update is called once per frame - for now just calling the base class
    /// </summary>
    void Update()
    {
        base.Update();

    } // end Update
}
