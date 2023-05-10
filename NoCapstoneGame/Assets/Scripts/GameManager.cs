using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent OnPlayerHeal;
    public UnityEvent OnPlayerHurt;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnEnergyChange;
    public UnityEvent OnChargeChange;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        cameraBounds = new Vector2(gameplayCamera.orthographicSize, gameplayCamera.orthographicSize * gameplayCamera.aspect);

        // Initilize playerHealth to 0, the player will call AddPlayerHealth() when the game starts.
        // This allows max health to be configued in the player object or at runtime
        playerHealth = 0;
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
    }

    public void KillPlayer()
    {
        playerHealth = 0;
        OnPlayerDeath.Invoke();
    }

    public int GetPlayerHealth(int amount)
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
}