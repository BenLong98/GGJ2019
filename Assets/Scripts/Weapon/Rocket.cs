using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    float timeToDestroy;

    int damage = 0;

    float speed = 0.15f;

    public GameObject rocketExplosionPrefab;

    UnityEngine.Networking.NetworkInstanceId id;



    void OnCollisionEnter(Collision col)
    {
        SelfDestruct();
    }

    void Update()
    {
        if (Time.time > timeToDestroy)
        {
            SelfDestruct();
        }
        else
        {
            this.transform.position += this.transform.forward * speed;

            // baby collision detector
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward * speed, out hit, speed))
            {
                SelfDestruct();
            }
        }
    }

    public void SetAttributes(int _damage, float _timer, UnityEngine.Networking.NetworkInstanceId _id)
    {
        timeToDestroy = Time.time + _timer;
        damage = _damage;
        id = _id;
    }

    void SelfDestruct()
    {
        // radius check
        Task.DealDamageInRadius(this.transform.position, 3f, damage, id);

        // explosion
        GameObject exp = Instantiate(rocketExplosionPrefab, this.transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }

}
