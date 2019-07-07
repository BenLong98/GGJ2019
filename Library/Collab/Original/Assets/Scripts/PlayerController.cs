using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerClass { Heavy, Assault, Ninja }

[RequireComponent(typeof(MovementControl))]
public class PlayerController : HealthController {

    GameObject bulletPrefab;



    public PlayerClass MyClass;
    /// <summary> Bullets per second </summary>
    public float firerate = 0f;
    /// <summary> Between 0 and 1. 0 is perfect accuracy </summary>
    public float spread = 0f;
    /// <summary> Movement speed </summary>
    public float speed { set { this.GetComponent<MovementControl>().speed = value; } }
    public float jumppower { set { this.GetComponent<MovementControl>().jumpPower = value; } }

    float lastFire = 0f; //Time.time val of last fire

    public bool isBuilding = false;
    
	public virtual void Awake()
    {
        bulletPrefab = Resources.Load<GameObject>("Bullet");
	}
	
	// Used for inputs
	void Update()
    {
		if (isLocalPlayer && Input.GetMouseButton(0))
        {
            FireWeapon();
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.C)) {
            SetBuildingToggle(false);
        }
	}


    public bool SetBuildingToggle(bool isBuild) {
        return isBuild = !isBuild;  
    }


    // ------------------------------
    // PLAYER SPECIFIC FUNCTIONS
    // ------------------------------

    public virtual void FireWeapon()
    {
        // if ready to fire
        if (Time.time > lastFire + (1/firerate))
        {
            lastFire = Time.time;

        }
    }
    
    /// <summary>
    /// Shoots a bullet in raycast form
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="spread"></param>
    /// <param name="maxDistance"></param>
    public void BulletRaycast(Vector3 from, Vector3 to, int damage, float spread = 0f, float maxDistance = 999f)
    {
        RaycastHit hit;
        Vector3 direction = (to - from).normalized;

        Vector3 endPoint;

        // Calculate spread
        direction.x += Random.Range(-spread, spread);
        direction.y += Random.Range(-spread, spread);
        direction.z += Random.Range(-spread, spread);
        direction = direction.normalized;

        if (Physics.Raycast(from, direction, out hit, maxDistance))
        {
            print("hit");
            endPoint = hit.point;
            if (hit.transform.root.GetComponent<HealthController>())
            {
                hit.transform.root.GetComponent<HealthController>().TakeDamage(damage);
            }
        }
        else
        {
            print("theoretical");
            // No hit.point to pull from, calculate where the bullet would have gone to
            Vector3 theoretical = from + (direction * maxDistance);
            endPoint = theoretical;
        }



        GameObject bullet = Instantiate(bulletPrefab, from, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetAttributes(from, endPoint);
    }

    /// <summary>
    /// Gets the point that the cursor of this player is aiming at
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAim()
    {
        Vector3 endPoint = Vector3.zero;
        if (Camera.main)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 999f))
            {
                endPoint = hit.point;
            }
            else
            {
                Vector3 theoretical = Camera.main.transform.position + (ray.direction * 999f);
                endPoint = theoretical;
            }
        }

        return endPoint;
    }

}
