using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Camera mainCamera;
    [SerializeField] string hazardLayer;

    [SerializeField] int maxHealth;

    private GameManager gameManager;
    private LaserSpawner[] spawners;

    public void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.HealPlayer(maxHealth);
        // store all the Laser Spawners components in an array to avoid calling GetComponents() many times
        spawners = GetComponentsInChildren<LaserSpawner>();
    }

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        // converts cursor position (in screen space) to world space based on camera position/size
        Vector2 cursorPos = context.ReadValue<Vector2>();
        Vector2 position = mainCamera.ScreenToWorldPoint(cursorPos);
        playerBody.transform.position = position;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            FireLasers();
        }
    }

    public void FireLasers()
    {
        // tell every spawner to spawn a laser
        foreach (LaserSpawner spawner in spawners)
        {
            spawner.SpawnLaser();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(hazardLayer))
        {
            gameManager.HurtPlayer(1);
            Debug.Log("Player hit");
        }
    }
}
