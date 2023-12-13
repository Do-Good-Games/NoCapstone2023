using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class SpeedManager : MonoBehaviour
{
    #region Variables

    [Header("fundamentals")]
    [SerializeField] private PlayerController playerController;

    [Tooltip("the Current speed as calculated by the speed manager")]
    //[SerializeField] private float BoostMultiplier; //refactor to boostSpeed from playerCont


    //[SerializeField] public float fired { get; private set;  }
    //private float currTime;

    //[SerializeField] public float maxFired { get; private set; }

    #region Speed Vars

    [Header("speed increase balance variables")]
    //[SerializeField] protected bool BByEnergyHeld;
    //[SerializeField] protected float PerEnergyHeld;

    //[SerializeField] protected bool BByEnergyCollected;
    //[SerializeField] protected float PerEnergyCollected;

    [SerializeField] protected bool BByEnergyFired;
    [SerializeField] protected float PerEnergyFired;

    //[SerializeField] protected bool BByTime;
    //[SerializeField] protected float PerSecond;

    protected float prevEnergyLevel;

    //[Header("amount of each variable lost on hit")]
    //[Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    //[SerializeField] private float heldLostOnHit = 1;
    //[Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    //[SerializeField] private float collectedLostOnHit = 1;
    //[Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    //[SerializeField] private float firedLostOnHit = 1;
    //[SerializeField] private float  =1;


    #endregion


    #region boost vars
    protected enum SpeedOnExitType { Ratio, Static } //after prototyping consider having all value types be done as one universal enum

    [Header("boost variables")]
    public float boostSpeed = 100;
    [SerializeField] protected float minBoostEnergy = 0;
    [SerializeField] protected float boostEnergyLostPerSecond = 20;

    [SerializeField] protected int numOfBoosts;

    [SerializeField] protected SpeedOnExitType speedOnExitType;
    [SerializeField] protected float speedOnExit;

    public bool inBoost { get; private set; }
    public bool inBoostGracePeriod { get; private set; }
    [Tooltip("how long (in seconds) after the player exits boost that they're immune (but don't destroy asteroids)")]
    [SerializeField] private float boostGracePeriodDuration;

    #endregion boost vars

    private GameManager gameManager;

    private IEnumerator BoostCoroutineObject;
    private IEnumerator boostGracePeriodCoroutineObject;

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
        if(gameManager.relativeSpeed + charge < gameManager.GetEnergy())
        {

            gameManager.UpdateFired(charge );
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
        Debug.Log("boost method activated");
        if (BoostCoroutineObject is not null)
        {
            StopCoroutine(BoostCoroutineObject);
        }
        BoostCoroutineObject = BoostCoroutine();
        StartCoroutine(BoostCoroutineObject);
    }

    public IEnumerator BoostCoroutine()
    {
        Debug.Log("boost coroutine activated");
        inBoost = true;
        numOfBoosts++;
        while (gameManager.GetEnergy() > minBoostEnergy)
        {
            //for some reason this removes the energy twice as quickly as it should
            gameManager.UpdateEnergy(- boostEnergyLostPerSecond * Time.deltaTime);
            gameManager.UpdateCharge(-boostEnergyLostPerSecond * Time.deltaTime);
            gameManager.OnFiredChange.Invoke();

            yield return null;
        }
        inBoost = false;

        boostGracePeriodCoroutineObject = BoostGracePeriod();
        StartCoroutine(boostGracePeriodCoroutineObject);

        gameManager.EndBoost(numOfBoosts, speedOnExit);
        ResetVariables();
    }

    private IEnumerator BoostGracePeriod()
    {
        inBoostGracePeriod = true;
        float timeSinceGracePeriodStart = Time.time;
        Debug.Log("tsgps: " + timeSinceGracePeriodStart);
        while (Time.time - timeSinceGracePeriodStart <= boostGracePeriodDuration)
        {
            Debug.Log("tsgps1: " + timeSinceGracePeriodStart);
            yield return new WaitForSeconds(.1f);
        }
        gameManager.ResetEnergy();
        inBoostGracePeriod = false;
        Debug.Log("ended coroutine");

    }

}
