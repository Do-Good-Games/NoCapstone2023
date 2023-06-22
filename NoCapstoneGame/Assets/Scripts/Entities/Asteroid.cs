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
    [SerializeField] private GameObject droppedEntityPrefab;
    [SerializeField] private int numDrops;


	[Header("Interaction")] 
	[SerializeField] public float health;
    [SerializeField] public float size;
	[SerializeField] public string laserTag;
    [SerializeField] public int score;

    [Header("Sound")]
    [SerializeField] public AudioSource destroySound;

    public override void Start()
    {
        base.Start();
        this.transform.localScale = new Vector2(size, size);
    }

    public void setVariables(float health, float size)
    {
        this.health = health;
        this.size = size;
    }

    public bool Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy();
            return true;
        }
        return false;
    }

    override public void Destroy()
    {
        destroySound.Play();
        gameManager.UpdateScore(score);
        asteroidRenderer.enabled = false;
        asteroidCollider.enabled = false;
        Destroy(this.gameObject, 0.5f);
    }
}
