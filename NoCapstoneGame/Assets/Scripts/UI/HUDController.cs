using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController: MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] UIDocument UIDoc;
    [SerializeField] Event triggerEvent;

    GameManager gameManager;

    private VisualElement root;
    private VerticalProgressBar healthBar;
    private VerticalProgressBar energyBar;
    private VerticalProgressBar chargeBar;
    private VerticalProgressBar firedBar;
    private Label scoreDisplay;
    private Label speedDisplay;
    private float maxHealth;
    private float maxEnergy;

    public float fired;//prototype var to be more cleanly implemented later

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;   

        //UIDoc stuff
        root = UIDoc.rootVisualElement;
        healthBar = root.Q<VerticalProgressBar>("HealthBar");
        energyBar = root.Q<VerticalProgressBar>("EnergyBar");
        chargeBar = root.Q<VerticalProgressBar>("ChargeBar");
        firedBar = root.Q<VerticalProgressBar>("FiredBar");

        scoreDisplay = root.Q<Label>("ScoreDisplay");
        speedDisplay = root.Q<Label>("SpeedDisplay");

        maxHealth = player.maxHealth;
        maxEnergy = gameManager.GetMaxEnergy();
        healthBar.value = healthBar.highValue;
        energyBar.value = energyBar.lowValue;


        gameManager.OnPlayerHurt.AddListener(UpdateHealthBar);
        gameManager.OnEnergyChange.AddListener(UpdateEnergyBar);
        gameManager.OnChargeChange.AddListener(UpdateChargeBar);
        gameManager.OnFiredChange.AddListener(UpdateFiredBar);
    }

    private void UpdateChargeBar()
    {
        chargeBar.value =gameManager.GetCharge()/ gameManager.GetMaxEnergy() ;
    }

    void Update()
    {
        scoreDisplay.text = gameManager.GetScore().ToString().PadLeft(5, '0');
        speedDisplay.text = gameManager.GetSpeed().ToString() + "kph";
    }

    void UpdateHealthBar()
    {
        healthBar.value = (float) ((float)gameManager.GetPlayerHealth()/(float)maxHealth);
        //transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
    }

    void UpdateEnergyBar()
    {

        energyBar.value = (float) ((float)gameManager.GetEnergy()/(float)maxEnergy);
        //transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
    }

    void UpdateFiredBar()
    {
        firedBar.value = player.speedManager.fired;
    }

}
