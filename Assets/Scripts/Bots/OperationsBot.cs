using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationsBot : MonoBehaviour
{
    // private fields to help set up the operation bot
    private ShipManager shipManager;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        shipManager = GameObject.Find("ShipSpawner").GetComponent<ShipManager>();

    } // end Start

    // Update is called once per frame
    void Update()
    {
        
    }
}
