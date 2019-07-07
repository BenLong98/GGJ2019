using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : PlayerWeapon
{
    public RocketLauncher()
    {
        SetWeaponProjectile(110, 0.5f, 0f, 110, ProjectileType.Rocket, 3f);
        handheld = Handheld.RocketLauncher;
    }
}
