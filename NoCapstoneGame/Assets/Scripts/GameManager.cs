using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] public Camera gameplayCamera;

    public Vector2 cameraBounds;

    // The current health of the player
    private int playerHealth;

    // The current energy level (max charge amount) (for MVP)
    private int energyLevel;

    // The current energy charge (for MVP)
    private int chargeLevel;

    // The current score (probably measured in distance)
    private int score;

    //whether or not the game is currently paused
    public bool paused; //may want to expand this an enum

    public UnityEvent OnPlayerHeal;
    public UnityEvent OnPlayerHurt;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnEnergyChange;
    public UnityEvent OnChargeChange;

    public UnityEvent OnGameTogglePause;

    void Awake()
    {
        paused = false;

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        cameraBounds = new Vector2(gameplayCamera.orthographicSize, gameplayCamera.orthographicSize * gameplayCamera.aspect);

        // Initilize playerHealth to 0, the player will call AddPlayerHealth() when the game starts.
        // This allows max health to be configued in the player object or at runtime
        playerHealth = 0;
        Cursor.visible = false;
    }

    private void Start()
    {
        OnGameTogglePause.AddListener(togglePause);
    }

    public void AddPlayerHealth(int amount)
    {
        playerHealth += amount;
        OnPlayerHeal.Invoke();
    }

    public void RemovePlayerHealth(int amount)
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

    public int GetPlayerHealth()
    {
        return playerHealth;
    }


    public void UpdateEnergy(int amount)
    {
        energyLevel += amount;
        OnEnergyChange.Invoke();
    }

    public void ResetEnergy()
    {
        energyLevel = 0;
        OnEnergyChange.Invoke();
    }

    public int getEnergy() => energyLevel;


    public void UpdateCharge(int amount)
    {
        chargeLevel += amount;
        OnChargeChange.Invoke();
    }

    public void ResetCharge()
    {
        chargeLevel = 0;
        OnChargeChange.Invoke();
    }

    public int getCharge()
    {
        return chargeLevel;
    }


    public void UpdateScore(int amount)
    {
        score += amount;
    }

    public int getScore()
    {
        return score;
    }

    public void togglePause()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1;
            ///wil also want to invoke the pause event

        } else
        {
            paused = true;
            Time.timeScale = 0;
        }

    }
}