using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaScript : MonoBehaviour {

	// Use this for initialization
	float lerpFrom = 0;
	float lerpTo = 1;
	float lerpSpeed = 0.001f;
	float currentLerp = 0;

	Material mat;
	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;

	}
	
	// Update is called once per frame
	void Update () {
		
		mat.SetFloat("_scaletime",Mathf.Max(Mathf.Sin(Time.time)/8f-0.05f,0));
		mat.SetFloat("_passedTime",Time.time/8f);
	}
}
