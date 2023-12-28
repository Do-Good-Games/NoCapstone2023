using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GoldenAsteroid : Asteroid
{
    [Header("Golden Asteroid Values")]
    [SerializeField] private Vector2 extraDropsRange;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        numDrops += (int) Random.Range(extraDropsRange.x, extraDropsRange.y);
    }
}
