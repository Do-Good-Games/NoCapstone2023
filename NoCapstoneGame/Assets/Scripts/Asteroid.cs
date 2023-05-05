using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
	[SerializeField] private Rigidbody2D asteroidBody;
	
	[Header("movement")]
	[SerializeField] public float downSpeed;
	[SerializeField] public float swaySpeed;
	[SerializeField] public float swayWidth;
	[SerializeField] public float direction;

	[Header("player interaction")] 
	[SerializeField] public float health;
	
	
	

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    Move();
	    
    }
    
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
	    Debug.Log(("Ian do ya thang"));
    }

    public void Move()
    {
	    Vector3 oldPos = asteroidBody.transform.position;
	    
	    Vector3 newPos = new Vector3( oldPos.x,(oldPos.y - downSpeed), oldPos.z );

	    
	    asteroidBody.MovePosition(newPos);
    }
}
