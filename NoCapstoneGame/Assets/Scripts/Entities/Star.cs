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
}
