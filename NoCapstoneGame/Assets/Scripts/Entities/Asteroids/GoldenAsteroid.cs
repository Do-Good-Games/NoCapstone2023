using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Pool;

public class GoldenAsteroid : Asteroid
{
    [Header("Golden Asteroid Values")]
    [SerializeField] private Vector2 extraDropsRange;
    [SerializeField] private Vector2 extraHealthRange;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void SetVariables(ObjectPool<GameObject> pool, float upwardsSpeed, float swaySpeed, float swayWidth)
    {
        base.SetVariables(pool, upwardsSpeed, swaySpeed, swayWidth);
        int addDrops = (int)Random.Range(extraDropsRange.x, extraDropsRange.y);
        numDrops += addDrops;

        int addHealth = (int)Random.Range(extraHealthRange.x, extraHealthRange.y);
        maxHealth += addHealth;
        currentHealth += addHealth;
    }
}
