using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
	[SerializeField] private Rigidbody2D asteroidBody;
	
	[Header("Movement")]
	[SerializeField] public float downSpeed;
	[SerializeField] public float swaySpeed;
	[SerializeField] public float swayWidth;
	[SerializeField] public float direction;

	[Header("Interaction")] 
	[SerializeField] public float health;
	[SerializeField] public string laserTag;

    // Update is called once per frame
    void Update()
    {
	    Move();
    }

    public void Move()
    {
	    Vector3 oldPos = asteroidBody.transform.position;
	    
	    Vector3 newPos = new Vector3( oldPos.x,(oldPos.y - downSpeed), oldPos.z );

	    
	    asteroidBody.MovePosition(newPos);
    }

    public bool Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy();
            return true;
        }
        return false;
    }

    public void Destroy()
    {
        // Code regarding destruction animations and energy drops goes here
        Destroy(this.gameObject);
    }
}
