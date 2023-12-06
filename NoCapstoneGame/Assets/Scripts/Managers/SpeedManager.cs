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
    [SerializeField] private float speed;
    [SerializeField] private bool bBoosting;
    [SerializeField] private float BoostMultiplier;


    [SerializeField] private float collected;
    [SerializeField] private float held;
    [SerializeField] public float fired { get; private set;  }
    private float currTime;

    [SerializeField] public float maxFired { get; private set; }

    public float GetSpeed()
    {
        if (bBoosting)
        {
            return boostSpeed;
        } else
        {
            //currently setting this in updateEnergy(), but we could also set it here 
            //held = BByEnergyHeld ? gameManager.GetEnergy() * PerEnergyHeld : 0;

            speed = fired + held + currTime + (numOfBoosts * speedOnExit);
            return speed;
        }
    }

    #region Speed Vars

    [Header("speed increase balance variables")]
    [SerializeField] protected bool BByEnergyHeld;
    [SerializeField] protected float PerEnergyHeld;

    [SerializeField] protected bool BByEnergyCollected;
    [SerializeField] protected float PerEnergyCollected;

    [SerializeField] protected bool BByEnergyFired;
    [SerializeField] protected float PerEnergyFired;

    [SerializeField] protected bool BByTime;
    [SerializeField] protected float PerSecond;

    protected float prevEnergyLevel;

    [Header("amount of each variable lost on hit")]
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float heldLostOnHit = 1;
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float collectedLostOnHit = 1;
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float firedLostOnHit = 1;
    //[SerializeField] private float  =1;


    #endregion


    #region boost vars
    protected enum SpeedOnExitType { Ratio, Static } //after prototyping consider having all value types be done as one universal enum

    [Header("boost variables")]
    [SerializeField] protected float boostSpeed = 100;
    [SerializeField] protected float minBoostEnergy = 0;
    [SerializeField] protected float boostEnergyLostPerSecond = 20;

    [SerializeField] protected bool incrByNumOfBoosts;

    [SerializeField] protected int numOfBoosts;

    [SerializeField] protected SpeedOnExitType speedOnExitType;
    [SerializeField] protected float speedOnExit;

    #endregion boost vars

    private GameManager gameManager;

    private IEnumerator BoostCoroutineObject;

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (BByTime)
        {
            currTime += Time.deltaTime * PerSecond;

            speed += Time.deltaTime * PerSecond;
            //Debug.Log("adjusting speed over time"); //do we perhaps want to scale this by energy levels?
        }

        if(gameManager.speed >= gameManager.maxSpeed)
        {
            //ActivateBoost();
        }

    }

    public void UpdateEnergy()
    {
        Debug.Log("unless we see this line pop up, we can remove this overload");
        //updateEnergy()
    }

    public void UpdateEnergy(bool increment)
    {
        //if (increment)
        //{
        //    collected++;
        //    held++;
        //}

        if (BByEnergyHeld)
        {
            //if we so chose, we could also set the held variable here rather than in getSpeed()
            held = gameManager.GetEnergy() * PerEnergyHeld;
            
            /*
            if (increment)
            {
                //speed += PerEnergyHeld;
                held++;
            }
            else
            {
                held--;
                //speed -= PerEnergyHeld;
            }*/
        }

        if (BByEnergyCollected && increment)
        {
            collected += PerEnergyCollected;
        }

        if (increment && (gameManager.speed > gameManager.maxSpeed) )
        {
            //ActivateBoost();
        }
    }

    public void Fired( float charge)
    {
        if (fired + charge < GameManager.Instance.GetEnergy())//if we're still below GM's energy, IE we want to increase our values
        {
            fired += BByEnergyFired ? charge : 0;
            held -= BByEnergyHeld ? charge : 0;
            
            gameManager.OnFiredChange.Invoke();
            
            /*if(fired >= gameManager.GetMaxEnergy()){
                ActivateBoost();
            }*/
        }

        //this was how I did it in speed prototype, the active code (above) was adopted from the boost prototype
        /*
        if (BByEnergyFired)
        {
            fired++;
            held -= charge;
            //speed += PerEnergyFired;
            
            //Debug.Log("espeed2 " + speed);
            //Debug.Log("adjusting speed by energy fired");
        }*/

    }

    public void Hit()
    {

        if (BByEnergyHeld)
        {
            //speed -= held *  heldLostOnHit * PerEnergyHeld;
            held -= held * heldLostOnHit;
        }

        if (BByEnergyFired)
        {
            //speed -= fired * firedLostOnHit * PerEnergyFired;
            fired -= fired * firedLostOnHit;
        }

        if (BByEnergyCollected)
        {
            //speed -= collected;
            collected -= collected * collectedLostOnHit;
        }

    }

    public void ResetVariables()
    {
        held = 0;
        collected = 0;
        fired = 0;
        gameManager.OnFiredChange.Invoke();
        speed = 0;
        bBoosting = false;
    }

    //used for if we want to reset the number of boosts (aka speed baseline) as well
    public void ResetVariables(bool fullReset){
        ResetVariables();
        numOfBoosts = (fullReset == true ? 0 : numOfBoosts);
    }


    //this is the function you'll need to refactor. 
    public void ActivateBoost()
    {
        bBoosting = true;
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
        gameManager.StartBoost();
        numOfBoosts++;
        while (gameManager.GetEnergy() > minBoostEnergy)
        {
            //for some reason this removes the energy twice as quickly as it should
            gameManager.UpdateEnergy(- boostEnergyLostPerSecond * Time.deltaTime);
            gameManager.UpdateCharge(-boostEnergyLostPerSecond * Time.deltaTime);
            fired -= boostEnergyLostPerSecond * Time.deltaTime;
            gameManager.OnFiredChange.Invoke();

            yield return null;
        }
        gameManager.EndBoost();
        ResetVariables();
    }
}
