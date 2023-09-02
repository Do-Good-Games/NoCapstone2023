  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotProjectile : FadingProjectile
{
    public void SetSize(float size)
    {
        if (projectileCollider as CircleCollider2D == null)
        {
            Debug.Log("JESSE: Charge shot projectile's collider is not of type Circle Collider 2D");
            return;
        }
        projectileRenderer.size = Vector2.one * size;
        ((CircleCollider2D) projectileCollider).radius = (size / 2);
    }
}
