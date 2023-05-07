using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
	[SerializeField] private Rigidbody2D asteroidBody;
	
	[Header("Movement")]
	[SerializeField] public float downSpeed;
    [Tooltip("general movement speed of each asteroid, recommended range approx. .1")]
	[SerializeField] public float moveSpeed; //technically used as the hypoteneuse of the triangle used to calculate movement
	[SerializeField] public float swaySpeed;
	[SerializeField] public float swayWidth;
    [Tooltip("the direction of movement by the asteroid - represented as an angle from -90 to 90, 0 = straight down")]
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
        float directionRadians = direction * Mathf.Deg2Rad;
        Debug.Log("direction in radians: " + directionRadians);
        Vector3 oldPos = asteroidBody.transform.position;
        float downMovementAmount = Mathf.Sin(directionRadians) * moveSpeed; //calculated as the adjacent side of triangle/ y coord
        float sidewaysMovementAmount = Mathf.Cos(directionRadians) * moveSpeed * downSpeed; // calculated as the opposite side of triangle/ x coord
        Debug.Log("downward movement: " + downMovementAmount);
        Vector3 anglePos = new Vector3();

        Vector3 newPos = new Vector3( (oldPos.x - downMovementAmount),(oldPos.y - sidewaysMovementAmount), oldPos.z );

	    
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
