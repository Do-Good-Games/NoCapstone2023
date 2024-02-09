using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ExplosiveAsteroid : Asteroid
{
    [Header("Explosive Asteroid Values")]
    [SerializeField] Explosion explosionPrefab;
    [SerializeField] Vector2 extraHealthrange;

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

        // Debug.Log("check1 destroying asteroid");
        base.DestroyAsteroid();
    }
    public override void SetVariables(ObjectPool<GameObject> pool, float upwardsSpeed, float swaySpeed, float swayWidth)
    {
        base.SetVariables(pool, upwardsSpeed, swaySpeed, swayWidth);
        int addHealth = (int)Random.Range(extraHealthrange.x, extraHealthrange.y);
        maxHealth += addHealth;
        currentHealth += addHealth;

        if (maxHealth <= 0)
        {
            maxHealth = 1;
            currentHealth = 1;
        }
    }

}
