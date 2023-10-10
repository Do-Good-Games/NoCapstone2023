using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Speed Prototyping/all", fileName = "all")]
public class SPAll : SPSOBase
{

    [SerializeField] private float collected = 0;
    [SerializeField] private float held = 0;
    [SerializeField] private float fired=0;

    private float timeSinceGameStart;
    
    public override void SPEnergyHeld() //currently calling once
    {
        //held = (int) GameManager.Instance.GetEnergy();
        if(GameManager.Instance.GetEnergy() != 0)
        {
            held++;
        }
        if (BByEnergyHeld)
        {
            Debug.Log("held " + held);

            speed += PerEnergyHeld;
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
            Debug.Log("speed " + speed);
        }
    }

    public override void SPEnergyFired(float charge)
    {
        fired++;
        held -= charge;
        if (BByEnergyFired)
        {
            speed += fired * PerEnergyFired; //finish implementing these

            Debug.Log("adjusting speed by energy fired");
        }
    }

    public override void SPHit()
    {
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

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetVariables();
    }

}
