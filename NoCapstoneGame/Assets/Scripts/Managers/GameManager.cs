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

    [SerializeField] public Camera gameplayCamera;
    [SerializeField] public string hazardTag;
    [SerializeField] public string playerTag;

    public Vector2 cameraBounds;

    // The current health of the player
    [SerializeField] private float playerHealth;

    [SerializeField] public float startingSpeedUnscaled; 

    [SerializeField] private float maxEnergyLevel;
    // The current energy level (max charge amount) 
    [SerializeField] private float energyLevel;

    // The current energy charge (for MVP)
    [SerializeField] private float chargeLevel;

    public float maxFired;
    [SerializeField] public float relativeSpeed;

    [Tooltip("the minimum speed the player will go, relative to the number of boosts the player has undergone")]
    public float baseSpeed;
    [Tooltip("setting speed to ints (or at least floats at similar sizes to ints, such as 1.75) is more intuitive, but causes insane speeds. This scales it down as well as offering parameterization of how quickly speed increases")]
    [SerializeField] private float speedScale;

    [Tooltip("This modifies asteroid values to make the game harder with each level")]
    [SerializeField] private float levelScale = 1;

    [SerializeField] public GameObject explosionPrefab; //I will hopefully not need to keep this here (bobby)

    // The current score (probably measured in distance)
    private int score;

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

    private void Start()
    {
        //OnGameTogglePause.AddListener(TogglePause);
        baseSpeed = startingSpeedUnscaled ;
    }

    private void Update()
    {

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
            OnPlayerDeath.Invoke();
            EnterMenus();   
        }
    }


    public float GetPlayerHealth()
    {
        return playerHealth;
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

    public float GetEnergy() => energyLevel;

    public void UpdateRelativeSpeed(float amount)
    {
        if(amount > 0)
        {

            relativeSpeed = Mathf.Min(relativeSpeed + amount,
                Mathf.Min(energyLevel, maxFired));

        } else //we're decreasing
        {

            relativeSpeed = Mathf.Max(relativeSpeed, 0); //make sure we don't go below zero
        }



        OnFiredChange.Invoke();
    }


    public float GetMaxEnergy()
    {
        return maxEnergyLevel;
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

    public float GetCharge()
    {
        return chargeLevel;
    }


    public void EndBoost(int numOfResets, float speedOnExit)
    {
        relativeSpeed = 0;
        baseSpeed = numOfResets * speedOnExit;
    }


    public void UpdateScore(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
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

    public void EnterMenus()
    {
        gameState = GameState.menus;
        Time.timeScale = 0;

        OnGameEnterMenus.Invoke();
    }

    [Tooltip("if not in boost, returns the total of the current relative speed, plus the base speed")]
    public float GetUnscaledSpeed() => speedManager.inBoost? speedManager.boostSpeed : relativeSpeed + baseSpeed;
    public float GetSpeedScale() => speedScale;
    public float GetScaledSpeed() => GetUnscaledSpeed() * GetSpeedScale();
}