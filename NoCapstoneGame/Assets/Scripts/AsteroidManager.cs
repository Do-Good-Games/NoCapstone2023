using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;


public class AsteroidManager : MonoBehaviour
{
    GameManager gameManager;


    [SerializeField] public GameObject asteroidPrefab;
       
    [Header("Generation timers and probabilities")]
    [Tooltip("the amount of time between when asteroids generate")]
    [SerializeField] private float generationTime;
    [Tooltip("the minimum and maximum possible generation time")]
    [SerializeField] private Vector2 generationTimeRange;

    [SerializeField] private bool generatingAsteroids;

    [Header("per asteroid variables")]

    [Header("Movement")]
    [SerializeField] public Vector2 downSpeedRange;
    [Tooltip("general movement speed of each asteroid, recommended range approx. .1")]
    [SerializeField] public Vector2 stepSpeedRange; //technically used as the hypoteneuse of the triangle used to calculate movement
    [Tooltip("the direction of movement by the asteroid - represented as an angle from -90 to 90, 0 = straight down - set relative to directionAngle")]
    [SerializeField] public Vector2 directionAngleRange;

    [Header("wobble")]
    [Tooltip("the frequency by which each sway repeats. higher = more rapid movement back and forth. scale of ~ 1 -10 ")]
    [SerializeField] public Vector2 swaySpeedRange;
    [Tooltip("how far side to side the asteroid will sway - scaled down by two orders of magnitude to make it more intuitive to work with. scale of say .3-2")]
    [SerializeField] public Vector2 swayWidthRange;
    [Tooltip("range for the health of the asteroid")]
    [SerializeField] public Vector2 healthRange;


    Vector2 spawnRange;
    float spawnHeight;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        spawnRange = new Vector2(-gameManager.cameraBounds.x, gameManager.cameraBounds.x);
        spawnHeight = gameManager.cameraBounds.y + 1;
        StartCoroutine(GenerateAsteroids());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GenerateAsteroids()
    {
        while (generatingAsteroids)
        {
            //wait for the length of time set by generationTime - untill then do nothing
            float counter = 0f;
            while (counter < generationTime)//this feels bad tbh - consider running this by one of the others
            {
                counter += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            //[Header("per asteroid variables")]
            // [Header("Movement")]
            //[SerializeField] public Vector2 downSpeedRange;
            //[SerializeField] public Vector2 stepSpeedRange; //technically used as the hypoteneuse of the triangle used to calculate movement
            //[SerializeField] public Vector2 directionAngleRange;

            //[Header("wobble")]
            //[SerializeField] public Vector2 swaySpeedRange;
            //[SerializeField] public Vector2 swayWidthRange;

            float iterGenerationTime = Random.Range(generationTimeRange.x, generationTimeRange.y);

            //per asteroid variables

            //spawning
            float iterSpawn = Random.Range(spawnRange.x, spawnRange.y);

            //movement
            float iterDownSpeed = Random.Range(downSpeedRange.x, downSpeedRange.y);

            float iterStepSpeed = Random.Range(stepSpeedRange.x, stepSpeedRange.y);
            float iterDirectionAngle = Random.Range(directionAngleRange.x, directionAngleRange.y);

            //[Header("wobble")]
            float iterSwaySpeed = Random.Range(swaySpeedRange.x, swaySpeedRange.y);
            float iterSwayWidth= Random.Range(swayWidthRange.x, swayWidthRange.y);

            float iterHealth = Random.Range(healthRange.x, healthRange.y);

            GameObject gameObject =  Instantiate(asteroidPrefab, new Vector3(iterSpawn, spawnHeight, 0), Quaternion.identity);
            Asteroid asteroid = gameObject.GetComponent<Asteroid>();

            asteroid.setVariables(iterDownSpeed, iterStepSpeed, iterDirectionAngle, iterSwaySpeed, iterSwayWidth, iterHealth);
            generationTime = iterGenerationTime;


        }
    }
}
