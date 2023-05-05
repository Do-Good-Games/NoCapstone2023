using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Camera mainCamera;
    [SerializeField] string hazardLayer;

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        // converts cursor position (in screen space) to world space based on camera position/size
        Vector2 cursorPos = context.ReadValue<Vector2>();
        Vector2 position = mainCamera.ScreenToWorldPoint(cursorPos);
        playerBody.transform.position = position;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(hazardLayer))
        {
            //Placeholder, should notify game manager
            Debug.Log("Player hit");
        }
    }
}
