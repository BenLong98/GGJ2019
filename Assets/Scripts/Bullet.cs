using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    bool start = false;

    Vector3 from;
    Vector3 to;
    float distance;

    float startTime;
    float time;

    float speed = 100f;



    // called before the first Update frame
    void Start()
    {

    }

	// Update is called once per frame
	void Update()
    {
		if (start)
        {
            // Distance moved = time * speed.
            float distanceCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed = current distance divided by total distance.
            float t = distanceCovered / distance;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(from, to, t);
        }
	}



    /// <summary> Sets the to and from position of the bullet so it can go there and delete itself in reasonable time </summary>
    public void SetAttributes(Vector3 _from, Vector3 _to)
    {
        startTime = Time.time;

        from = _from;
        to = _to;

        distance = Vector3.Distance(from, to);

        time = distance / speed;


        start = true;

        Invoke("SelfDestruct", time);
    }

    void SelfDestruct()
    {
        //Destroy(this); // done after the particle effect ends now
        this.GetComponent<ParticleSystem>().Play();
    }

}
