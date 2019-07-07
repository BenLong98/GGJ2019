using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hexShaderScript : MonoBehaviour {

	
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
		
		mat.SetFloat("_HexAlpha",Mathf.Max(Mathf.Sin(Time.time)+0.1f,2.8f));
		mat.SetFloat("_passedTime",Time.time/10f);
	}
}
