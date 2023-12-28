using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Parameters:
///     menus:
///         when the player is in any menu OTHER THAN paued (any with thier own scene)
///         
///     gameplay:  
///         when the game is running
///         
///     paused: 
///         when in the gameplay scene, but gameplay is paused (pause menu)
/// </summary>
public enum GameState
{
    mainMenu, gameplay, paused, dead
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public PlayerController playerController;
    public SpeedManager speedManager;
    public LoseMenuController loseMenuController;

    [Tooltip("size of playing field rep'd by (height, width)")]
    public Vector2 cameraBounds;

    [Header("Balance")]
    [Tooltip("The starting/current health of the player")]
    [SerializeField] private float playerHealth;
    [SerializeField] private float maxEnergyLevel;
    [SerializeField] public float maxRelativeSpeed;
    [Tooltip("The starting/current base speed of the player")]
    [SerializeField] private float baseSpeed;
    [Tooltip("This modifies asteroid values to make the game harder with each level")]
    [SerializeField] private float levelScale = 1;

    [Header("References")]
    [SerializeField] public GameObject explosionPrefab; //I will hopefully not need to keep this here (bobby)
    [SerializeField] public Camera gameplayCamera;
    [SerializeField] public string hazardTag;
    [SerializeField] public string playerTag;

    [Header("Info")]
    [Tooltip("The current energy level (max charge amount")] 
    [SerializeField] private float energyLevel;
    [Tooltip("The current energy charge")]
    [SerializeField] private float chargeLevel;
    [Tooltip("The current value of the third bar, added to base speed")]
    [SerializeField] public float relativeSpeed;

    // The current score (probably measured in distance)
    private float score;

    //whether or not the game is currently paused
    public bool paused { get; private set; } //may want to expand this an enum

    [SerializeField] public GameState gameState { get; protected set; }

    public UnityEvent OnPlayerHeal;
    public UnityEvent OnPlayerHurt;

    public UnityEvent OnEnergyChange;
    public UnityEvent OnChargeChange;
    public UnityEvent OnFiredChange;

    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;

    public UnityEvent OnBoostStart;
    public UnityEvent OnBoostEnd;


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        cameraBounds = new Vector2(gameplayCamera.orthographicSize - 1, (gameplayCamera.orthographicSize * gameplayCamera.aspect) - 1f);

        // Initilize playerHealth to 0, the player will call AddPlayerHealth() when the game starts.
        // This allows max health to be configued in the player object or at runtime
        playerHealth = 0;
        Time.timeScale = 1;
        Cursor.visible = false;
        speedManager = playerController.speedManager;
    }


    private void FixedUpdate()
    {
        score += GetCameraSpeed() * Time.deltaTime;
    }

    public void AddPlayerHealth(float amount)
    {
        playerHealth += amount;
        OnPlayerHeal.Invoke();
    }

    public void RemovePlayerHealth(float amount)
    {
        playerHealth -= amount;
        OnPlayerHurt.Invoke();



        if (playerHealth <= 0)//if player is at or below zero health, kill them
        {
            
            playerHealth = 0;

            Die();
        }
    }

    private void Die()
    {
        playerController.Die();

        StopAllCoroutines();

        // playerCollider.enabled = false;

        gameState = GameState.dead;
        Time.timeScale = 0;

        playerController.SwitchActionMap();

        loseMenuController.ShowDeathMenu();

    }

    public void UpdateEnergy(float amount)
    {
        //energyLevel = Mathf.Clamp(0, energyLevel + amount, maxEnergyLevel);

        energyLevel = Mathf.Min(maxEnergyLevel, energyLevel + amount);
        energyLevel = Mathf.Max(0, energyLevel);

        if (amount > 0)
        {
            //playerController.energyDecayTime = 0; 
        }
        

        OnEnergyChange.Invoke();
    }


    public void ResetEnergy()
    {
        energyLevel = 0;
        OnEnergyChange.Invoke();
    }

    public void UpdateRelativeSpeed(float amount)
    {
        if(amount > 0)
        {

            relativeSpeed = Mathf.Min(relativeSpeed + amount,
                Mathf.Min(energyLevel, maxRelativeSpeed));

        } else //we're decreasing
        {

            relativeSpeed = Mathf.Max(relativeSpeed, 0); //make sure we don't go below zero
        }



        OnFiredChange.Invoke();
    }

    public float GetLevelScale() => levelScale;

    [Tooltip("adds (or subtracts) the given value to the amount of charge")]
    public void UpdateCharge(float adjustmentAmount)
    {
        chargeLevel = Mathf.Min(energyLevel, chargeLevel + adjustmentAmount);
        chargeLevel = Mathf.Max(0, chargeLevel);

        OnChargeChange.Invoke();

    }

    public void ResetCharge()
    {
        chargeLevel = 0;
        OnChargeChange.Invoke();
    }

    public void EndBoost(float numOfBoosts, float speedOnExit)
    {
        relativeSpeed = 0;
        baseSpeed += speedOnExit;
        //Debug.Log("number of resets, speed on exit " + numOfBoosts + " " + speedOnExit);
    }


    public void UpdateScore(int amount)
    {
        score += amount;
    }


    public void PauseGame()
    {
        gameState = GameState.paused;
        Time.timeScale = 0;

        OnGamePause.Invoke();       

    }

    public void ResumeGame(bool invokeEvent = true)
    {
        gameState = GameState.gameplay;
        Time.timeScale = 1;
        if (invokeEvent) { OnGameResume.Invoke(); }        
    }


    [Tooltip("if not in boost, returns the total of the current relative speed, plus the base speed")]
    public float GetCameraSpeed() => speedManager.inBoost? speedManager.boostSpeed : relativeSpeed + baseSpeed;

    public float GetPlayerHealth() => playerHealth;
    public float GetEnergy() => energyLevel;
    public float GetMaxEnergy() => maxEnergyLevel;
    public float GetCharge() => chargeLevel;
    public float GetScore() => score;
}