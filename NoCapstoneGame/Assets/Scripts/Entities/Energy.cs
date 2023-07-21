using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : Entity
{
    [SerializeField] private Collider2D energyCollider;
    [SerializeField] private SpriteRenderer energyRenderer;

    [SerializeField] private float energyGain;

    bool inMagnet;

    public override void Start()
    {
        base.Start();
        inMagnet = false;
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
        gameManager.UpdateEnergy(energyGain);
        Destroy(this.gameObject);
    }
}
