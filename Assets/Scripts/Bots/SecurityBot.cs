using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBot : GenericBot
{
    // constant variables for this bot
    public static RoomData.ModuleType[] modules = { RoomData.ModuleType.Cannon, RoomData.ModuleType.MissileBay };

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        base.Start();

        // security bots profession is combat, they can work on combat modules and general combat well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = PROFESSION_SKILL_VALUE;

    } // end Start

    /// <summary>
    /// Update is called once per frame - for now just calling the base class
    /// </summary>
    void Update()
    {
        base.Update();
        
    } // end Update
}
