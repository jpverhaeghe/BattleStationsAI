using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBot : GenericBot
{

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // security bots profession is combat, they can work on combat modules and general combat well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = PROFESSION_SKILL_VALUE;
        engineering = NON_PROFESSION_SKILL_VALUE;
        piloting = NON_PROFESSION_SKILL_VALUE;
        science = NON_PROFESSION_SKILL_VALUE;

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }
}
