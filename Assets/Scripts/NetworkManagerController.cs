using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerController : MonoBehaviour {

    [SerializeField] GameObject networkManager;

	void Start () {
        DontDestroyOnLoad(networkManager);
	}
	
}
