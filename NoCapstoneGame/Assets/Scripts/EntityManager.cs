using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random=UnityEngine.Random;


public class EntityManager : MonoBehaviour
{
    GameManager gameManager;


    [SerializeField] public GameObject entityPrefab;
       
    [Header("Generation timers and probabilities")]
    [Tooltip("the amount of time between when entities generate")]
    [SerializeField] private float generationTime;
    [Tooltip("the minimum and maximum possible generation time")]
    [SerializeField] private Vector2 generationTimeRange;

    [SerializeField] private bool generatingEntities;

    [Header("per entity variables")]

    [Header("Movement")]
    [SerializeField] public Vector2 downSpeedRange;
    [Tooltip("general movement speed of each entity, recommended range approx. .1")]
    [SerializeField] public Vector2 stepSpeedRange; //technically used as the hypoteneuse of the triangle used to calculate movement
    [Tooltip("the direction of movement by the entity - represented as an angle from -90 to 90, 0 = straight down - set relative to directionAngle")]
    [SerializeField] public Vector2 directionAngleRange;

    [Header("wobble")]
    [Tooltip("the frequency by which each sway repeats. higher = more rapid movement back and forth. scale of ~ 1 -10 ")]
    [SerializeField] public Vector2 swaySpeedRange;
    [Tooltip("how far side to side the entity will sway - scaled down by two orders of magnitude to make it more intuitive to work with. scale of say .3-2")]
    [SerializeField] public Vector2 swayWidthRange;


    Vector2 spawnRange;
    float spawnHeight;

    // Start is called before the first frame update
    virtual public void Start()
    {
        gameManager = GameManager.Instance;
        spawnRange = new Vector2(-gameManager.cameraBounds.x, gameManager.cameraBounds.x);
        spawnHeight = gameManager.cameraBounds.y + 1;
        StartCoroutine(GenerateEntities());
    }

    protected IEnumerator GenerateEntities()
    {
        generatingEntities = true;
        while (generatingEntities)
        {
            //wait for the length of time set by generationTime - untill then do nothing
            yield return new WaitForSeconds(generationTime);

            float iterGenerationTime = Random.Range(generationTimeRange.x, generationTimeRange.y);


            //spawning
            float iterSpawn = Random.Range(spawnRange.x, spawnRange.y);

            GameObject gameObject =  Instantiate(entityPrefab, new Vector3(iterSpawn, spawnHeight, 0), Quaternion.identity);
            Entity entity = gameObject.GetComponent<Entity>();
            SetVariables(entity);
  
            generationTime = iterGenerationTime;
        }
    }



    virtual protected void SetVariables(Entity entity)
    {
        entity.setVariables(downSpeedRange, stepSpeedRange, directionAngleRange, swaySpeedRange, swayWidthRange);
    }
}
