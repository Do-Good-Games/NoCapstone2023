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
    menus, gameplay, paused
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public PlayerController playerController;
    public SpeedManager speedManager;

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

    [SerializeField] public GameState gameState { get; private set; }

    public UnityEvent OnPlayerHeal;
    public UnityEvent OnPlayerHurt;
    public UnityEvent OnPlayerDeath;

    public UnityEvent OnEnergyChange;
    public UnityEvent OnChargeChange;
    public UnityEvent OnFiredChange;

    public UnityEvent OnGameEnterMenus;
    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;



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

        cameraBounds = new Vector2(gameplayCamera.orthographicSize - 1, (gameplayCamera.orthographicSize * gameplayCamera.aspect) - 1.5f);

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
        if (playerHealth <= 0)
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        playerHealth = 0;
        OnPlayerDeath.Invoke();
    }


    public void UpdateEnergy(float amount)
    {
        energyLevel = Mathf.Min(energyLevel+ amount, maxEnergyLevel);
        energyLevel = Mathf.Max(energyLevel, 0);

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

    public void UpdateFired(float amount)
    {
        relativeSpeed = Mathf.Min(relativeSpeed + amount, maxRelativeSpeed);
        relativeSpeed = Mathf.Max(relativeSpeed, 0);

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

    public void ResumeGame()
    {
        gameState = GameState.gameplay;
        Time.timeScale = 1;

        OnGameResume.Invoke();
    }

    public void EnterMenus()
    {
        gameState = GameState.menus;
        Time.timeScale = 0;

        OnGameEnterMenus.Invoke();
    }

    [Tooltip("if not in boost, returns the total of the current relative speed, plus the base speed")]
    public float GetCameraSpeed() => speedManager.inBoost? speedManager.boostSpeed : relativeSpeed + baseSpeed;

    public float GetPlayerHealth() => playerHealth;
    public float GetEnergy() => energyLevel;
    public float GetMaxEnergy() => maxEnergyLevel;
    public float GetCharge() => chargeLevel;
    public float GetScore() => score;
}