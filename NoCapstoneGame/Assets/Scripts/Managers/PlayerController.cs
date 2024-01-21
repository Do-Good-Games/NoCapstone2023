using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("The player's starting health")]
    [SerializeField] public float maxHealth;
    [Tooltip("The length of time that the player is invincible after being hit, in seconds")]
    [SerializeField] public float DamageCooldownTime;
    //[Tooltip("The units of charged gained each second the mouse is clicked")]
    [Tooltip("how much damage the player takes on a hit")]
    [SerializeField] private float healthLostOnHit;
    [Tooltip("used to control behavior surrounding speed")]
    [SerializeField] public SpeedManager speedManager;

    //[SerializeField] public SPSOBase SpeedPrototypeSO;
    //[SerializeField] public SOBoostBase SOBoost;

    //TODO when releasing rightmb reset energy


    [Header("energy and charge values")]
    [SerializeField] public float ChargeGainPerSecond;
    [Tooltip("The units of charge spent by a single shot")]
    [SerializeField] public float ChargeSpentPerShot;
    [Tooltip("The units of energy spent by a single shot")]
    [SerializeField] public float EnergySpentPerShot;
    [Tooltip("The time between individual shots in a volley, in seconds")]
    [SerializeField] public float TimeBetweenShots;
    
    //energy decay
    private enum EnergyDecayType { none, currentRatio, totalRatio, fixedAmount}
    [Tooltip("which type of energy decay we want to use")]
    [SerializeField] private EnergyDecayType energyDecayType;
    [Tooltip("rate at which the player loses energy over time")]
    [SerializeField] private float energyDecayAmount;
    [Tooltip("the amount of time (in seconds) until the energy decay occurs")]
    [SerializeField] private float energyDecayDelay;
    private float energyDecayTime;
    
    //on hit
    [SerializeField] private AnimationCurve protectionThreshold;
    [Tooltip("whether or not we want energy to act as a \"buffer\" to the player taking damage - when this is true, the player will not take damage when hit provided they have energy")]
    [SerializeField] private bool EnergyProtects;
    [Tooltip("the amount of energy the player loses when they get hit - represented as a ratio of the total energy the player can hold")]
    [SerializeField] private float energyLostOnHit;

    //[Tooltip("the minimum amount of energy needed for the player to protect against damage, represented as a ratio from 0-1 \n to disable this mechanic, set to 0")]
    //[SerializeField] float protectionThreshold;
    ////[Tooltip("the minimum amount of energy needed for the player to fully protect against damage represented as a ratio from 0 to 1 \n if the player has less energy than this amount they will take damage proportional to the amount they do have in relation to this value \n \n to disable this feature set it to 0 ")]
    //[Tooltip("point at which the player will start receiving \"partial\" protection below the full threshold. value from 0-1 representing a proportion of "]
    //[SerializeField] float protectionRatio;




    [Header("energy sphere")]
    [Tooltip("The size increase for the Energy Sphere per unit charged")]
    [SerializeField] public float EnergySizePerUnitCharged;
    [Tooltip("The maximum size of the energy sphere when charging")]
    [SerializeField] public float MaxEnergySphereSize;
    [Tooltip("The difference in the visual size of the energy sphere and its hitbox")]
    [SerializeField] public float EnergySphereHitboxGraceArea;


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

    [SerializeField] ChargeShotProjectile chargeShotPrefab;

    string currentActionMapName;
    public PlayerInput playerInput;



    private GameManager gameManager;
    private SceneManager sceneManager;
    private Vector2 cameraBounds;
    private LaserSpawner[] spawners;


    public bool leftMouseHeld;
    private bool RightMouseHeld;
    private Vector2 cursorPos;
    private Vector2 prevCursorPos;
    private Vector2 mouseDelta;
    private Vector2 slingshotAnchor;
    //private Vector2 cursorPosPrePause;
    private float energySphereSize;

    private bool shooting;
    private bool damageable;
    private bool damageCooldownEnding;

    private bool enteringGameplay = true;

    private IEnumerator ShootCoroutineObject;
    private IEnumerator DamageCooldownCoroutineObject;
    private IEnumerator DamageFlashCoroutineObject;

    private float PrevEnergyLevel = 0;

    public void Start()
    {
        gameManager = GameManager.Instance;
        cameraBounds = gameManager.cameraBounds - (Vector2) shipCollider.bounds.extents;

        gameManager.AddPlayerHealth(maxHealth);

        //gameManager.OnPlayerDeath.AddListener(Die);

        //all three of these reference the same method as that one contains code and logic that would be difficult to translate on a different set of case-by-case bases
        gameManager.OnGameResume.AddListener(SwitchActionMap); 
        gameManager.OnGamePause.AddListener(SwitchActionMap);
        

        //gameManager.OnBoostStart.AddListener(BoostStarted); //cleanup: remove?
        //gameManager.OnBoostEnd.AddListener(BoostEnded);//cleanup: remove?


        //gameManager.OnGameTogglePause.AddListener(SwitchActionMap);

        // store all the Laser Spawners components in an array to avoid calling GetComponents() many times
        spawners = GetComponentsInChildren<LaserSpawner>();

        mouseDelta = Vector2.zero;


        leftMouseHeld = false;
        RightMouseHeld = false;
        shooting = false;
        damageable = true;
        damageCooldownEnding = false;

        currentActionMapName = "Player";
        //could we instead 
        gameManager.ResumeGame(false);
        SwitchActionMap();

        enteringGameplay = true; // mousedelta - should this be false instead?

        //depreciated prototype code - no replacement necessary
        //SpeedPrototypeSO.ResetVariables();
        //SOBoost.ResetVariables();
        //SOBoost.speedPrototype = SpeedPrototypeSO;
    }



    public void Update()
    {
        //SpeedPrototypeSO.SPOverTime();//depreciated prototype code - no replacement necessary
        // If only the left mouse is held, increase charge value
        if (leftMouseHeld)
        {
            gameManager.UpdateCharge(ChargeGainPerSecond * Time.deltaTime);
            speedManager.Fired(ChargeGainPerSecond * Time.deltaTime);

            //SOBoost.incFired(ChargeGainPerSecond * Time.deltaTime); //depreciated prototype code - changed to sm.fired()

            UpdateEnergySphere();
            energyDecayTime = 0;
        }else if(RightMouseHeld) {
            gameManager.UpdateCharge(ChargeGainPerSecond * Time.deltaTime);
            energyDecayTime = 0;
        }
        else if (!shooting) //decaying energy while we're actively firing causes unwanted behavior with the speed var, plus we probably shouldn't anyway
        {
            if(energyDecayTime >= energyDecayDelay)
            {
                
                DecayEnergy();
            } else
            {
                energyDecayTime += Time.deltaTime;
            }
        }
    }


    #region cursor movement
    public void UpdateCursorPosition(InputAction.CallbackContext context)
    {

        mouseDelta = context.ReadValue<Vector2>();

        

        if(gameManager.gameState == GameState.gameplay)
        {
            AdjustPosition(mouseDelta);
        }


    }


    private void AdjustPosition(Vector2 direction)
    {
        //Debug.Log("set position called with " + position);
        Vector2 inBoundsPosition = KeepInBounds((Vector2) shipTransform.position +  direction);
        shipTransform.position = inBoundsPosition;
        energyTransform.position = inBoundsPosition;
        magnetTransform.position = inBoundsPosition;
    }

    private Vector2 KeepInBounds(Vector2 position)
    {
        float clampedXPos = Mathf.Clamp(position.x, -cameraBounds.x, cameraBounds.x);
        float clampedYPos = Mathf.Clamp(position.y, -cameraBounds.y, cameraBounds.y);
        return new Vector2(clampedXPos, clampedYPos);
    }
    #endregion cursor movement

    public void Charge(InputAction.CallbackContext context)
    {
        if(!speedManager.inBoost && !RightMouseHeld)
        {
            if (context.started)
            {
                leftMouseHeld = true;
                shooting = false;

                if (ShootCoroutineObject != null)
                {
                    StopCoroutine(ShootCoroutineObject);
                }
            }
            if (context.canceled)
            {
                leftMouseHeld = false;
                //if (gameManager.getCharge() >= ChargeSpentPerShot) //switch to this line if you want to disable single fire shooting
                if (gameManager.GetCharge() >= ChargeSpentPerShot || gameManager.GetEnergy() >= EnergySpentPerShot)
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
    }


    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            RightMouseHeld = false;

            if (gameManager.GetCharge() >= gameManager.GetMaxEnergy()) //if our charge is at max (max calc'd as max energy)
            {
                speedManager.ActivateBoost();
            } else
            {
                gameManager.ResetCharge();
            }
        }
        else
        {
            if (context.started)
            {//below had gameManager.maxFired instead of gameManager.maxRelativeSpeed
                if (gameManager.relativeSpeed >= gameManager.maxRelativeSpeed && gameManager.GetEnergy() >= gameManager.GetMaxEnergy()) //if our fired var and our energy var are at max
                {
                    RightMouseHeld = true;
                }
            }
        }
    }

    private IEnumerator ShootCoroutine()
    {
        int i = 0;
        while (shooting)
        {
            while (RightMouseHeld) //while we're still holding mouse, don't fire yet
            {
                yield return null;
            }

            float chargeLevel = gameManager.GetCharge();
            float energyLevel = gameManager.GetEnergy();

            if (chargeLevel >= ChargeSpentPerShot) //provided this won't cause us to run out of charge
            {
                //SpeedPrototypeSO.SPEnergyFired(ChargeSpentPerShot); //from prototyping - removed as we wanted the firing changes to happen from the way we did it on boost
                //speedManager.Fired(ChargeSpentPerShot);
                gameManager.UpdateCharge(-ChargeSpentPerShot);
                gameManager.UpdateEnergy(-EnergySpentPerShot);
                FireLasers();
                UpdateEnergySphere();
            }
            else if (energyLevel >= EnergySpentPerShot) //if we will run out of charge, (but we won't run out of energy)
            {
                //SpeedPrototypeSO.SPEnergyFired(ChargeSpentPerShot);//from prototyping - removed as we wanted the firing changes to happen from the way we did it on boost
                gameManager.UpdateEnergy(-EnergySpentPerShot);
                FireLasers();
                gameManager.ResetCharge();
                UpdateEnergySphere();
                yield break;
            }

            yield return new WaitForSeconds(TimeBetweenShots);

            //Debug.Log("yes it does");


            if (chargeLevel < ChargeSpentPerShot)
            {
                gameManager.ResetCharge();
                UpdateEnergySphere();
                shooting = false;
            }
        }
    }

    private void FireLasers()
    {
        // tell every spawner to spawn a laser
        foreach (LaserSpawner spawner in spawners)
        {
            shootSound.Play();
            //SFXManager.Instance.Play(shootSound.clip);
            gameManager.UpdateEnergy(-EnergySpentPerShot);
            spawner.SpawnLaser();
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
        float energySphereSize = Mathf.Min(gameManager.GetCharge() * EnergySizePerUnitCharged, MaxEnergySphereSize);
        energySphereRender.size = Vector2.one * energySphereSize;

        energySphereCollider.radius = (energySphereSize / 2) - EnergySphereHitboxGraceArea;
    }



    private void LaunchChargeShot(Vector2 position, Vector2 launchVector)
    {
        ChargeShotProjectile projectile = GameObject.Instantiate<ChargeShotProjectile>(chargeShotPrefab, position, Quaternion.identity);
        projectile.SetSize(energySphereSize * energyTransform.lossyScale.x);
        projectile.Launch(launchVector);
    }

    public void Hit() {

        if (damageable && !speedManager.inBoostGracePeriod) {
            //SpeedPrototypeSO.SPHit(); //prototype

            hitSound.Play();
            //SFXManager.Instance.Play(hitSound.clip);

            DamageCooldownCoroutineObject = DamageCooldownCoroutine();
            StartCoroutine(DamageCooldownCoroutineObject);

            DamageFlashCoroutineObject = DamageFlashCoroutine();
            StartCoroutine(DamageFlashCoroutineObject);

            if (EnergyProtects){

                float amount;

                amount = MathF.Max(0, protectionThreshold.Evaluate(gameManager.GetEnergy() / gameManager.GetMaxEnergy()));

                //Debug.Log("amount: " + amount);

                if(amount <1)
                {
                    float damageAmount = (healthLostOnHit * amount);
                    //Debug.Log("player at least partially protected " + damageAmount);
                    gameManager.RemovePlayerHealth(damageAmount);
                    gameManager.UpdateEnergy(- gameManager.GetMaxEnergy() * energyLostOnHit);
                }
                else
                {
                    float damageAmount = (healthLostOnHit);
                    //Debug.Log("player taking full damage " + damageAmount);
                    gameManager.RemovePlayerHealth(healthLostOnHit);


                }

            }
            if (RightMouseHeld)
            {
                RightMouseHeld = false;
                gameManager.UpdateRelativeSpeed(-gameManager.GetCharge());
                //the below line is causing an error because UpdateRelative speed does not exist anymore
                gameManager.UpdateRelativeSpeed(-gameManager.GetCharge());

                gameManager.ResetCharge();

            }
        }
    }

    public void Die()
    {
        StopAllCoroutines();

        //playerCollider.enabled = false;
        playerRenderer.enabled = false;
        SetActionMapUI();
        StartCoroutine(PlaySoundThenDestroy());

        //switch action map to UI
    }

    private IEnumerator PlaySoundThenDestroy()
    {
        playerRenderer.enabled = false;
        //m_collider.enabled = false;

        deathSound.Play();
        //SFXManager.Instance.Play(deathSound.clip);


        while (deathSound.isPlaying)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }


    public void togglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (gameManager.gameState == GameState.gameplay)
            {
                gameManager.PauseGame();
            }
            else if (gameManager.gameState == GameState.paused)
            {
                gameManager.ResumeGame();
            }
            else if (gameManager.gameState == GameState.mainMenu)
            {
                //throw new Exception("togglePause() (the input callback) is somehow being called when the game is in menus - this shouldn't be able to happen");  
            }
            //gameManager.OnGameTogglePause.Invoke();
        }
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
            // Assuming inBoost is true when the player is in hyperspeed
            if (speedManager.inBoost)
            {
                Asteroid asteroid = collision.GetComponent<Asteroid>();
                if (asteroid != null)
                {//if the hazard contains an asteroid component, destroy it
                    asteroid.DestroyAsteroid();
                }            
            } else
            { // if we're not in boost, hit the player
                Hit();
            }
        } else if (collision.GetComponent<Energy>())
        {
            energyDecayTime = 0;
        }
    }

    public void SwitchActionMap()
    {
        if (gameManager.gameState == GameState.paused || gameManager.gameState == GameState.dead)
        {
            //cursorPosPrePause = mouseDelta;
            //Debug.Log("pre pause: " + cursorPosPrePause);
            //SetPositions(cursorPosPrePause);
            SetActionMapUI();
            //here we'll want to swap the action mapping
        }
        else if (gameManager.gameState == GameState.mainMenu)
        {
            //cursorPosPrePause = mouseDelta; //check here if player position is wack upon loading the game
            //Debug.Log("pre pause: " + cursorPosPrePause);
            //SetPositions(cursorPosPrePause);
            SetActionMapUI();
        } else //we're entering gameplay
        {
            enteringGameplay = true;

            //Mouse.current.WarpCursorPosition(gameManager.gameplayCamera.WorldToScreenPoint(cursorPosPrePause));
            Debug.Log("set mouse pos" + Mouse.current.position);
            //SetPositions(cursorPosPrePause);
            SetActionMapPlayer();
            //Debug.Log("cursorPos:" + cursorPos + " pre pause: " + cursorPosPrePause + " w2sp: " + gameManager.gameplayCamera.WorldToScreenPoint(cursorPosPrePause));
        }
    }

    private void SetActionMapPlayer() { SetActionMap("Playing"); }
    private void SetActionMapUI() { SetActionMap("Menus"); }
    private void SetActionMap(string newActionMapName)
    {
        playerInput.currentActionMap.Disable();
        if (gameManager.gameState == GameState.paused)
        {
            //SetPositions(cursorPosPrePause);
        }
        playerInput.SwitchCurrentActionMap(newActionMapName);

        switch (newActionMapName)
        {
            case "Menus":
                //Debug.Log("ping");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            default: //case playing
                //Debug.Log("pong");
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    private void DecayEnergy()
    {
        float decayAmount = 0;
        switch (energyDecayType)
        {
            default:
            case (EnergyDecayType.none):
                break;
            case (EnergyDecayType.fixedAmount):
                decayAmount = energyDecayAmount * Time.deltaTime;
                gameManager.UpdateEnergy(-decayAmount);
                break;
            case (EnergyDecayType.currentRatio):
                decayAmount = gameManager.GetEnergy() * energyDecayAmount * Time.deltaTime;
                gameManager.UpdateEnergy(-decayAmount);
                break;
            case (EnergyDecayType.totalRatio):
                decayAmount = gameManager.GetMaxEnergy() * energyDecayAmount * Time.deltaTime;
                gameManager.UpdateEnergy(-decayAmount);
                break;
        }

        
    }
}
