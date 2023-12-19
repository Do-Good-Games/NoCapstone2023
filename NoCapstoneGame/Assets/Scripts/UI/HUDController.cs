using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController: MonoBehaviour
{
    [SerializeField] string speedUnits = "kph";
    [SerializeField] string scoreUnits = "km";
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
    private VisualElement speedNeedle;
    private VisualElement statusPanel;
    private VisualElement statusPanel2;
    private VisualElement statusPanel3;
    private VisualElement statusPanel4;
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

        speedNeedle = root.Q<VisualElement>("Needle");
        statusPanel = root.Q<VisualElement>("StatusPanel");
        statusPanel2 = root.Q<VisualElement>("StatusPanel2");
        statusPanel3 = root.Q<VisualElement>("StatusPanel3");
        statusPanel4 = root.Q<VisualElement>("StatusPanel4");

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
        speedDisplay.text = gameManager.GetCameraSpeed().ToString() + speedUnits;
        scoreDisplay.text = ((int)gameManager.GetScore()).ToString().PadLeft(5, '0') + scoreUnits;
        speedNeedle.style.rotate = new StyleRotate(new Rotate(new Angle(gameManager.relativeSpeed / gameManager.maxRelativeSpeed * 180 - 90, AngleUnit.Degree)));    //https://docs.unity3d.com/Manual/UIE-Transform.html
    }

    void UpdateHealthBar()
    {
        healthBar.value = (float) ((float)gameManager.GetPlayerHealth()/(float)maxHealth);
        //transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);


        //set the bottom left panel sprite
        if(healthBar.value <= .2)
        {
            statusPanel.visible = false;
            statusPanel2.visible = false;
            statusPanel3.visible = false;
            statusPanel4.visible = true;
        }
        else if(healthBar.value <= .5)
        {
            statusPanel.visible = false;
            statusPanel2.visible = false;
            statusPanel3.visible = true;
        }
        else if(healthBar.value <= .8)
        {
            statusPanel.visible = false;
            statusPanel2.visible = true;
        }

        //statusPanel.style.backgroundImage = null;
    }

    void UpdateEnergyBar()
    {

        energyBar.value = (float) ((float)gameManager.GetEnergy()/(float)maxEnergy);
        //transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
    }

    void UpdateFiredBar()
    {
        firedBar.value = gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar.highValue; //cleanup: remove
    }

}
