using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;


public class AsteroidManager : MonoBehaviour
{
       
    [Header("Generation timers and probabilities")]
    [Tooltip("the amount of time between when asteroids generate")]
    [SerializeField] private float generationTime;
    [Tooltip("maximum number of asteroids that can be spawned in a single wave - numbers greater than 1 cause the random generation code to iterate")]
    [SerializeField] private int maxAsteroids;
    [Tooltip("scale of 0-1 | likelihood that each time we check spawn a single asteroid, that asteroid will spawn ")]
    [SerializeField] private float chanceOfAsteroid;

    [SerializeField] private bool generatingAsteroids;
    
    
    // Start is called before the first frame update
    void Start()
    {
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
            float counter = 0f;
            while (counter < generationTime)
            {
                counter += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            
            Debug.Log("this is when we enter a wave of spawning asteroids");

            float randDivs = 1 / maxAsteroids;
            for (int i = 0; i < maxAsteroids; i++)
            {
                float rand = Random.value;
                //Debug.Log(rand);
                if (rand > chanceOfAsteroid)
                {
                    Debug.Log("we DID spawn an asteroid");
                }
                else
                {
                    Debug.Log("we did NOT spawn an asteroid");
                }
            }

        }
    }
}
