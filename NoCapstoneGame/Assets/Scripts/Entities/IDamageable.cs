using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Damage the object, return true if the object was destroyed
    public bool Damage(int damageAmount)
    {
        return false;
    }
}
