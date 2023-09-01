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
    [SerializeField] public string hazardTag;
    [SerializeField] public string playerTag;

    public Vector2 cameraBounds;

    // The current health of the player
    private int playerHealth;

    [SerializeField] private float maxEnergyLevel;
    // The current energy level (max charge amount) (for MVP)
    [SerializeField] private float energyLevel;

    // The current energy charge (for MVP)
    [SerializeField] private float chargeLevel;

    [SerializeField] private float speed;
    [Tooltip("setting speed to ints is more intuitive, but causes insane speeds. This scales it down as well as offering parameterization of how quickly speed increases")]
    [SerializeField] private float speedScale;

    [SerializeField] public SFXManager sfxManager;

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
        OnGameTogglePause.AddListener(TogglePause);
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


    public void UpdateEnergy(float amount)
    {
        energyLevel = Mathf.Min(energyLevel+ amount, maxEnergyLevel);
        OnEnergyChange.Invoke();
    }

    public void ResetEnergy()
    {
        energyLevel = 0;
        OnEnergyChange.Invoke();
    }

    public float GetEnergy() => energyLevel;

    public float GetMaxEnergy()
    {
        return maxEnergyLevel;
    }


    public void UpdateCharge(float amount)
    {
        chargeLevel = Mathf.Min(energyLevel, chargeLevel + amount);
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


    public void UpdateScore(int amount)
    {
        score += amount;
        CalculateSpeed(); // could also do this in update depending on how it's implemented, current implementation will only ever change with score
    }

    public int GetScore()
    {
        return score;
    }

    public void TogglePause()
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

    public void CalculateSpeed()
    {
        speed = score / 10;
    }

    public float GetSpeed() => speed;
    public float GetSpeedScale() => speedScale;
}