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

    [SerializeField] public Camera gameplayCamera;
    [SerializeField] public string hazardTag;
    [SerializeField] public string playerTag;

    public Vector2 cameraBounds;

    // The current health of the player
    [SerializeField] private float playerHealth;

    [SerializeField] private float maxEnergyLevel;
    // The current energy level (max charge amount) (for MVP)
    [SerializeField] private float energyLevel;

    // The current energy charge (for MVP)
    [SerializeField] private float chargeLevel;


    public float maxFired;
    public float firedLevel { get; private set; } 
    [Tooltip("setting speed to ints is more intuitive, but causes insane speeds. This scales it down as well as offering parameterization of how quickly speed increases")]
    [SerializeField] private float speedScale;

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

    public UnityEvent OnBoostStart;
    public UnityEvent OnBoostEnd;

    void Awake()
    {

        //TODO: SET INITIAL VALUE in liue of the following line
        //paused = false;


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
    }

    private void Start()
    {
        //OnGameTogglePause.AddListener(TogglePause);
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

    public float GetPlayerHealth()
    {
        return playerHealth;
    }


    public void UpdateEnergy(float amount)
    {
        energyLevel = Mathf.Min(energyLevel+ amount, maxEnergyLevel);
        energyLevel = Mathf.Max(energyLevel+ amount, 0);

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
    public void UpdateFired(float amount)
    {
        firedLevel = Mathf.Min(firedLevel + amount, maxFired);
        firedLevel = Mathf.Max(firedLevel + amount, 0);

        OnFiredChange.Invoke();

    }

    public float GetMaxEnergy()
    {
        return maxEnergyLevel;
    }

    [Tooltip("adds (or subtracts) the given value to the amount of charge")]
    public void UpdateCharge(float adjustmentAmount)
    {
        chargeLevel = Mathf.Min(energyLevel, chargeLevel + adjustmentAmount);
        chargeLevel = Mathf.Max(0, chargeLevel + adjustmentAmount);

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

    public void StartBoost()
    {
        OnBoostStart.Invoke();
    }

    public void EndBoost()
    {
        OnBoostEnd.Invoke();
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


    public float GetSpeed() => firedLevel;
    public float GetSpeedScale() => speedScale;
}