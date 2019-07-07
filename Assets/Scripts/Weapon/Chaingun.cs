using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaingun : PlayerWeapon {
    public Chaingun()
    {
        SetWeaponRaycast(19, 20f, 0.06f);
        handheld = Handheld.Chaingun;
    }
}
