using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Camera mainCamera;

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        Vector2 cursorPos = context.ReadValue<Vector2>();
        Vector2 position = mainCamera.ScreenToWorldPoint(cursorPos);
        playerBody.transform.position = position;
    }
}
