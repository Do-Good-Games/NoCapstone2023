using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Entity, IDamageable
{
    [SerializeField] private Collider2D asteroidCollider;
    [SerializeField] private SpriteRenderer asteroidRenderer;
    [SerializeField] private EntityManager droppedEntityManager;
    [SerializeField] private int numDrops;


	[Header("Interaction")] 
	[SerializeField] private float health;
    [SerializeField] private float size;
	[SerializeField] private string laserTag;
    [SerializeField] private int score;

    [Header("Sound")]
    [SerializeField] public AudioSource destroySound;

    public override void Start()
    {
        base.Start();
        this.transform.localScale = new Vector2(size, size);
    }

    public void setVariables(float health, float size, EntityManager droppedEntityManager, int numDrops)
    {
        this.health = health;
        this.size = size;
        this.droppedEntityManager = droppedEntityManager;
        this.numDrops = numDrops;
        this.transform.localScale = new Vector2(size, size);

    }

    public bool Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            DestroyFromLazer();
            return true;
        }
        return false;
    }

    public void DestroyFromLazer()
    {
        destroySound.Play();
        gameManager.UpdateScore(score);
        //asteroidRenderer.enabled = false;
        //asteroidCollider.enabled = false;
        if (droppedEntityManager != null && numDrops > 0)
        {
            DropEntities();
        }
        //Destroy(this.gameObject, 0.5f);
        DestroyEntity();
    }

    /*override public void DestroyEntity()
    {
        base.DestroyEntity();
        //entityManager.objectPool.Release(gameObject);

        //Destroy(this.gameObject, 0.5f);
    }*/

    private void DropEntities()
    {
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 spawnPoint = (Random.insideUnitCircle * (size/2)) + (Vector2) transform.position;
            droppedEntityManager.SpawnEntity(spawnPoint.x, spawnPoint.y);
        }
    }
}
