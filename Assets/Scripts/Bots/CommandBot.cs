using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBot : GenericBot
{

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // command bots profession is piloting, they can control the ship from the helm well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = NON_PROFESSION_SKILL_VALUE;
        engineering = NON_PROFESSION_SKILL_VALUE;
        piloting = PROFESSION_SKILL_VALUE;
        science = NON_PROFESSION_SKILL_VALUE;

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }
}
