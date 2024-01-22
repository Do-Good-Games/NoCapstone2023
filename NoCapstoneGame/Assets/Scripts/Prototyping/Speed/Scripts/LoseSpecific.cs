using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//half and half how much we currently hold and how much we collected
[CreateAssetMenu( menuName = "Speed Prototyping/LoseSpecific")]
public class LoseSpecific : SPSOBase
{
    [Header("amount of each variable lost on hit")]
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float heldLostOnHit =1;
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float collectedLostOnHit =1;
    [Tooltip("how much of the speed calculated by amount held will be lost when hit - value from 0 to 1")]
    [SerializeField] private float firedLostOnHit = 1;
    //[SerializeField] private float  =1;

    [Header("held and collected vars")]

    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    private float collectedWeight = 1;
    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    private float heldWeight = 1;
    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    private float firedWeight = 1;


    [Tooltip("how much do we want the collected portion of our speed calculation to decrease when the player gets hit scale of 0 (none) to 1 (all)")]
    private float colLossOnHit = 0;

    [Tooltip("calculated as each weight added up, used by dividing the wight by this to determine value")]
    private float weightTotal;

    [Header("debugging/tracking variables")]

    [SerializeField] private float collected;
    [SerializeField] private float held;
    [SerializeField] private float fired;

    private float timeSinceGameStart;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        weightTotal = collectedWeight + heldWeight + firedWeight;
        ResetVariables();
    }

    //reimplemented, not reref'd, not necessary any more
    public override void SPEnergyHeld() //currently calling once
    {
        Debug.Log("not inc");
        //held = (int) GameManager.Instance.GetEnergy();
        if (GameManager.Instance.GetEnergy() != 0) //I might be doing this wrong - or it might be an indication I'm doing something else wrong
        {
            held++;
        }
        if (BByEnergyHeld)
        {
            Debug.Log("held " + held);

            speed += PerEnergyHeld;
            //speed += PerEnergyHeld * (heldWeight/ weightTotal);
            //Debug.Log("speed " + speed);

        }
    }

    //reimplemented, not reref'd, now updateEnergy
    public override void SPEnergyHeld(bool increment)
    {
        Debug.Log("incremented" + increment);
        if (increment)
        {

            held++;
        }
        if (BByEnergyHeld)
        {
            Debug.Log("held " + held);

            if(increment)
            {
                speed += PerEnergyHeld;
                //speed += PerEnergyHeld * (heldWeight/weightTotal);
            } else
            {
                speed -= PerEnergyHeld;
                //speed -= PerEnergyHeld * (heldWeight / weightTotal);
                //speed =Mathf.Max( speed - PerEnergyHeld, 0);//bit dirty, buuuuuut 
            } 


           // Debug.Log("speed " + speed);

        }
    }

    //reimplemented, not reref'd, now updateEnergy
    public override void SPEnergyCollected() //do we want to scale this amount by energy collected amounts?
    {
        collected++;
        if (BByEnergyCollected)
        {
            //if(prevEnergy > currentEnergy
            Debug.Log("collected " + collected);

            speed += PerEnergyCollected;
            //speed += PerEnergyCollected * (collectedWeight / weightTotal);
            //Debug.Log("speed " + speed);
        }
    }

    //refacd, not rerefd, now Fired()
    public override void SPEnergyFired(float charge)
    {
        fired++;
        held -= charge;
        Debug.Log("espeed1 " + speed);
        if (BByEnergyFired)
        {
            speed += PerEnergyFired;
            //speed += PerEnergyFired * (firedWeight /weightTotal);
            Debug.Log("espeed2 " + speed);


            Debug.Log("adjusting speed by energy fired");
        }
    }

    //refacd, not rerefd, now Hit()
    public override void SPHit()
    {
        if (BByEnergyHeld)
        {
            speed -= held * heldWeight * heldLostOnHit * PerEnergyHeld;
            held -= held * heldLostOnHit;
        }

        if (BByEnergyFired)
        {
            speed -= fired * firedWeight * firedLostOnHit * PerEnergyFired;
            fired -= fired * firedLostOnHit;
        }

        if (BByEnergyCollected)
        {
            speed -= collected * collectedWeight;
            collected -= collected * collectedLostOnHit;
        }


        //if (BByEnergyHeld)
        //{
        //    held = 0;
        //    speed = speed 
            
            
        //    //speed -= held * PerEnergyHeld;
        //    //speed -= held * (heldWeight / weightTotal);
        //}

        //if (BByEnergyCollected)
        //{

        //    //7 * 1 + 4
        //    float colLost = collected * colLossOnHit * PerEnergyCollected;
        //    //float colLost = collected * colLossOnHit * (collectedWeight / weightTotal);
        //    Debug.Log(colLost);
        //    speed -= colLost;
        //    collected = collected * PerEnergyCollected - colLost;
        //}


        if (hitLossType == HitLossType.Ratio)
        {
            speed = speed * AmountLostOnHit;
        }
        else if (hitLossType == HitLossType.Static)
        {
            speed = Mathf.Max(speed - AmountLostOnHit, 0);
        }


    }

    //refac'd, reref'd, old ref not removed now done directly in update, 
    public override void SPOverTime()
    {
        if (speed < 0){
            //Debug.Log("ruh roh raggy");
        }

        if (BByTime)
        {
            speed += Time.deltaTime * timeScale;
            //Debug.Log("adjusting speed over time"); //do we perhaps want to scale this by energy levels?
        }
    }

    public override void ResetVariables()
    {
        held = 0;
        collected = 0;
        fired = 0;
        speed = 0;
    }


}
