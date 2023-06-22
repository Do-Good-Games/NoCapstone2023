using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : EntityManager
{
    [Header ("Asteroid-specific variables")]
    [Tooltip("range for the health of the entity")]
    [SerializeField] public Vector2 healthRange;
    [Tooltip("The size range of the asteroid. the size of each asteroid is dependent on the health value, with the highest health becoming the largest size")]
    [SerializeField] public Vector2 sizeRange;

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

    override protected void SetVariables(Entity entity)
    {
        base.SetVariables(entity);
        float iterHealth = Random.Range(healthRange.x, healthRange.y);
        // Maps iterHealth to the size range based on the health range
        float iterSize = Mathf.Lerp(sizeRange.x, sizeRange.y, Mathf.InverseLerp(healthRange.x, healthRange.y, iterHealth));
        Debug.Log(Mathf.InverseLerp(healthRange.x, healthRange.y, iterHealth));
        Debug.Log(iterSize);
        ((Asteroid) entity).setVariables(iterHealth, iterSize);
    }
}
