using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{

    public bool init = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
