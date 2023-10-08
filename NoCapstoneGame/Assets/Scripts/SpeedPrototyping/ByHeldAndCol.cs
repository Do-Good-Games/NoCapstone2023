using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//half and half how much we currently hold and how much we collected
[CreateAssetMenu( menuName = "Speed Prototyping/byHeldAndCollected", fileName = "byHeldAndCollected")]
public class ByHeldAndCol : SPSOBase
{
    [Header("held and collected vars")]

    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    [SerializeField] private float collectedWeight;
    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    [SerializeField] private float heldWeight;
    [Tooltip("a ratio of how much each value should be weighed against one another. ")]
    [SerializeField] private float firedWeight;


    [Tooltip("how much do we want the collected portion of our speed calculation to decrease when the player gets hit scale of 0 (none) to 1 (all)")]
    [SerializeField] private float colLossOnHit;

    [Tooltip("calculated as each weight added up, used by dividing the wight by this to determine value")]
    private float weightTotal;

    [Header("debugging/tracking variables")]

    [SerializeField] private float collected = 0;
    [SerializeField] private float held = 0;
    [SerializeField] private float fired=0;

    private float timeSinceGameStart;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        weightTotal = collectedWeight + heldWeight + firedWeight;
        ResetVariables();
    }


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
            Debug.Log("speed " + speed);

        }
    }

    public override void SPEnergyHeld(bool increment)
    {
        Debug.Log("incremented");
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


            Debug.Log("speed " + speed);

        }
    }

    public override void SPEnergyCollected() //do we want to scale this amount by energy collected amounts?
    {
        collected++;
        if (BByEnergyCollected)
        {
            //if(prevEnergy > currentEnergy
            Debug.Log("collected " + collected);

            speed += PerEnergyCollected;
            //speed += PerEnergyCollected * (collectedWeight / weightTotal);
            Debug.Log("speed " + speed);
        }
    }

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

    public override void SPHit()
    {

        //7 * 1 + 4
        float colLost = collected * colLossOnHit * PerEnergyCollected;
        //float colLost = collected * colLossOnHit * (collectedWeight / weightTotal);
        Debug.Log(colLost);
        speed -= colLost;
        collected = collected *PerEnergyCollected - colLost;

        speed -= held * PerEnergyHeld;
        //speed -= held * (heldWeight / weightTotal);
        held = 0;
        if (hitLossType == HitLossType.Ratio)
        {
            Debug.Log("adjusting speed by energy held - ratio version");

        }
        else if (hitLossType == HitLossType.Static)
        {
            Debug.Log("adjusting speed by energy held - static version");

        }
    }


    public override void SPOverTime()
    {
        if (speed < 0){
            Debug.Log("ruh roh raggy");
        }

        if (BByTime)
        {
            speed += Time.timeSinceLevelLoad * timeScale;
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
