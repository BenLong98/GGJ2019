using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class WallController : HealthController {

    BuildingManager bm;

    public void Start()
    {
        bm = FindObjectOfType<BuildingManager>();
    }


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
        bm.buildings.Add(transform.gameObject);
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
        bm.GetRidOfBuildsAbove(this.gameObject);
        //Destory after seconds.
        Destroy(this.gameObject);
        bm.buildings.Remove(this.gameObject);

        // we'll do that below in server death so it's properly networked
    }

    public override void OnServerDeath() {
        NetworkServer.Destroy(gameObject);
    }

    
}
