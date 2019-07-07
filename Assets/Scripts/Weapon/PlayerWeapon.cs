using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Handheld { Fists, Chaingun, RocketLauncher }

public enum WeaponType { Raycast, Projectile, Melee }

public enum ProjectileType { Rocket, Floater }

/// <summary>
/// Handles the variables and attacks of all weapons, is fed actions from Toolbar
/// </summary>
public class PlayerWeapon
{

    public WeaponType weaponType;

    public Handheld handheld = Handheld.Fists;


    public Toolbar tb;

    public int damage = 0;
    public int special_damage = 0;
    public float firerate = 0f;
    public float spread = 0f;
    public float cooldown = 0f;
    public float length = 0f;
    public ProjectileType projectile;
    public float timer = 0f;

    float lastUseWeapon = 0f;



    public void AttemptUseWeapon()
    {
        if (weaponType == WeaponType.Raycast)
        {
            if (Time.time > lastUseWeapon + (1 / firerate))
            {
                lastUseWeapon = Time.time;
                UseWeapon();
            }
        }
        if (weaponType == WeaponType.Projectile)
        {
            if (Time.time > lastUseWeapon + (1 / firerate))
            {
                lastUseWeapon = Time.time;
                UseWeapon();
            }
        }
        if (weaponType == WeaponType.Melee)
        {
            if (Time.time > lastUseWeapon + cooldown)
            {
                lastUseWeapon = Time.time;
                UseWeapon();
            }
        }
    }

    /// <summary>
    /// Override this to set the primary attack for the weapon
    /// </summary>
    public virtual void UseWeapon()
    {
        if (weaponType == WeaponType.Raycast)
        {
            tb.ShootBullet();
        }
        if (weaponType == WeaponType.Projectile)
        {
            tb.LaunchBullet();
        }
        if (weaponType == WeaponType.Melee)
        {
            Debug.LogWarning("Not done");
        }

        // override to add extra effects
    }



    /// <summary> Holds all required values for a Raycast wep </summary>
    public void SetWeaponRaycast(int _damage, float _firerate, float _spread)
    {
        weaponType = WeaponType.Raycast;

        damage = _damage;
        firerate = _firerate;
        spread = _spread;
    }
    /// <summary> Holds all required values for a Raycast wep </summary>
    public void SetWeaponProjectile(int _damage, float _firerate, float _spread, int _special_damage, ProjectileType _projectile, float _timer)
    {
        weaponType = WeaponType.Projectile;

        damage = _damage;
        firerate = _firerate;
        spread = _spread;
        special_damage = _special_damage;
        projectile = _projectile;
        timer = _timer;
    }
    /// <summary> Holds all required values for a Melee wep </summary>
    public void SetWeaponMelee(int _damage, float _length, float _cooldown)
    {
        weaponType = WeaponType.Melee;

        damage = _damage;
        length = _length;
        cooldown = _cooldown;
    }

}
