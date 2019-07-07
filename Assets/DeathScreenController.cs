using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathScreenController : MonoBehaviour {

	NetworkLobbyManager netManager;

	// Use this for initialization
	void Start () {
		if (!netManager && GameObject.FindWithTag("LobbyManager")) {
			netManager = GameObject.FindWithTag("LobbyManager").GetComponent<NetworkLobbyManager>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void retryGame() {
		NetworkServer.Reset();
	}

	public void backToLobby() {
		netManager.ServerReturnToLobby();
        this.gameObject.SetActive(false);
	}
}
