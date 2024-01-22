using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Entity
{
    [Header("Star Variables")]
    [SerializeField] float startingLength;
    [SerializeField] float lengthPerUnitSpeed;

    // Update is called once per frame
    override public void FixedUpdate()
    {
        base.FixedUpdate();

        float xScale = transform.localScale.x;
        float yScale = startingLength + gameManager.GetCameraSpeed() * lengthPerUnitSpeed;
        float zScale = transform.localScale.z;

        transform.localScale = new Vector3(xScale, yScale, zScale);
    }

    // Override the generic code to operate on the object's transform rather than its rigidbody
    // Its a bit messy but it runs a lot better
    override public void ApplyMovementVector(Vector3 movementVector)
    {
        transform.Translate(movementVector);
    }

    override public bool IsOutOfBounds()
    {
        return (transform.position.y + transform.lossyScale.y / 2 < -gameManager.cameraBounds.y) ||
            (Mathf.Abs(transform.position.x) - transform.lossyScale.x / 2 > gameManager.cameraBounds.x);
    }

    override public void DestroyEntity()
    {
        transform.position = Vector2.zero;
        pool.Release(gameObject);
    }
}
