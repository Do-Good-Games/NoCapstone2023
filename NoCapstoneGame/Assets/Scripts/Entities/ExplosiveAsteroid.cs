using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAsteroid : Asteroid
{
    [Header("Explosive Asteroid Values")]
    [SerializeField] GameObject explosionPrefab;

    public override void DestroyAsteroid()
    {
        //take current position and place an explosion object here
        GameObject explosion = GameObject.Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);

        base.DestroyAsteroid();
    }
}
