using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Speed Prototyping/all", fileName = "all")]
public class SPAll : SPSOBase
{

    
    
    public override void SPEnergyHeld() //currently calling once
    {
        if (BByEnergyHeld)
        {
            Debug.Log("adjusting speed by energy held");

        }
    }

    public override void SPEnergyCollected() //do we want to scale this amount by energy collected amounts?
    {
        if (BByEnergyCollected)
        {
            //if(prevEnergy > currentEnergy
            Debug.Log("adjusting speed by energy collected");
        }
    }

    public override void SPEnergyFired(float charge)
    {
        if (BByEnergyFired)
        {

            Debug.Log("adjusting speed by energy fired");
        }
    }

    public override void SPHit()
    {
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
        if (BByTime)
        {

            Debug.Log("adjusting speed over time"); //do we perhaps want to scale this by energy levels?
        }
    }

}
