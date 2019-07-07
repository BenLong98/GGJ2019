using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class WallController : HealthController {

    public override void TakeDamage(int amount) {
        base.TakeDamage(amount);
    }

    public void CallResizeBuilding() {
        Debug.Log("Call Resize");
        RpcResize();
    }

    public void CallPosition(Vector3 placementVector) {
        RpcSetPos(placementVector);
    }

    [ClientRpc]
    public void RpcSetPos(Vector3 placementVector) {
        transform.position = placementVector;
    }

    [ClientRpc]
    public void RpcResize() {
        Debug.Log("Resize");
        var go = NetworkServer.FindLocalObject(this.netId);
    }

    [ClientRpc]
    public override void RpcDeath()
    {
        // Start Dust Particle to disappear for flare

        //Destory after seconds.
        //Destroy(this.gameObject);
        // we'll do that below in server death so it's properly networked
    }

    public override void OnServerDeath() {
        NetworkServer.Destroy(gameObject);
    }

    
}
