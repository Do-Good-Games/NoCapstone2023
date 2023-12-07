using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D entityBody;

    [Header("Movement")]
    protected float gmSpeed;
    protected float gmSpeedScale;
    [SerializeField] public float downSpeed;
    [Tooltip("general movement speed of each entity, recommended range approx. .1")]
    [SerializeField] public float stepSpeed; //technically used as the hypoteneuse of the triangle used to calculate movement
    [Tooltip("the direction of movement by the entity - represented as an angle from -90 to 90, 0 = straight down - set relative to directionAngle")]
    [SerializeField] public float directionAngle;
    [Tooltip("the vector representing the direction the entity is moving in")]
    [SerializeField] protected Vector3 directionVector;
    [Tooltip("vector perpendicular to the direction vector - used to calculate wobble")]
    protected Vector3 perpVector;

    [Header("wobble")]
    [Tooltip("the frequency by which each sway repeats. higher = more rapid movement back and forth. scale of ~ 1-10 ")]
    [SerializeField] public float swaySpeed;
    [Tooltip("how far side to side the entity will sway - scaled down by two orders of magnitude to make it more intuitive to work with. scale of say .3-2")]
    [SerializeField] public float swayWidth;

    protected GameManager gameManager;
    public EntityManager entityManager;

    // Start is called before the first frame update
    virtual public void Start()
    {
        // setting variables for direction triangle calculations
        float directionRadians = directionAngle * Mathf.Deg2Rad;//convert the direction angle into radians
        float downMovementAmount = Mathf.Sin(directionRadians) * stepSpeed; //calculated as the adjacent side of triangle/ y coord
        float sidewaysMovementAmount = Mathf.Cos(directionRadians) * stepSpeed * downSpeed; // calculated as the opposite side of triangle/ x coord

        directionVector = new Vector3(-downMovementAmount, -sidewaysMovementAmount, 0);// vector representing the direction the ship will move in

        perpVector = new Vector3(directionVector.y, -directionVector.x, 0); //perpendicular vector for calculation with wobble

        this.gameManager = GameManager.Instance;
        gameManager.OnSpeedChange.AddListener(UpdateSpeed);
    }

    virtual public void setVariables(float downSpeed, float stepSpeed, float directionAngle, float swaySpeed, float swayWidth)
    {
        this.downSpeed = downSpeed;
        this.stepSpeed = stepSpeed;
        this.directionAngle = directionAngle;
        this.swaySpeed = swaySpeed;
        this.swayWidth = swayWidth;
        //this.gmSpeed = gameManager.getSpeed();
    }

    // Update is called once per frame
    virtual public void FixedUpdate()
    {
        Move();
    }

    virtual public void Move()
    {
        Vector3 oldPos = entityBody.transform.position;//store the current position of the entity

        gmSpeed = gameManager.GetSpeed() * gameManager.GetSpeedScale();

        

        if ((oldPos.y < -15) || (Mathf.Abs(oldPos.x) > 40))
        {
            DestroyEntity();
        }

        float swayScale = swayWidth * Mathf.Cos(swaySpeed * Time.fixedTime) * swaySpeed;//convert the current time and sway variables into an oscillating value from 1 to -1
        //we use cos rather than sin because this is the amoutn we SCALE the sideways vector, not the offset itself. starting at 1 means we start the loop moving at fulls peed to the left from zero

        Vector3 speedVector = new Vector3(0, gmSpeed, 0);

        //set the new position to the old position, plus the vector representing the overall direction in which we are going
        Vector3 newPos = oldPos + directionVector + (perpVector * swayScale) - speedVector;

        //newPos.y -= (gmSpeed * gameManager.GetSpeedScale());

        entityBody.MovePosition(newPos);
    }

    virtual public void DestroyEntity()
    {
        entityManager.objectPool.Release(gameObject);
        //Destroy(this.gameObject, 0.5f);
    }

    public void UpdateSpeed()
    {
        //directionVector.y = (gmSpeed * gameManager.GetSpeedScale());

    }
}
