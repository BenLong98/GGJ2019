using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructTimer : MonoBehaviour {

    public float timer;
    public float randomizer;
    


	void Start()
    {
        float rnd = Random.Range(-randomizer, randomizer);
        Invoke("DestroyMe", timer + rnd);
	}
	
    void DestroyMe()
    {
        Destroy(this.gameObject);
    }

}
