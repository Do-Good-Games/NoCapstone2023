using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController: MonoBehaviour
{
    [Tooltip("The amount to scale the internal distance by when calculating score and speed")]
    [SerializeField] float scoreSpeedScale;
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
    private VerticalProgressBar firedBar1;
    private VerticalProgressBar firedBar2;
    private Label scoreDisplay;
    private Label speedDisplay;
    private VisualElement speedNeedle;
    private VisualElement statusPanel;
    private VisualElement statusPanel2;
    private VisualElement statusPanel3;
    private VisualElement statusPanel4;
    private Button muteButton;
    private float maxHealth;
    private float maxEnergy;

    private int timer;

    private bool isBoosting;


    public float fired;//prototype var to be more cleanly implemented later

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.playerController;

        //UIDoc stuff
        root = UIDoc.rootVisualElement;
        healthBar = root.Q<VerticalProgressBar>("HealthBar");
        energyBar = root.Q<VerticalProgressBar>("EnergyBar");
        chargeBar = root.Q<VerticalProgressBar>("ChargeBar");
        firedBar1 = root.Q<VerticalProgressBar>("FiredBar1");
        firedBar2 = root.Q<VerticalProgressBar>("FiredBar2");

        scoreDisplay = root.Q<Label>("ScoreDisplay");
        speedDisplay = root.Q<Label>("SpeedDisplay");

        speedNeedle = root.Q<VisualElement>("Needle");
        statusPanel = root.Q<VisualElement>("StatusPanel");
        statusPanel2 = root.Q<VisualElement>("StatusPanel2");
        statusPanel3 = root.Q<VisualElement>("StatusPanel3");
        statusPanel4 = root.Q<VisualElement>("StatusPanel4");

        muteButton = UIDoc.rootVisualElement.Q<Button>("MuteButton");

        maxHealth = player.maxHealth;
        maxEnergy = gameManager.GetMaxEnergy();
        healthBar.value = healthBar.highValue;
        energyBar.value = energyBar.lowValue;


        gameManager.OnPlayerHurt.AddListener(UpdateHealthBar);
        gameManager.OnEnergyChange.AddListener(UpdateEnergyBar);
        gameManager.OnChargeChange.AddListener(UpdateChargeBar);
        gameManager.OnFiredChange.AddListener(UpdateFiredBar);

        gameManager.OnBoostStart.AddListener(EmptyFiredBar);
        gameManager.OnBoostEnd.AddListener(EmptyFiredBar);

        muteButton.clicked += MuteClicked;
    }

    private void UpdateChargeBar()
    {

        chargeBar.value = 
            gameManager.GetCharge()/ gameManager.GetMaxEnergy() ;
    }

    void Update()
    {
        float scaledSpeed = Mathf.FloorToInt(gameManager.GetCameraSpeed() * scoreSpeedScale);
        speedDisplay.text = scaledSpeed.ToString() + speedUnits;

        int scaledScore = Mathf.FloorToInt(gameManager.GetScore() * scoreSpeedScale);
        scoreDisplay.text = scaledScore.ToString().PadLeft(5, '0') + scoreUnits;

        speedNeedle.style.rotate = new StyleRotate(new Rotate(new Angle(gameManager.relativeSpeed / gameManager.maxRelativeSpeed * 180 - 90, AngleUnit.Degree)));    //https://docs.unity3d.com/Manual/UIE-Transform.html


        if (gameManager.speedManager.inBoostGracePeriod)
        {
            UpdateFiredBar();
        }
    }

    private void FixedUpdate()
    {
        if(timer < 100)
        {
            timer +=1;
        }
        else
        {
            timer = 0;
        }
        if(timer < 50 && healthBar.value <= .3)
        {
            if(statusPanel4.visible)
            {
                statusPanel4.visible = false;
                statusPanel3.visible = true;
            }
            else
            {
                statusPanel4.visible = true;
                statusPanel3.visible = false;
            }
        }
        if (gameManager.speedManager.inBoost)
        {

        }

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


        firedBar1.value = gameManager.speedManager.inBoostGracePeriod ?
            (gameManager.speedManager.speedAdditionFromBoost / gameManager.speedManager.boostSpeed) * firedBar1.highValue:
            gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar1.highValue;

        firedBar2.value = gameManager.speedManager.inBoostGracePeriod ?
            (gameManager.speedManager.speedAdditionFromBoost / gameManager.speedManager.boostSpeed) * firedBar1.highValue:
            gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar2.highValue;



        //firedBar1.value = gameManager.speedManager.inBoost ?
        //    gameManager.speedManager.remainingRatio * firedBar1.highValue / 2:
        //    gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar1.highValue;

        ////gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar1.highValue;
        //firedBar2.value = gameManager.speedManager.inBoost ?
        //    gameManager.speedManager.remainingRatio * firedBar1.highValue :
        //    gameManager.relativeSpeed / gameManager.maxRelativeSpeed * firedBar2.highValue;

        //Debug.Log("baseSpeed, MaxRelativeSpeed, HighValue " + gameManager.baseSpeed + " " + gameManager.maxRelativeSpeed + " " + firedBar1.highValue);

        if(firedBar1.value == 400)
        {
            firedBar1.value = 0;
            firedBar2.value = 0;
        }
    }

    void EmptyFiredBar()
    {
        Debug.Log("EMPTY FIRED BAR");
        firedBar1.value = 0;
        firedBar2.value = 0;
    }

    private void MuteClicked()
    {
        SFXManager.Instance.isMuted = !SFXManager.Instance.isMuted;
        if(SFXManager.Instance.isMuted)
        {
            muteButton.text = "Unmute";
        }
        else
        {
            muteButton.text = "mute";
        }
        OptionsManager.Instance.CheckMute();
    }
}
