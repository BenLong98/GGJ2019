using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRocketLauncher : Pickup {
	void Awake()
    {
        weapon = new RocketLauncher();
    }
}
