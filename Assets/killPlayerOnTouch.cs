using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killPlayerOnTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<HealthController>()) {
			// i send this it's own ID, I only need damage
			col.gameObject.GetComponent<HealthController>().TakeDamage(10000, col.gameObject.GetComponent<HealthController>().netId);
		}
	}
}
