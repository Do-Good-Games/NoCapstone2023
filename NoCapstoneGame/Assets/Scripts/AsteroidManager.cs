using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] bool init = false;
       
    [Header("Generation timers and probabilities")]
    [Tooltip("the amount of time between when asteroids generate")]
    [SerializeField] private float generationTime;

    [SerializeField] private bool generatingAsteroids;
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateAsteroids());
    }

    // Update is called once per frame
    void Update()
    {
        if(!init)
        {
            Debug.Log("pretend this spawned an asteroid");
            init = true;
            //spawn asteroid, will later set this by a coroutine 
            //- or maybe at any given point there's a chance of spawning an asteroid
        }
        
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
            
            Debug.Log("here is where we will spawn an asteroid");
        }
    }
}
