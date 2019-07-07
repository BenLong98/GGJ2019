using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public enum WeaponSlot { Primary, Secondary }

/// <summary>
/// Handles input and switching weapons
/// </summary>
public class Toolbar : NetworkBehaviour
{

    public bool ON = true;


    bool isLocalCache = false;

    GameObject bulletPrefab;

    public WeaponSlot currentWeaponSlot = WeaponSlot.Primary;
    public PlayerWeapon currentWeapon;

    GameObject nameTag;

    public PlayerWeapon primaryWeapon
    {
        get { return _primaryWeapon; }
        set { value.tb = this; _primaryWeapon = value; }
    }
    private PlayerWeapon _primaryWeapon;

    public PlayerWeapon secondaryWeapon
    {
        get { return _secondaryWeapon; }
        set { value.tb = this; _secondaryWeapon = value; }
    }
    private PlayerWeapon _secondaryWeapon;

    public GameObject pickupUI;

    public Transform barrel;

    // Pickups
    KeyCode pickupKey = KeyCode.E;
    float pickupRange = 5f;
    Pickup availablePickup = null;




    void Awake()
    {
        pickupUI = GameObject.FindGameObjectWithTag("PickupUI");
        bulletPrefab = Resources.Load<GameObject>("Bullet");
        barrel = this.transform.Find("heavyAvatar").Find("pelvis").Find("waist").GetChild(0).Find("r_shoulder").GetChild(0).GetChild(0).GetChild(0).Find("Fists").GetChild(0);
    }

    void Start()
    {
        currentWeapon = primaryWeapon;
        ChangeCurrentVisual();

        // get the attached nametag and set the name to the one we held in PlayerUI while the page loaded

        nameTag = transform.Find("NameTagHolder").Find("NameTag").gameObject;
        nameTag.GetComponent<UnityEngine.UI.Text>().text = GameObject.FindWithTag("PlayerUI").GetComponent<PlayerNameHolder>().PlayerName;


        if (isLocalPlayer)
        {
            isLocalCache = true;
            nameTag.SetActive(false);
        }
    }

    void Update()
    {
        // testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (ON)
            {
                ON = false;
                Debug.LogWarning("TURNED OFF TOOLBAR");
            }
            else
            {
                ON = true;

                this.transform.Find("BuildingMode").GetChild(0).gameObject.SetActive(false);
                this.transform.Find("BuildingMode").GetChild(1).gameObject.SetActive(false);
                this.transform.Find("BuildingMode").GetChild(2).gameObject.SetActive(false);

                Debug.LogWarning("TURNED ON TOOLBAR");
            }
        }
        // testing

        if (ON)
        {
            if (isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (primaryWeapon != null)
                    {
                        EquipWeapon(WeaponSlot.Primary);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (secondaryWeapon != null)
                    {
                        EquipWeapon(WeaponSlot.Secondary);
                    }
                }

                if (Input.GetMouseButton(0) && currentWeapon != null)
                {
                    currentWeapon.AttemptUseWeapon();
                }

                if (Input.GetKeyDown(pickupKey) && availablePickup != null)
                {
                    SwapWeapon(availablePickup.weapon);
                    availablePickup.Take();
                }
            }
        }
    }

    // Runs at end of every frame. Used for UI
    void LateUpdate()
    {
        if (isLocalCache)
        {
            DisplayClosestPickupInRange();
        }
    }



    public void SwapWeapon(PlayerWeapon _newWeapon)
    {
        if (secondaryWeapon == null)
        {
            secondaryWeapon = _newWeapon;
        }
        else
        {
            switch (currentWeaponSlot)
            {
                case WeaponSlot.Primary:
                    primaryWeapon = _newWeapon;
                    break;
                case WeaponSlot.Secondary:
                    secondaryWeapon = _newWeapon;
                    break;
            }
        }

        currentWeapon = secondaryWeapon;
        ChangeCurrentVisual();

        Debug.LogWarning("Swapped weapon for " + _newWeapon.ToString());
    }

    public void EquipWeapon(WeaponSlot _slot)
    {
        switch (_slot)
        {
            case WeaponSlot.Primary:
                if (primaryWeapon != null)
                {
                    currentWeapon = primaryWeapon;
                    currentWeaponSlot = WeaponSlot.Primary;
                    print("Primary equipped");
                    ChangeCurrentVisual();
                }
                break;
            case WeaponSlot.Secondary:
                if (secondaryWeapon != null)
                {
                    currentWeapon = secondaryWeapon;
                    currentWeaponSlot = WeaponSlot.Secondary;
                    print("Secondary equipped");
                    ChangeCurrentVisual();
                }
                break;
        }
    }



    void ChangeCurrentVisual()
    {
        if (currentWeapon == null) { Debug.Log("returning early"); return; }
        
        switch (currentWeapon.handheld)
        {
            case Handheld.Fists:
                barrel.parent.gameObject.SetActive(false);
                barrel = this.transform.Find("heavyAvatar").Find("pelvis").Find("waist").GetChild(0).Find("r_shoulder").GetChild(0).GetChild(0).GetChild(0).Find("Fists").GetChild(0);
                barrel.parent.gameObject.SetActive(true);
                break;
            case Handheld.Chaingun:
                barrel.parent.gameObject.SetActive(false);
                barrel = this.transform.Find("heavyAvatar").Find("pelvis").Find("waist").GetChild(0).Find("r_shoulder").GetChild(0).GetChild(0).GetChild(0).Find("Chaingun").GetChild(0);
                barrel.parent.gameObject.SetActive(true);
                break;
            case Handheld.RocketLauncher:
                barrel.parent.gameObject.SetActive(false);
                barrel = this.transform.Find("heavyAvatar").Find("pelvis").Find("waist").GetChild(0).Find("r_shoulder").GetChild(0).GetChild(0).GetChild(0).Find("RocketLauncher").GetChild(0);
                barrel.parent.gameObject.SetActive(true);
                break;
            default: goto case Handheld.Fists;
        }
    }



    /// <summary>
    /// Shoots a networked raycast bullet
    /// </summary>
    public void ShootBullet()
    {
        Vector3 from = barrel.position;
        Vector3 to = GetAim();

        CmdShootBullet(from, to);
    }
    // Step 1, send a command from client to server. Cmd is run on server
    [Command]
    public void CmdShootBullet(Vector3 from, Vector3 to)
    {
        RpcShootBullet(from, to);
    }
    // Step 2, send an RPC from the server. Rpc is run on the server only and is replicated to all clients
    [ClientRpc]
    public void RpcShootBullet(Vector3 from, Vector3 to)
    {
        BulletRaycast(from, to, currentWeapon.damage, currentWeapon.spread);
    }



    public void LaunchBullet()
    {
        Vector3 from = barrel.position;
        Vector3 to = GetAim();

        CmdLaunchBullet(from, to);
    }
    // Step 1, send a command from client to server. Cmd is run on server
    [Command]
    public void CmdLaunchBullet(Vector3 from, Vector3 to)
    {
        RpcLaunchBullet(from, to);
    }
    // Step 2, send an RPC from the server. Rpc is run on the server only and is replicated to all clients
    [ClientRpc]
    public void RpcLaunchBullet(Vector3 from, Vector3 to)
    {
        BulletLaunch(from, to, currentWeapon.damage, currentWeapon.special_damage, currentWeapon.projectile, currentWeapon.timer);
    }



    /// <summary>
    /// Gets the point that the cursor of this player is aiming at
    /// </summary>
    public Vector3 GetAim()
    {
        Vector3 endPoint = Vector3.zero;
        if (Camera.main)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            LayerMask lyrMask = 1 << 9;
            lyrMask = ~lyrMask;
            if (Physics.Raycast(ray, out hit, 999f, lyrMask))
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

    /// <summary> Shoots a bullet in raycast form </summary>
    public void BulletRaycast(Vector3 from, Vector3 to, int damage, float spread = 0f, float maxDistance = 999f)
    {
        bool fake = false;
        if (!isLocalPlayer) { fake = true; }

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
            endPoint = hit.point;
            if (hit.transform.root.GetComponent<HealthController>() && hit.transform.root.GetComponent<PlayerController>()==null && !fake)
            {
                //print(hit.transform.root);
                hit.transform.root.GetComponent<HealthController>().TakeDamage(damage, netId);
            }
        }
        else
        {
            // No hit.point to pull from, calculate where the bullet would have gone to
            Vector3 theoretical = from + (direction * maxDistance);
            endPoint = theoretical;
        }

        GameObject bullet = Instantiate(bulletPrefab, from, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetAttributes(from, endPoint);
    }

    /// <summary> Launches a projectile </summary>
    public void BulletLaunch(Vector3 from, Vector3 to, int damage, int damage_special, ProjectileType projectile, float timer, float maxDistance = 999f)
    {
        if (!isLocalPlayer) { return; }

        Vector3 direction = (to - from);

        switch (projectile)
        {
            case ProjectileType.Rocket:
                GameObject rocket = Instantiate(Resources.Load<GameObject>("Rocket"), from, Quaternion.LookRotation(direction, Vector3.up));
                rocket.GetComponent<Rocket>().SetAttributes(damage_special, timer, netId);
                break;
            case ProjectileType.Floater:
                GameObject floater = Instantiate(Resources.Load<GameObject>("Floater"), from, Quaternion.LookRotation(direction, Vector3.up));
                floater.GetComponent<Floater>().SetAttributes(damage_special, timer, netId);
                break;
            default: goto case ProjectileType.Rocket;
        }
    }

    /// <summary>
    /// Shows a pickup action for the closest pickup within range
    /// </summary>
    void DisplayClosestPickupInRange()
    {
        if (!pickupUI) { return; }

        // Check for pickups in range
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");

        if (pickups.Length > 0)
        {
            GameObject closestPickup = null;

            foreach (GameObject pickup in pickups)
            {
                if (Vector3.Distance(this.transform.position, pickup.transform.position) < pickupRange)
                {
                    if (closestPickup != null)
                    {
                        if (Vector3.Distance(pickup.transform.position, this.transform.position)
                            < Vector3.Distance(closestPickup.transform.position, this.transform.position))
                        {
                            closestPickup = pickup;
                        }
                    }
                    else
                    {
                        closestPickup = pickup;
                    }
                }
            }

            if (closestPickup != null)
            {
                availablePickup = closestPickup.GetComponent<Pickup>();
            }
            else
            {
                availablePickup = null;
            }
        }
        else
        {
            availablePickup = null;
        }

        // Show visual
        if (availablePickup != null)
        {
            if (!pickupUI.activeSelf)
            {
                pickupUI.SetActive(true);
            }

            Vector2 screenPosition = Camera.main.WorldToScreenPoint(availablePickup.transform.position);
            pickupUI.transform.position = Vector3.Lerp(pickupUI.transform.position, screenPosition, 30f * Time.deltaTime);
        }
        else
        {
            if (pickupUI.activeSelf)
            {
                pickupUI.SetActive(false);
            }
        }
    }

}
