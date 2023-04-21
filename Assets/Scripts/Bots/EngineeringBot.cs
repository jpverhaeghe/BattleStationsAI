using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineeringBot : GenericBot
{

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // engineering bots profession is engineering, they can work on engineering modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = NON_PROFESSION_SKILL_VALUE;
        engineering = PROFESSION_SKILL_VALUE;
        piloting = NON_PROFESSION_SKILL_VALUE;
        science = NON_PROFESSION_SKILL_VALUE;

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }
}
