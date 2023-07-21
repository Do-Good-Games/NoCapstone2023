using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public Collider2D shipCollider;
    public CircleCollider2D energySphereCollider;

    private GameManager gameManager;

    public void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // If the colliding GameObject is tagged as a hazard, take damage
        if (collision.gameObject.CompareTag(gameManager.hazardTag))
        {
            playerController.Hit();
        }
    }

    public void UpdateEnergySphereCollider(float size, float grace)
    {
        energySphereCollider.radius = (size / 2) - grace;
    }
   
}
