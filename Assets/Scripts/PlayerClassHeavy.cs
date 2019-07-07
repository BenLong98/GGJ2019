using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClassHeavy : PlayerController {
    


	public override void Awake()
    {
        base.Awake();

        MyClass = PlayerClass.Heavy;

        this.GetComponent<Toolbar>().primaryWeapon = new Chaingun();
        // secondary is empty

        startingHealth = 500;
        speed = 5f;
        jumppower = 10f;
    }

    [ClientRpc]
    public override void RpcDeath() {
        base.RpcDeath();
    }

}
