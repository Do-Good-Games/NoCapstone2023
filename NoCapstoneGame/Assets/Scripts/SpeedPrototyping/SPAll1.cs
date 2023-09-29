using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAll1 : SPSOBase
{
    
    public override void SPEnergyHeld()
    {
        if (BByEnergyHeld)
        {

        }
    }

    public override void SPEnergyCollected()
    {
        if (BByEnergyCollected)
        {

        }
    }

    public override void SPHit()
    {
        if (BByEnergyCollected)
        {

        }
    }

    public override void SPEnergyFired(float charge)
    {
        if (BByEnergyFired)
        {

        }
    }

    public override void SPOverTime()
    {
        if(hitLossType == HitLossType.Ratio)
        {

        } else if(hitLossType == HitLossType.Static)
        {

        }
    }

    public override void ResetVariables()
    {
        throw new System.NotImplementedException();
    }
}
