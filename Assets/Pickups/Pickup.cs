using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Pickup : NetworkBehaviour {

    public PlayerWeapon weapon;

    /// <summary>
    /// Destroys this pickup instance
    /// </summary>
    public void Take()
    {
        NetworkServer.Destroy(this.gameObject);
    }

}
