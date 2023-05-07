using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
	[SerializeField] private Rigidbody2D asteroidBody;
	
	[Header("Movement")]
	[SerializeField] public float downSpeed;
    [Tooltip("general movement speed of each asteroid, recommended range approx. .1")]
	[SerializeField] public float moveSpeed; //technically used as the hypoteneuse of the triangle used to calculate movement
    [Tooltip("the direction of movement by the asteroid - represented as an angle from -90 to 90, 0 = straight down - set relative to directionAngle")]
    [SerializeField] public float directionAngle;
    [Tooltip("the vector representing the direction the asteroid is moving in")]
    private Vector3 directionVector;
    [Tooltip("vector perpendicular to the direction vector - used to calculate wobble")]
    private Vector3 perpVector;

    [Header("wobble")]
    [Tooltip("the frequency by which each sway repeats. higher = more rapid movement back and forth. scale of ~ 1 -10 ")]
	[SerializeField] public float swaySpeed;
    [Tooltip("how far side to side the asteroid will sway - scaled down by two orders of magnitude to make it more intuitive to work with. scale of say .3-2")]
	[SerializeField] public float swayWidth;

	[Header("Interaction")] 
	[SerializeField] public float health;
	[SerializeField] public string laserTag;


    void Start()
    {

        //setting variables for direction triangle calculations 
        float directionRadians = directionAngle * Mathf.Deg2Rad;//convert the direction angle into radians
        float downMovementAmount = Mathf.Sin(directionRadians) * moveSpeed; //calculated as the adjacent side of triangle/ y coord
        float sidewaysMovementAmount = Mathf.Cos(directionRadians) * moveSpeed * downSpeed; // calculated as the opposite side of triangle/ x coord
        
        directionVector = new Vector3(-downMovementAmount, -sidewaysMovementAmount, 0);// vector representing the direction the ship will move in

        perpVector = new Vector3(directionVector.y, -directionVector.x, 0); //perpendicular vector for calculation with wobble
        Debug.Log("perpendicular vector" + perpVector);//returns (-.5, -.87,0)

    }

    // Update is called once per frame
    void Update()
    {
	    
    }

    //physics stuff
    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        
        //if we reach a point where the downward speed of the asteroid will vary (such as with an increasing variable modifier, then we'll want to recalculate these every update
        //for now they only need to happen once, so for performance that's as often as they'll happen
        /*
        //setting variables for direction triangle calculations 
        float directionRadians = directionAngle * Mathf.Deg2Rad;//convert the direction angle into radians
        float downMovementAmount = Mathf.Sin(directionRadians) * moveSpeed; //calculated as the adjacent side of triangle/ y coord
        float sidewaysMovementAmount = Mathf.Cos(directionRadians) * moveSpeed * downSpeed; // calculated as the opposite side of triangle/ x coord

        directionVector = new Vector3(-downMovementAmount, -sidewaysMovementAmount, 0);// vector representing the direction the ship will move in

        perpVector = new Vector3(directionVector.y, -directionVector.x, 0); //perpendicular vector for calculation with wobble
        Debug.Log("perpendicular vector" + perpVector);//returns (-.5, -.87,0)*/



        Vector3 oldPos = asteroidBody.transform.position;//store the current position of the asteroid
        float swayScale = swayWidth* Mathf.Cos(swaySpeed * Time.fixedTime) * swaySpeed;//convert the current time and sway variables into an oscillating value from 1 to -1
        //we use cos rather than sin because this is the amoutn we SCALE the sideways vector, not the offset itself. starting at 1 means we start the loop moving at fulls peed to the left from zero

        //set the new position to the old position, plus the vector representing the overall direction in which we are going
        Vector3 newPos = oldPos + directionVector + ( perpVector * swayScale);

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
