using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceBot : GenericBot
{

    // private variables

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // science bots profession is science, they can work on science modules well, but not other actions
        athletics = NON_PROFESSION_SKILL_VALUE;
        combat = NON_PROFESSION_SKILL_VALUE;
        engineering = NON_PROFESSION_SKILL_VALUE;
        piloting = NON_PROFESSION_SKILL_VALUE;
        science = PROFESSION_SKILL_VALUE;

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }
}
