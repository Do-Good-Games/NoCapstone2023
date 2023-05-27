using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Collider2D playerCollider;
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] string hazardLayer;

    [Header("audio")]
    [SerializeField] AudioSource shootSound;
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource deathSound;

    [Header("Gampeplay variables")]
    [SerializeField] public int maxHealth;

    string currentActionMapName;

    GameManager gameManager;
    private Camera gameplayCamera;
    private Vector2 cameraBounds;
    private LaserSpawner[] spawners;

    public void Start()
    {

        currentActionMapName = "Player";




        gameManager = GameManager.Instance;
        gameplayCamera = gameManager.gameplayCamera;
        cameraBounds = gameManager.cameraBounds - (Vector2) playerCollider.bounds.extents;

        gameManager.AddPlayerHealth(maxHealth);

        gameManager.OnPlayerDeath.AddListener(Die);

        // store all the Laser Spawners components in an array to avoid calling GetComponents() many times
        spawners = GetComponentsInChildren<LaserSpawner>();
    }

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        // converts cursor position (in screen space) to world space based on camera position/size
        Vector2 cursorPos = context.ReadValue<Vector2>();
        //Debug.Log(cursorPos);
        Vector2 position = gameManager.gameplayCamera.ScreenToWorldPoint(cursorPos);
        playerBody.transform.position = KeepInBounds(position);
    }

    private Vector2 KeepInBounds(Vector2 position)
    {
        float clampedXPos = Mathf.Clamp(position.x, -cameraBounds.x, cameraBounds.x);
        float clampedYPos = Mathf.Clamp(position.y, -cameraBounds.y, cameraBounds.y);
        return new Vector2(clampedXPos, clampedYPos);
    }
  
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shootSound.Play();
            FireLasers();
        }
    }

    private void FireLasers()
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
            gameManager.RemovePlayerHealth(1);
            hitSound.Play();
            Debug.Log("Player hit");
        }
    }

    private void Die()
    {
        playerCollider.enabled = false;
        playerRenderer.enabled = false;
        deathSound.Play();
        Destroy(this.gameObject, 0.5f);
    }

    public void togglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Toggle pause reached");
            gameManager.OnGameTogglePause.Invoke();


            if (gameManager.paused)
            {
                //here we'll want to swap the action mapping
            }
        }
    }

    /*
    public void SetActionMap(string newActionMap)
    {
        if (newActionMap.Equals("Player"))
        {
            PlayerInputActions
        }
    }

    public void ToggleActionMapMap()
    {
        if()
    }
    */
}
