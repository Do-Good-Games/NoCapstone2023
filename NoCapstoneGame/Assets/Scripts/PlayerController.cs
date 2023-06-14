using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Collider2D playerCollider;
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] SpriteRenderer energySphereRender;
    [SerializeField] CircleCollider2D energySphereCollider;

    [SerializeField] string hazardTag;
    [SerializeField] AudioSource shootSound;
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource deathSound;

    [SerializeField] public int maxHealth;

    [SerializeField] public float ChargeGainPerSecond;
    [SerializeField] public float ChargeSpentPerShot;
    [SerializeField] public float TimeBetweenShots;
    [Tooltip("The size increase for the Energy Sphere per unit charged")]
    [SerializeField] public float EnergySizePerUnitCharged;
    [SerializeField] public float MaxEnergySphereSize;
    [Tooltip ("The difference in the visual size of the energy sphere and its hitbox")]
    [SerializeField] public float EnergySphereHitboxGraceArea;
    [SerializeField] public float DamageCooldownTime;
    [SerializeField] public float DamageFlashSpeed;
    [SerializeField] public float DamageFlashSpeedupTime;
    [SerializeField] public float DamageFlashFastSpeed;

    GameManager gameManager;
    private Camera gameplayCamera;
    private Vector2 cameraBounds;
    private LaserSpawner[] spawners;
    private bool mouseHeld;
    private bool shooting;
    private bool damageable;
    private bool damageCooldownEnding;
    private IEnumerator ShootCoroutineObject;
    private IEnumerator DamageCooldownCoroutineObject;
    private IEnumerator DamageFlashCoroutineObject;

    public void Start()
    {
        gameManager = GameManager.Instance;
        gameplayCamera = gameManager.gameplayCamera;
        cameraBounds = gameManager.cameraBounds - (Vector2) playerCollider.bounds.extents;

        gameManager.AddPlayerHealth(maxHealth);

        gameManager.OnPlayerDeath.AddListener(Die);

        // store all the Laser Spawners components in an array to avoid calling GetComponents() many times
        spawners = GetComponentsInChildren<LaserSpawner>();

        mouseHeld = false;
        shooting = false;
        damageable = true;
        damageCooldownEnding = false; 
    }

    public void Update()
    {
        // If the mouse is held, increase charge value
        if (mouseHeld)
        {
            gameManager.UpdateCharge(ChargeGainPerSecond * Time.deltaTime);

            UpdateEnergySphere();
        }
      
    }

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        // converts cursor position (in screen space) to world space based on camera position/size
        Vector2 cursorPos = context.ReadValue<Vector2>();
        Vector2 position = gameManager.gameplayCamera.ScreenToWorldPoint(cursorPos);
        playerBody.transform.position = KeepInBounds(position);
    }

    private Vector2 KeepInBounds(Vector2 position)
    {
        float clampedXPos = Mathf.Clamp(position.x, -cameraBounds.x, cameraBounds.x);
        float clampedYPos = Mathf.Clamp(position.y, -cameraBounds.y, cameraBounds.y);
        return new Vector2(clampedXPos, clampedYPos);
    }
  
    public void Charge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            mouseHeld = true;
            shooting = false;

            if (ShootCoroutineObject != null)
            {
                StopCoroutine(ShootCoroutineObject);
            }
        }
        if (context.canceled)
        {
            mouseHeld = false;
            shooting = true;

            ShootCoroutineObject = ShootCoroutine();
            StartCoroutine(ShootCoroutineObject);
        }
    }
    
    private IEnumerator ShootCoroutine()
    {
        while (shooting)
        {
            if (gameManager.getCharge() > ChargeSpentPerShot)
            {
                FireLasers();
                gameManager.UpdateCharge(-ChargeSpentPerShot);
                UpdateEnergySphere();
            }

            yield return new WaitForSeconds(TimeBetweenShots);

            if (gameManager.getCharge() < ChargeSpentPerShot)
            {
                gameManager.UpdateCharge(-gameManager.getCharge());
                UpdateEnergySphere();
                shooting = false;
            }
        }
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        damageable = false;

        yield return new WaitForSeconds(DamageFlashSpeedupTime);
        // Let DamageFlashCoroutine() know that the cooldown is ending
        damageCooldownEnding = true;

        // Wait the remaining time left in DamageCooldownTime
        yield return new WaitForSeconds(DamageCooldownTime - DamageFlashSpeedupTime);
        damageable = true;
        damageCooldownEnding = false;
    }

    private IEnumerator DamageFlashCoroutine()
    {
        while (damageable == false)
        {
            // Toggle the player's visibility
            playerRenderer.enabled = !playerRenderer.enabled;
            // Wait the correct amount of time based on if the cooldown is about to end
            if (damageCooldownEnding)
            {
                yield return new WaitForSeconds(DamageFlashFastSpeed);
            }
            else
            {
                yield return new WaitForSeconds(DamageFlashFastSpeed);
            }
        }
        playerRenderer.enabled = true;
    }

    private void UpdateEnergySphere()
    {
        float energySphereSize = Mathf.Min(gameManager.getCharge() * EnergySizePerUnitCharged, MaxEnergySphereSize);
        Debug.Log(energySphereSize);
        Debug.Log(Vector2.one * energySphereSize);
        energySphereRender.size = Vector2.one * energySphereSize;

        energySphereCollider.radius = (energySphereSize / 2) - EnergySphereHitboxGraceArea;
    }

    private void FireLasers()
    {
        // tell every spawner to spawn a laser
        foreach (LaserSpawner spawner in spawners)
        {
            shootSound.Play();
            spawner.SpawnLaser();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // If the colliding GameObject is tagged as a hazard, take damage
        if (collision.gameObject.CompareTag(hazardTag) && damageable)
        {
            gameManager.RemovePlayerHealth(1);
            hitSound.Play();

            DamageCooldownCoroutineObject = DamageCooldownCoroutine();
            StartCoroutine(DamageCooldownCoroutineObject);

            DamageFlashCoroutineObject = DamageFlashCoroutine();
            StartCoroutine(DamageFlashCoroutineObject);
        }
    }

    private void Die()
    {
        playerCollider.enabled = false;
        playerRenderer.enabled = false;
        deathSound.Play();
        Destroy(this.gameObject, 0.5f);
    }
}
