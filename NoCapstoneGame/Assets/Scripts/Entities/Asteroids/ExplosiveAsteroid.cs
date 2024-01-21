using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAsteroid : Asteroid
{
    [Header("Explosive Asteroid Values")]
    [SerializeField] GameObject explosionPrefab;

    public override void DestroyAsteroid()
    {
        this.numDrops = 0;
        //take current position and place an explosion object here
        GameObject explosion = GameObject.Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
        //Explosion explosion = (Explosion)GameObject.Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);

        base.DestroyAsteroid();
    }
}
