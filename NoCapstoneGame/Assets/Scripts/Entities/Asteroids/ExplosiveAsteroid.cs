using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAsteroid : Asteroid
{
    [Header("Explosive Asteroid Values")]
    [SerializeField] Explosion explosionPrefab;

    [SerializeField] float sizeScale = 1.3f;

    public override void DestroyAsteroid()
    {
        this.numDrops = 0;
        //take current position and place an explosion object here
        Explosion explosion = GameObject.Instantiate<Explosion>(explosionPrefab, this.transform.position, this.transform.rotation);
        //Explosion explosion = (Explosion)GameObject.Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);

        explosion.transform.localScale = new Vector2(this.size * sizeScale, this.size * sizeScale);
        //explosion.transform.localScale.Set(this.size, this.size, 1);

        //set the appropriate animation
        explosion.index = spriteIndex;

        Debug.Log("check1 destroying asteroid");
        base.DestroyAsteroid();
    }
}
