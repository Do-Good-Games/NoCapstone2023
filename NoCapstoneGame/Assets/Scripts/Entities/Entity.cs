using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D entityBody;

    [Tooltip("The speed of the entity AWAY from the player, lowering this value increases the relative speed")]
    [SerializeField] public float upwardsSpeed;

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
        this.gameManager = GameManager.Instance;
        gameManager.OnFiredChange.AddListener(UpdateSpeed);
    }

    virtual public void setVariables(float upwardsSpeed, float swaySpeed, float swayWidth)
    {
        this.upwardsSpeed = upwardsSpeed;
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
        float cameraSpeed = gameManager.GetCameraSpeed();
        float cameraDistance = cameraSpeed * Time.deltaTime;

        float entityDistance = upwardsSpeed * Time.deltaTime;

        float swayAmount = swaySpeed * Time.deltaTime;
        float swayOffset = swayAmount * Mathf.Cos(swaySpeed * Time.fixedTime) * swayAmount; //I dont think the second swaySpeed bit makes a ton of sense, but I'm leaving it so everything performs the same

        Vector2 movementVector = new Vector3(swayOffset,entityDistance - cameraDistance);
        entityBody.position += movementVector;

        //OOB check
        if ((entityBody.position.y < -gameManager.cameraBounds.y) || (Mathf.Abs(entityBody.position.x) > gameManager.cameraBounds.x))
        {
            DestroyEntity();
        }
    }

    /*
    virtual public void Move()
    {
        Vector3 oldPos = entityBody.transform.position;//store the current position of the entity

        gmSpeed = gameManager.GetScaledSpeed() - gameManager.startingSpeedUnscaled * gameManager.GetSpeedScale();

        if ((oldPos.y < -15) || (Mathf.Abs(oldPos.x) > 40))
        {
            DestroyEntity();
        }

        float swayScale = swayWidth * Mathf.Cos(swaySpeed * Time.fixedTime) * swaySpeed;//convert the current time and sway variables into an oscillating value from 1 to -1
        //we use cos rather than sin because this is the amoutn we SCALE the sideways vector, not the offset itself. starting at 1 means we start the loop moving at fulls peed to the left from zero

        Vector3 speedVector = new Vector3(0, -gmSpeed, 0);

        //set the new position to the old position, plus the vector representing the overall direction in which we are going
        Vector3 newPos = oldPos + directionVector + (perpVector * swayScale);

        //newPos.y -= (gmSpeed * gameManager.GetSpeedScale());

        entityBody.MovePosition(newPos + speedVector);
    }
    */

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
