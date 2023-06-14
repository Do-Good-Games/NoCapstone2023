using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : EntityManager
{
    [Header ("Asteroid-specific variables")]
    [Tooltip("range for the health of the entity")]
    [SerializeField] public Vector2 healthRange;

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
        ((Asteroid) entity).setVariables(healthRange);
    }
}
