using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPSOBase : ScriptableObject
{
    protected GameManager gameManager;
    protected PlayerController playerController;

    [SerializeField] protected bool BByEnergyHeld;
    [SerializeField] protected float PerEnergyHeld;

    [SerializeField] protected bool BByEnergyCollected;
    [SerializeField] protected float PerEnergyCollected;

    [SerializeField] protected bool BByEnergyFired;
    [SerializeField] protected float PerEnergyFired;

    [SerializeField] protected bool BByTime;
    [SerializeField] protected float PerTime;

    protected enum HitLossType { Disabled, Static, Ratio } ;
    [SerializeField] protected HitLossType hitLossType;
    [SerializeField] protected float AmountLostOnHitStatic;

    protected float prevEnergyLevel;

    private void Start()
    {
        
    }

    protected void OnEnable()
    {
        Debug.Log("sO is enabled");
        //gameManager = ;

        GameManager.Instance.OnEnergyChange.AddListener(SPEnergyCollected);
        gameManager.OnEnergyChange.AddListener(SPEnergyHeld);
    }

    abstract public void SPEnergyHeld();
    abstract public void SPEnergyCollected();
    abstract public void SPEnergyFired(float charge);
    abstract public void SPOverTime();
    abstract public void SPHit();


}
