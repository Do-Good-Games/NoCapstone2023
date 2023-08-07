using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : EntityManager
{
    [SerializeField] public EntityManager energyManager;

    [Header ("Asteroid-specific variables")]
    [Tooltip("range for the health of the entity")]
    [SerializeField] public Vector2 healthRange;
    [Tooltip("The size range of the asteroid. the size of each asteroid is dependent on the health value, with the highest health becoming the largest size")]
    [SerializeField] public Vector2 sizeRange;
    [Tooltip("The range for the base number of drops, mapped linearly by size")]
    [SerializeField] public Vector2 baseNumDropsRange;
    [Tooltip("The variance randomly added to the base number of drops")]
    [SerializeField] public Vector2 numDropsVarianceRange;

    private void Awake()
    {
        // Type check
        if (entityPrefab.GetComponent<Asteroid>() == null){
            Debug.LogError(entityPrefab + " has no Asteroid component!");
            Destroy(this.gameObject);
        }
    }

    public override void Start()
    {
        base.Start();
        StartGenerating();
    }

    override public void SetVariables(Entity entity)
    {
        //Debug.Log("set variables called as INHERITED CLASS (asteroid");
        base.SetVariables(entity);
        float iterHealth = Random.Range(healthRange.x, healthRange.y);
        // Maps iterHealth to the size range based on the health range
        float iterSize = Mathf.Lerp(sizeRange.x, sizeRange.y, Mathf.InverseLerp(healthRange.x, healthRange.y, iterHealth));

        int iterBaseNumDrops = (int) Mathf.Lerp(baseNumDropsRange.x, baseNumDropsRange.y, Mathf.InverseLerp(sizeRange.x, sizeRange.y, iterSize));
        int iterNumDrops = iterBaseNumDrops + (int) Random.Range(numDropsVarianceRange.x, numDropsVarianceRange.y);
        ((Asteroid) entity).setVariables(iterHealth, iterSize, energyManager, iterNumDrops);
    }
}
