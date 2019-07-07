using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setPlayerName : MonoBehaviour {

	[SerializeField] InputField nameField;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FindAndSetName() {
		GameObject.FindWithTag("PlayerUI").GetComponent<PlayerNameHolder>().PlayerName = nameField.text;
	}

	public void SetName(string nameToSet) {
		GameObject.FindWithTag("PlayerUI").GetComponent<PlayerNameHolder>().PlayerName = nameToSet;
	}
}
