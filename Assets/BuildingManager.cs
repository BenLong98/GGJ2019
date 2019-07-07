using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class BuildingManager : NetworkBehaviour {

    public List<GameObject> buildings;
    public List<GameObject> toDestroy = new List<GameObject>();

    private void Start()
    {
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("structure"));
    }

    public void GetRidOfBuildsAbove(GameObject build)
    {
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("structure"));

        FindObjectsToDestroy(build);

        foreach( GameObject _go in toDestroy.ToArray())
        {
            if (_go.GetComponent<WallController>()) {
                _go.GetComponent<WallController>().RpcDeath();
            }
        }

        toDestroy.Clear();
    }



    public void FindObjectsToDestroy(GameObject build) {
        foreach (GameObject obj in buildings.ToArray())
        {
            if (obj.GetComponent<BoxCollider>().bounds.Intersects(build.GetComponent<BoxCollider>().bounds) &&
            obj.gameObject.transform.position.y < build.transform.position.y)
            {
                //obj.GetComponent<WallController>().RpcDeath();
                buildings.Remove(obj);
                toDestroy.Add(obj);
                FindObjectsToDestroy(obj);
            }
        }
    }
}
