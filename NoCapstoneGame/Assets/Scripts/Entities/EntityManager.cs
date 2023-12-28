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

    //[SerializeField] public Dictionary<GameObject, float> entityPrefabs;
    [SerializeField] protected List<EntityOption> entityOptions;

    [Header("Generation timers and probabilities")]
    [Tooltip("The amount of asteroids per unscaled unit")]
    [SerializeField] private float density;

    [Tooltip("the minimum and maximum possible variation to generation time")]
    [SerializeField] private Vector2 generationTimeRange;

    [SerializeField] private bool generatingEntities;

    [Header("per entity variables")]

    [Header("Movement")]
    [SerializeField] public Vector2 upwardsSpeedRange;

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
            //wait for the length of time set by generationTime - until then do nothing
            float generationTime = GetGenerationTime();
            yield return new WaitForSeconds(generationTime);

            //spawning
            float iterSpawn = Random.Range(spawnRange.x, spawnRange.y);

            //SpawnEntity(iterSpawn, spawnHeight);
            GameObject entity;
            entity = objectPool.Get();
            entity.transform.position = new Vector3(iterSpawn, spawnHeight, 0);
        }
    }

    protected float GetGenerationTime()
    {
        float generationTime = 1/((gameManager.GetCameraSpeed()) * density);
        //Debug.Log(gameManager.GetCameraSpeed() + "," + generationTime);
        return generationTime;
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

    virtual public GameObject getEntity()
    {
        float roll = Random.value; // float between 0 and 1, inclusive
        float weightReached = 0;
        foreach (EntityOption option in entityOptions){
            weightReached += option.weight;
            if (roll <= weightReached)
            {
                return option.entity;
            }
        }

        //This should never happen
        Debug.Log("rolled a null entity, weights are fucked up");
        return null;
    }

    virtual public GameObject SpawnEntity(float spawnX, float spawnY)
    {
        GameObject entityPrefab = getEntity();
        GameObject gameObject = Instantiate(entityPrefab, new Vector3(spawnX, spawnY, Random.Range(-.5f, .5f)), Quaternion.identity);
        gameObject.transform.position = new Vector3(spawnX, spawnY, 0);
        Entity entity = gameObject.GetComponent<Entity>();
        SetVariables(entity);
        entity.entityManager = this;
        return gameObject;
    }


    virtual public void SetVariables(Entity entity)
    {
        //Movement
        float iterUpSpeed = Random.Range(upwardsSpeedRange.x, upwardsSpeedRange.y);

        //Wobble
        float iterSwaySpeed = Random.Range(swaySpeedRange.x, swaySpeedRange.y);
        float iterSwayWidth = Random.Range(swayWidthRange.x, swayWidthRange.y);
        entity.setVariables(iterUpSpeed, iterSwaySpeed, iterSwayWidth);
    }


    [System.Serializable]
    protected class EntityOption
    {
        [SerializeField] public GameObject entity;
        [SerializeField] public float weight;
    }
}
