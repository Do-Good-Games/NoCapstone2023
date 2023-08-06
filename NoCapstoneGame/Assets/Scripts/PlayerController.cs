using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip ("The player's starting health")]
    [SerializeField] public int maxHealth;

    [Tooltip("The units of charged gained each second the mouse is clicked")]
    [SerializeField] public float ChargeGainPerSecond;

    [Tooltip("The units of charge spent by a single shot")]
    [SerializeField] public float ChargeSpentPerShot;

    [Tooltip("The time between individual shots in a volley, in seconds")]
    [SerializeField] public float TimeBetweenShots;

    [Tooltip("The size increase for the Energy Sphere per unit charged")]
    [SerializeField] public float EnergySizePerUnitCharged;

    [Tooltip("The maximum size of the energy sphere when charging")]
    [SerializeField] public float MaxEnergySphereSize;

    [Tooltip("The difference in the visual size of the energy sphere and its hitbox")]
    [SerializeField] public float EnergySphereHitboxGraceArea;

    [Tooltip("The length of time that the player is invincible after being hit, in seconds")]
    [SerializeField] public float DamageCooldownTime;

    [SerializeField] private float MinChargeForSlingshot;

    [SerializeField] private float SpaceshipStretchFactor;
    [SerializeField] private float EnergySphereStretchFactor;


    [Header("Visuals")]
    [Tooltip("The initial length of each flash during the cooldown, in seconds")]
    [SerializeField] public float DamageFlashSpeed;

    [Tooltip("The length of time before the flashing increases in speed, in seconds (Must be smaller than DamageCooldownTime)")]
    [SerializeField] public float DamageFlashSpeedupTime;

    [Tooltip("The length of each flash towards the end of the cooldown, in seconds")]
    [SerializeField] public float DamageFlashFastSpeed;

    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] SpriteRenderer energySphereRender;

    [Header("Sounds")]
    [SerializeField] AudioSource shootSound;
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource deathSound;

    [Header("Physics")]
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Collider2D shipCollider;
    [SerializeField] CircleCollider2D energySphereCollider;

    [Header("Children")]
    [SerializeField] Transform shipTransform;
    [SerializeField] Transform energyTransform;
    [SerializeField] Transform magnetTransform;


    public GameManager gameManager;
    private Camera gameplayCamera;
    private Vector2 cameraBounds;
    private LaserSpawner[] spawners;

    private bool mouseHeld;
    private bool slingshotHeld;
    private Vector2 cursorPos;
    private Vector2 slingshotAnchor;

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
        cameraBounds = gameManager.cameraBounds - (Vector2) shipCollider.bounds.extents;

        gameManager.AddPlayerHealth(maxHealth);

        gameManager.OnPlayerDeath.AddListener(Die);

        // store all the Laser Spawners components in an array to avoid calling GetComponents() many times
        spawners = GetComponentsInChildren<LaserSpawner>();

        mouseHeld = false;
        slingshotHeld = false;
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

    public void UpdateCursorPosition(InputAction.CallbackContext context)
    {
        // converts cursor position (in screen space) to world space based on camera position/size
        Vector2 screenSpaceCursorPos = context.ReadValue<Vector2>();
        cursorPos = gameManager.gameplayCamera.ScreenToWorldPoint(screenSpaceCursorPos);

        if (slingshotHeld)
        {
            SetStretchedPositions(cursorPos);
        }
        else
        {
            SetPositions(cursorPos);
        }
    }

    private void SetPositions(Vector2 position)
    {
        Vector2 inBoundsPosition = KeepInBounds(position);
        shipTransform.position = inBoundsPosition;
        energyTransform.position = inBoundsPosition;
        magnetTransform.position = inBoundsPosition;
    }

    private void SetStretchedPositions(Vector2 position)
    {
        Vector2 stretchedShipPosition = Vector2.Lerp(slingshotAnchor, position, SpaceshipStretchFactor);
        Debug.Log(slingshotAnchor + " " + " " + position + " " + stretchedShipPosition);
        stretchedShipPosition = KeepInBounds(stretchedShipPosition);
        shipTransform.position = stretchedShipPosition;
        energyTransform.position = Vector2.Lerp(slingshotAnchor, stretchedShipPosition, EnergySphereStretchFactor);
        magnetTransform.position = stretchedShipPosition;
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
            if (gameManager.getCharge() > ChargeSpentPerShot)
            {
                shooting = true;

                ShootCoroutineObject = ShootCoroutine();
                StartCoroutine(ShootCoroutineObject);
            }
            else
            {
                gameManager.ResetCharge();
                UpdateEnergySphere();
            }
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (gameManager.getCharge() >= MinChargeForSlingshot)
            {
                slingshotHeld = true;
                slingshotAnchor = cursorPos;
            }
            else
            {
                // Give the player some feedback here 
                Debug.Log("Slingshot attempted with insufficient charge");
            }
        }
        if (context.canceled)
        {
            slingshotHeld = false;
            SetPositions(cursorPos);
        }
    }
    
    private IEnumerator ShootCoroutine()
    {
        while (shooting)
        {
            if (gameManager.getCharge() >= ChargeSpentPerShot)
            {
                FireLasers();
                gameManager.UpdateCharge(-ChargeSpentPerShot);
                UpdateEnergySphere();
            }

            yield return new WaitForSeconds(TimeBetweenShots);

            if (gameManager.getCharge() < ChargeSpentPerShot)
            {
                gameManager.ResetCharge();
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

   public void Hit() {
        if (damageable) { 
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
        StopAllCoroutines();

       // playerCollider.enabled = false;
        playerRenderer.enabled = false;
        deathSound.Play();

        Destroy(this.gameObject, 0.5f);
    }

    private void MoveChildren()
    {
        shipTransform = playerBody.transform;
        energyTransform = playerBody.transform;
        magnetTransform = playerBody.transform;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // If the colliding GameObject is tagged as a hazard, take damage
        if (collision.gameObject.CompareTag(gameManager.hazardTag))
        {
            Hit();
        }
    }
}
