using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class SpeedManager : MonoBehaviour
{
    #region Variables

    [Header("fundamentals")]
    [SerializeField] private PlayerController playerController;

    //[Tooltip("the Current speed as calculated by the speed manager")]
    //[SerializeField] private float BoostMultiplier; //refactor to boostSpeed from playerCont


    //[SerializeField] public float fired { get; private set;  }
    //private float currTime;

    //[SerializeField] public float maxFired { get; private set; }

    #region Speed Vars

    [Header("speed increase balance variables")]


    [Tooltip("a value to determine how quickly relative speed increases when the player increases the amount fired")]
    [SerializeField] private float speedIncreaseScale;


    protected float prevEnergyLevel;

    [Tooltip("the amount of energy the player collects when they have ZERO energy during the boost")]
    [SerializeField] private float lowerThresholdForDimRet;
    [Tooltip("the smount of energy the player collects when they have FULL energy during the boost")]
    [SerializeField] private float upperThresholdForDimRet;

    [SerializeField] private AnimationCurve dimRetCurve;


    #endregion


    #region boost vars

    [Header("boost variables")]
    public float boostSpeed = 100;
    [SerializeField] protected float minBoostEnergy = 0;
    [SerializeField] protected float boostEnergyLostPerSecond = 20;

    [SerializeField] protected int numOfBoosts;

    [SerializeField] protected float speedOnExit;

    public bool inBoost { get; private set; }
    public bool inBoostGracePeriod { get; private set; }
    public float speedAdditionFromBoost { get; private set; }
    [Tooltip("how long (in seconds) after the player exits boost that they're immune (but don't destroy asteroids)")]
    [SerializeField] private float boostGracePeriodDuration;

    #endregion boost vars

    private GameManager gameManager;

    private IEnumerator BoostCoroutineObject;
    private IEnumerator boostGracePeriodCoroutineObject;
    public float remainingRatio;

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        inBoost = false;
    }
    // Update is called once per frame
    void Update()
    {

        //if (BByTime)
        //{
        //    currTime += Time.deltaTime * PerSecond;

        //    //Debug.Log("adjusting speed over time"); //do we perhaps want to scale this by energy levels?
        //}
    }

    public void Fired(float charge)
    {
        float speedRatio = gameManager.relativeSpeed / gameManager.maxRelativeSpeed; //a ratio from 0 to 1 representing how much of the total the player's relative speed currently is
        float energyRatio = gameManager.GetEnergy() / gameManager.GetMaxEnergy(); //ratio from 0-1 representing how much of the total the player's currently energy is
        charge = charge * speedIncreaseScale; //scale charge by the increase scale

        //float speedRatio = gameManager.relativeSpeed + charge / gameManager.maxRelativeSpeed; //a ratio from 0 to 1 representing how much of the total the player's relative speed currently is
        //INCLUDING how much speed the player will have if we increase their charge

        if (speedRatio < energyRatio) //if the amount of speed the player has is less than the amount of energy they have (porportional to the respective totals)
        {
            gameManager.UpdateRelativeSpeed(charge);
        }
    }

    public void Hit()
    {
        //    if (playerController.mouse)
        //    {
        //        gameManager.UpdateFired(-gameManager.GetCharge());
        //    }
    }
    public void ResetVariables()
    {
        gameManager.OnFiredChange.Invoke();
    }

    //used for if we want to reset the number of boosts (aka speed baseline) as well
    public void ResetVariables(bool fullReset){ 
        ResetVariables();
        numOfBoosts = (fullReset == true ? 0 : numOfBoosts);
    }


    //this is the function you'll need to refactor. 
    public void ActivateBoost()
    {
        if (BoostCoroutineObject is not null)
        {
            StopCoroutine(BoostCoroutineObject);
        }
        gameManager.StartBoost();
        BoostCoroutineObject = BoostCoroutine();
        StartCoroutine(BoostCoroutineObject);
    }

    public IEnumerator BoostCoroutine()
    {
        inBoost = true;
        speedAdditionFromBoost = boostSpeed;
        numOfBoosts++;
        while (gameManager.GetEnergy() > minBoostEnergy) //if relative speed >= max relative speed you can charge.
        {
            //for some reason this removes the energy twice as quickly as it should
            gameManager.UpdateEnergy(- boostEnergyLostPerSecond * Time.deltaTime);
            gameManager.UpdateCharge(-boostEnergyLostPerSecond * Time.deltaTime);
            gameManager.OnFiredChange.Invoke();

            yield return null;
        }

        inBoostGracePeriod = true;
        float timeSinceGracePeriodStart = Time.time;
        while (Time.time - timeSinceGracePeriodStart <= boostGracePeriodDuration)
        {
            speedAdditionFromBoost = Mathf.Lerp(boostSpeed, 0, (Time.time - timeSinceGracePeriodStart) / boostGracePeriodDuration);
            yield return new WaitForSeconds(.1f);
        }
        gameManager.ResetEnergy();
        inBoostGracePeriod = false;

        inBoost = false;

        gameManager.EndBoost(numOfBoosts, speedOnExit);
        ResetVariables();
    }

    public void CollectEnergyInBoost(float charge)
    {

        //float remainingRatio = (gameManager.GetEnergy() / gameManager.GetMaxEnergy()); //use this line to ONLY calculate by energy
        //remainingRatio = (gameManager.GetEnergy() / 2*gameManager.GetMaxEnergy() + gameManager.GetCharge() / 2*gameManager.GetMaxEnergy());
        float remainingRatio = (gameManager.GetEnergy() + gameManager.GetCharge()) / (2 * gameManager.GetMaxEnergy()); //use this line to calculate by energy and charge  

        //(gameManager.GetEnergy()  + gameManager.GetCharge()) / (2 * gameManager.GetMaxEnergy()); //use this line to calculate by energy and charge
        //old range = 1
        float dimRetRange = upperThresholdForDimRet - lowerThresholdForDimRet;
        //float dimRetRatio = Mathf.Lerp(lowerThresholdForDimRet, upperThresholdForDimRet, remainingRatio);
        float dimRetRatio = dimRetCurve.Evaluate(remainingRatio);

        //((remainingRatio* dimRetRange) + lowerThresholdForDimRet); //
        //the ratio between the diminishing return upper and lower value,
        //calculated by the amount of energy the player has. if the player has full energy, this value will be upperThresholdForDimRet, as the amount decreases,
        //it will approach the lowerThreshold
        //Debug.Log("energy amount" + gameManager.GetEnergy() + " remaining ratio: " + remainingRatio + " dimRetRatio " + dimRetRatio);
        gameManager.UpdateEnergy(dimRetRatio * charge);

    }
}
