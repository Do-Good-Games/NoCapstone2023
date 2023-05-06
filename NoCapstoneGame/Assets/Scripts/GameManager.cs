using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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

        // Initilize playerHealth to 0, the player will call AddPlayerHealth() when the game starts.
        // This allows max health to be configued in the player object or at runtime
        playerHealth = 0;
    }


    AddPlayerHealth(int amount)
    {
        playerHealth += amount;
        OnPlayerHeal.Invoke();
    }

    RemovePlayerHealth(int amount)
    {
        playerHealth -= amount;
        OnPlayerHurt.Invoke();
    }

    KillPlayer()
    {
        playerHealth = 0;
        OnPlayerDeath.Invoke();
    }


    UpdateEnergy(int amount)
    {
        energyLevel += amount;
        OnEnergyChange.Invoke();
    }

    ResetEnergy()
    {
        energyLevel = 0;
        OnEnergyChange.Invoke();
    }


    UpdateCharge(int amount)
    {
        chargeLevel += amount;
        OnChargeChange.Invoke();
    }

    ResetCharge()
    {
        chargeLevel = 0;
        OnChargeChange.Invoke();
    }


    UpdateScore(int amount)
    {
        score += amount;
    }
}
