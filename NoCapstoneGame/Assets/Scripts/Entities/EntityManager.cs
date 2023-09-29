using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;
using Random=UnityEngine.Random;


public class EntityManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] public ObjectPool<GameObject> objectPool;

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

        objectPool = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject go = SpawnEntity(Random.Range(spawnRange.x, spawnRange.y), spawnHeight);
                //Debug.Log("--SPAWN entity called from object pool");
                return go;
            },
            actionOnGet: (obj) => { 
                obj.SetActive(true); 
                //Debug.Log("GET entity called from object pool for " + obj.name); 
            },
            actionOnRelease: (obj) => { 
                obj.SetActive(false); 
                SetVariables(obj.GetComponent<Entity>()); 
            },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 75,
            maxSize: 150
            );

        StartGenerating();
    }

    public void StartGenerating()
    {
        StartCoroutine(GenerateEntities());
    }

    public void StopGenerating()
    {
        StopCoroutine(GenerateEntities());
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


            //SpawnEntity(iterSpawn, spawnHeight);
            GameObject entity;
            entity = objectPool.Get();
            entity.transform.position = new Vector3(iterSpawn, spawnHeight, 0);
  
            generationTime = iterGenerationTime;
        }
    }

    protected void GenerateAmountOnPoint(int amount, Vector3 point)
    {
        generatingEntities = true;
        for(int i = 0; i < amount; i++)
        {
            GameObject entity;
            entity = objectPool.Get();
            entity.transform.position = point;
        }
    }

    virtual public GameObject SpawnEntity(float spawnX, float spawnY)
    {
        GameObject gameObject = Instantiate(entityPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
        Entity entity = gameObject.GetComponent<Entity>();
        SetVariables(entity);
        entity.entityManager = this;
        return gameObject;
    }

    virtual public void SetVariables(Entity entity)
    {
        //Debug.Log("set variables called as BASE CLASS");
        //Movement
        float iterDownSpeed = Random.Range(downSpeedRange.x, downSpeedRange.y);

        float iterStepSpeed = Random.Range(stepSpeedRange.x, stepSpeedRange.y);
        float iterDirectionAngle = Random.Range(directionAngleRange.x, directionAngleRange.y);

        //Wobble
        float iterSwaySpeed = Random.Range(swaySpeedRange.x, swaySpeedRange.y);
        float iterSwayWidth = Random.Range(swayWidthRange.x, swayWidthRange.y);
        entity.setVariables(iterDownSpeed, iterStepSpeed, iterDirectionAngle, iterSwaySpeed, iterSwayWidth);
    }
}
