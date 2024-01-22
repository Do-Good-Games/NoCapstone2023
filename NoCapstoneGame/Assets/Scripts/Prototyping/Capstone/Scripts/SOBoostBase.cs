using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SOBoostBase : ScriptableObject
{

    public float fired = 0;


    [Tooltip("(inspector/design use only) briefly describe the (intended) behavior of this prototype")]
    [TextArea(1, 10)]
    [SerializeField] protected string Description;

    [SerializeField] protected float scoreMultiplier;
    protected enum SpeedOnExitType { Ratio, Static} //after prototyping consider having all value types be done as one universal enum

    [SerializeField] protected bool incrByNumOfBoosts;

    [SerializeField] protected int numOfBoosts;

    [SerializeField] protected SpeedOnExitType speedOnExitType;
    [SerializeField] protected float speedOnExit;

    public SPSOBase speedPrototype;


    abstract public void energyFull();
    abstract public void activate();

    abstract public void checkActivate();

    abstract public void ChargeStart();

    abstract public void ChargeContinue();

    abstract public void ChargeFinish();

    virtual public void incFired(float charge) { }

    public virtual void ResetVariables()
    {
        numOfBoosts = 0;
    }

}
