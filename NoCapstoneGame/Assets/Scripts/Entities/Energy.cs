using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : Entity
{
    [SerializeField] private Collider2D energyCollider;
    [SerializeField] private SpriteRenderer energyRenderer;

    [SerializeField] private float energyGain;

    [Tooltip("for diminishing returns while in boost - to scale the rate at which the player's current energy level translates to the amount collected")]
    [SerializeField] private float diminishingReturnRatio;

    bool inMagnet;

    private SpeedManager speedManager;

    public override void Start()
    {
        base.Start();
        inMagnet = false;

        speedManager = GameManager.Instance.speedManager;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(gameManager.playerTag))
        {
            Collect();
            
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        EnergyMagnetArea magnet = collision.GetComponent<EnergyMagnetArea>();
        if (magnet == null)
        {
            return;
        }

        inMagnet = true;


        float magnetForce = magnet.getStrength();

        Vector2 magnetCenter = magnet.gameObject.transform.position;
        Vector2 currentPos = this.gameObject.transform.position;
        Vector2 magnetVector = magnetCenter - currentPos;

        Vector2 newPos = (currentPos + magnetVector * magnetForce);

        entityBody.MovePosition(newPos);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        inMagnet = false;
    }

    public override void Move()
    {
        if (!inMagnet)
        {
            base.Move();
        }
    }

    private void Collect()
    {

        if (speedManager.inBoost) //while the player is boosting, add diminishing returns to their energy collection to prevent them from staying in boost forever
        {
            float remainingRatio = (gameManager.GetCharge() / gameManager.GetMaxEnergy());
            gameManager.UpdateEnergy(energyGain * diminishingReturnRatio);
        } else if (!speedManager.inBoostGracePeriod)//don't let the player collect energy right after boost, this is to ensure the player doesn't have spare energy after boost
        { //otherwise, the player will occasionally destroy an asteroid, then exit boost before collecting energy. at which point they start with energy after boost
            gameManager.UpdateEnergy(energyGain);
        }

        Destroy(this.gameObject);
    }
}
