using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPSOBase : ScriptableObject
{
    protected GameManager gameManager;
    protected PlayerController playerController;

    [Tooltip("(inspector/design use only) briefly describe the (intended) behavior of this prototype")]
    [TextArea(1, 10)]
    [SerializeField] protected string Description;

    [SerializeField] public float speed;

    [SerializeField] protected bool BByEnergyHeld;
    [SerializeField] protected float PerEnergyHeld;

    [SerializeField] protected bool BByEnergyCollected;
    [SerializeField] protected float PerEnergyCollected;

    [SerializeField] protected bool BByEnergyFired;
    [SerializeField] protected float PerEnergyFired;

    [SerializeField] protected bool BByTime;
    [SerializeField] protected float timeScale;

    protected enum HitLossType { Disabled, Static, Ratio };//after prototyping consider having all value types be done as one universal enum
    [SerializeField] protected HitLossType hitLossType;
    [SerializeField] protected float AmountLostOnHit;

    protected float prevEnergyLevel;

    

    protected virtual void OnEnable()
    {
    //    Debug.Log("sO is enabled");
    //    //gameManager = ;

    //    GameManager.Instance.OnEnergyChange.AddListener(SPEnergyCollected);
    //    gameManager.OnEnergyChange.AddListener(SPEnergyHeld);
    }
    abstract public void SPEnergyHeld();
    virtual public void SPEnergyHeld(bool increment) { SPEnergyHeld(); }
    abstract public void SPEnergyCollected();
    abstract public void SPEnergyFired(float charge);
    abstract public void SPOverTime();
    abstract public void SPHit();
    abstract public void ResetVariables();

}
