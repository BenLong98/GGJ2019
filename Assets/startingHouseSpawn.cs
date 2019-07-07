using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class startingHouseSpawn : NetworkBehaviour {

	public GameObject playerHousePrefab;

	// Use this for initialization
	void Start () {
		/*if (isServer) {
			GameObject playerHouse = Instantiate(playerHousePrefab, new Vector3(-3, 0.31f, 15.5f), Quaternion.identity);

			
			for (int i = 0; i < playerHouse.transform.childCount; i++)
			{
				Transform childt = playerHouse.transform.GetChild(i);
				print(childt);
				if (childt != null) {
					print("spawned");
					NetworkServer.Spawn(childt.gameObject);
				}
			}

			Destroy(playerHouse);
			NetworkServer.Spawn(playerHouse);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
