using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Entity, IDamageable
{
    [SerializeField] private Collider2D m_collider;
    [SerializeField] public SpriteRenderer m_spriteRenderer;
    [SerializeField] public EntityManager droppedEntityManager;
    [SerializeField] protected int numDrops;


    [Header("Interaction")]
    [SerializeField] private float health;
    [SerializeField] private float size;
    [SerializeField] private string laserTag;
    [SerializeField] private int score;

    [Header("Sound")]

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioReference destroySoundArray;


    private IEnumerator DestroyAsteroidCoroutine;

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
            DestroyAsteroid();
            return true;
        }
        return false;
    }

    private IEnumerator PlaySoundThenDestroy()
    {
        m_spriteRenderer.enabled = false;
        m_collider.enabled = false;

        audioSource.clip = destroySoundArray.GetRandomClip();

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        m_spriteRenderer.enabled = true;
        m_collider.enabled = true;

        base.DestroyEntity();
    }

    //could theoretically also do this as an override of DestroyEntity(), though that would involve an extra bool to differentiate the case of the asteroid going offscreen, as we don't want to play sound or drop
    //energy when it does that. this still felt cleaner
    virtual public void DestroyAsteroid()
    { 
        gameManager.UpdateScore(score);
        if (droppedEntityManager != null && numDrops > 0)
        {
            // keeping this for reference later
            //if(isSplitter)
            //{
            //    //GameObject entity = EntityManager.instance.GenerateAmountOnPoint(4, this.transform.position); //THIS SHOULD WORK, I JUST NEED TO CREATE AN INSTANCE FOR ENTITY MANAGER
            //    //alternatively I could store some small asteroids and spawn them in here.

            //    //MAKE METHOD FOR SPLITTER, REFERENCE ENTITYMANAGER.GENERATE ENTITITES
            //    //i might have to make a coroutine called generateAmountOnPoint(int amountOfAsteroids, Vector3 position
            //    //Projectile laser = GameObject.Instantiate<Projectile>(laserPrefab, this.transform.position, this.transform.rotation);
            //    //GameObject smallerAsteroid = GameObject.Instantiate(smallAsteroidPrefab, this.transform.position, this.transform.rotation); //THIS IS HOW THE EXPLOSION SHOULD WORK, BUT WE SHOULD PULL FROM OBJECT POOL
            //}


            DropEntities();

        }
        //Destroy(this.gameObject, 0.5f);

        DestroyAsteroidCoroutine = PlaySoundThenDestroy();
        StartCoroutine(DestroyAsteroidCoroutine);
    }

    override public void DestroyEntity()
    {
        if (DestroyAsteroidCoroutine == null)
        {//if the object is already being destroyed then we don't want it to interupt that process (edge case is when in boost, player destroys asteroid, and it goes OOB before the sound fully plays, cutting the sound off
            base.DestroyEntity();
        }

        //entityManager.objectPool.Release(gameObject);

        //Destroy(this.gameObject, 0.5f);
    }

    virtual protected void DropEntities()
    {
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 spawnPoint = (Random.insideUnitCircle * (size/2)) + (Vector2) transform.position;
            droppedEntityManager.SpawnEntity(spawnPoint.x, spawnPoint.y);
        }
    }
}
