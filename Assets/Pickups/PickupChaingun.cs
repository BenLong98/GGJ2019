using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupChaingun : Pickup {
    
    void Awake()
    {
        weapon = new Chaingun();
    }

}
