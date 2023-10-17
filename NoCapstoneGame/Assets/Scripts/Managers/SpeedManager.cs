using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SpeedManager : MonoBehaviour
{
    #region Variables

    [Header("fundamentals")]
    [SerializeField] private PlayerController playerController;

    [Tooltip("the Current speed as calculated by the speed manager")]
    [SerializeField] private float speed;
    [SerializeField] private bool Boosting;
    [SerializeField] private float BoostMultiplier;


    [SerializeField] private float collected;
    [SerializeField] private float held;
    [SerializeField] private float fired;


    public float getSpeed() => Boosting? speed : speed * BoostMultiplier;

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
    [SerializeField] protected bool incrByNumOfBoosts;

    [SerializeField] protected int numOfBoosts;

    [SerializeField] protected SpeedOnExitType speedOnExitType;
    [SerializeField] protected float speedOnExit;

    #endregion boost vars

    private GameManager gameManager;

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        playerController = gameObject.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

        if (BByTime)
        {
            speed += Time.deltaTime * PerSecond;
            //Debug.Log("adjusting speed over time"); //do we perhaps want to scale this by energy levels?
        }

    }

    public void UpdateEnergy()
    {
        //updateEnergy()
    }

    public void updateEnergy(bool increment)
    {
        if (increment)
        {
            collected++;
            held++;
        }
        if (BByEnergyHeld)
        {
            if (increment)
            {
                speed += PerEnergyHeld;
            }
            else
            {
                speed -= PerEnergyHeld;
            }
        }
        if (BByEnergyCollected && increment)
        {
            speed += PerEnergyCollected;
        }
    }

    public void Fired( float charge)
    {
        fired++;
        held -= charge;

        if (BByEnergyFired)
        {
            speed += PerEnergyFired;
            //Debug.Log("espeed2 " + speed);
            //Debug.Log("adjusting speed by energy fired");
        }

    }

    public void Hit()
    {

        if (BByEnergyHeld)
        {
            speed -= held *  heldLostOnHit * PerEnergyHeld;
        }
        held -= held * heldLostOnHit;

        if (BByEnergyFired)
        {
            speed -= fired * firedLostOnHit * PerEnergyFired;
        }
        fired -= fired * firedLostOnHit;

        if (BByEnergyCollected)
        {
            speed -= collected;
        }
        collected -= collected * collectedLostOnHit;

    }

    public void ResetVariables()
    {
        held = 0;
        collected = 0;
        fired = 0;
        speed = 0;
    }


    //this is the function you'll need to refactor. 
    public void ActivateBoost()
    {

        numOfBoosts++; //keep this line 

        #region keep
        //keep the code in this region, feel free to refactor it but use it as the baseline for what to do when exiting the speed boost
        //I imagine you'll do this as a coroutine, put this at the end of the coroutine
        ResetVariables();

        if (speedOnExitType == SpeedOnExitType.Static)
        {
            if (incrByNumOfBoosts)
            {
                float newspeed = speedOnExit * numOfBoosts;
                speed = newspeed;


            }
            else
            {
                speed = speedOnExit;
            }
        }
        else if (speedOnExitType == SpeedOnExitType.Ratio)
        {
            if (incrByNumOfBoosts)
            {
                speed = GameManager.Instance.speed * speedOnExit * numOfBoosts;

            }
            else
            {
                speed = speedOnExit * GameManager.Instance.speed;
            }
        }
        #endregion keep


        //you're probably not going to want to keep this, in fact I'd advise against it
        int currScore = GameManager.Instance.GetScore();

        GameManager.Instance.UpdateScore(-currScore);//resets score
        GameManager.Instance.UpdateScore(Mathf.FloorToInt((float)currScore * BoostMultiplier));//multiplies score by modifier, rounding down

        GameManager.Instance.ResetEnergy();
        Debug.Log("speed boost! -----------");

    }

}
