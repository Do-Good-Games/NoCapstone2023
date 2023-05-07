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
    [Tooltip("the direction of movement by the asteroid - represented as an angle from -90 to 90, 0 = straight down")]
    public Vector3 directionVector;
    public Vector3 perpVector;
    [SerializeField] public float directionAngle;
    [Header("wobble")]
	[SerializeField] public float swaySpeed;
	[SerializeField] public float swayWidth;
    [Tooltip("the vector representing the direction the asteroid is moving in")]

	[Header("Interaction")] 
	[SerializeField] public float health;
	[SerializeField] public string laserTag;

    [Tooltip("the previous location of the asteroid, PRIOR to sin wave wobble adjustment")]
    Vector3 prevDirectionVector;

    void Start()
    {
        float directionRadians = directionAngle * Mathf.Deg2Rad;
        float downMovementAmount = Mathf.Sin(directionRadians) * moveSpeed; //calculated as the adjacent side of triangle/ y coord
        float sidewaysMovementAmount = Mathf.Cos(directionRadians) * moveSpeed * downSpeed; // calculated as the opposite side of triangle/ x coord

        directionVector = new Vector3(-downMovementAmount, -sidewaysMovementAmount, 0);
        //Debug.Log("direction vector" + directionVector);

        perpVector = new Vector3(directionVector.y, -directionVector.x, 0);
        perpVector.Normalize();
        Debug.Log("perpendicular vector" + perpVector);//returns (-.5, -.87,0)

        //Debug.Log("downward movement: " + downMovementAmount);
    }

    // Update is called once per frame
    void Update()
    {
	    Move();
    }

    public void Move()
    {
        Debug.Log("sinescale: "  + "perpvector: " + perpVector); //returns (0,0,0)
        //Debug.Log("direction in radians: " + directionRadians);
        Vector3 oldPos = asteroidBody.transform.position;

        float sinescale = Mathf.Cos(Time.time);


        Vector3 newPos = oldPos + directionVector + (sinescale * perpVector);
        //perpVector = perpVector * sinescale;
        Debug.Log("sinescale: "  + "perpvector: " + perpVector);

        //prevDirectionVector = newPos;
        //newPos = newPos + perpVector;
	    
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
