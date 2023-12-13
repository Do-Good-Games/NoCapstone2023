using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DESCRIPTION: when energy reaches max, doubles score and resets speed to value determined by inspector vars

[CreateAssetMenu(menuName = "boost prototyping/ fullEnergy")]
public class SOBoostSimple : SOBoostBase
{


    public override void energyFull()
    {
        activate();
    }

    public override void activate()
    {
        //figure out how to set GM's speed, which might require getting a reference to the speed SO?

        speedPrototype.ResetVariables();
        ResetVariables();


        numOfBoosts++;
        if (speedOnExitType == SpeedOnExitType.Static)
        {
            if (incrByNumOfBoosts)
            {
                float newspeed = speedOnExit * numOfBoosts;
                speedPrototype.speed  = newspeed;
                
                
            } else
            {
                speedPrototype.speed = speedOnExit;
            }
        } else if (speedOnExitType == SpeedOnExitType.Ratio)
        {
            if (incrByNumOfBoosts)
            {
                speedPrototype.speed = GameManager.Instance.relativeSpeed *  speedOnExit * numOfBoosts;

            }
            else
            {
                speedPrototype.speed = speedOnExit * GameManager.Instance.relativeSpeed;
            }
        }

        //you can probably throw these following lines out, in fact I'd probably advise doing so
        int currScore = GameManager.Instance.GetScore();

        GameManager.Instance.UpdateScore( -currScore);//resets score
        GameManager.Instance.UpdateScore(Mathf.FloorToInt((float)currScore * scoreMultiplier));//multiplies score by modifier, rounding down

        GameManager.Instance.ResetEnergy();
        Debug.Log("speed boost! -----------");
    }

    public override void ChargeContinue()
    {
        //not necessary in this prototype
    }

    public override void ChargeFinish()
    {
        //not necessary in this prototype
    }

    public override void ChargeStart()
    {
        //not necessary in this prototype
    }

    public override void checkActivate()
    {
        //not necessary in this prototype
    }
}
