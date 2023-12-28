using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Entity, IDamageable
{
    [SerializeField] public SpriteRenderer m_spriteRenderer;
    [SerializeField] public EntityManager droppedEntityManager;
    [SerializeField] private int numDrops;
    [SerializeField] private int numDropsGoldAsteroid;


    [Header("Interaction")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float size;
    [SerializeField] private string laserTag;
    [SerializeField] private int score;
    [SerializeField] private bool isExplosive = false;
    [SerializeField] private bool isGold = false;
    [SerializeField] private bool isSplitter = false;
    [SerializeField] private bool asteroidVersions = true;
    [SerializeField] private float goldenAsteroidEnergyRatio;

    [Header("Sound")]

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioReference destroySoundArray;


    private IEnumerator DestroyAsteroidCoroutine;

    public override void Start()
    {

        base.Start();
        this.transform.localScale = new Vector2(size, size);
        ChangeAsteroidType();
    }

    public void setVariables(float health, float size, EntityManager droppedEntityManager, int numDrops)
    {
        this.maxHealth = health;
        this.health = health;
        this.size = size;
        this.droppedEntityManager = droppedEntityManager;
        this.numDrops = numDrops;
        this.numDropsGoldAsteroid = Mathf.FloorToInt(numDrops * goldenAsteroidEnergyRatio);
        this.transform.localScale = new Vector2(size, size);

        ChangeAsteroidType();


    }

    public bool Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            DestroyAsteroid();
            return true;
        }

        //change the asteroid sprite
        if(health <= (maxHealth * 1 / 3))
        {
            Debug.Log("switch to 2nd damage sprite");
        }
        else if (health <= (maxHealth * 2 / 3))
        {
            Debug.Log("switch to 1st damage sprite");
        }
        else
        {
            //this needs to happen when the sprite is destroyed as well
            Debug.Log("stay in undamaged sprite");
        }

        return false;
    }

    private IEnumerator PlaySoundThenDestroy()
    {
        m_spriteRenderer.enabled = false;
        entityCollider.enabled = false;

        audioSource.clip = destroySoundArray.GetRandomClip();

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        m_spriteRenderer.enabled = true;
        entityCollider.enabled = true;

        base.DestroyEntity();
    }

    //could theoretically also do this as an override of DestroyEntity(), though that would involve an extra bool to differentiate the case of the asteroid going offscreen, as we don't want to play sound or drop
    //energy when it does that. this still felt cleaner
    public void DestroyAsteroid()
    { 

        gameManager.UpdateScore(score);
        if (droppedEntityManager != null && numDrops > 0)
        {
            if(isGold)
            {
                DropEntitiesGoldAsteroid();
            }
            else if(isExplosive)
            {
                //take current position and place an explosion object here
                GameObject explosion = GameObject.Instantiate(GameManager.Instance.explosionPrefab, this.transform.position, this.transform.rotation);
            }

            else if(isSplitter)
            {
                //GameObject entity = EntityManager.instance.GenerateAmountOnPoint(4, this.transform.position); //THIS SHOULD WORK, I JUST NEED TO CREATE AN INSTANCE FOR ENTITY MANAGER
                //alternatively I could store some small asteroids and spawn them in here.

                //MAKE METHOD FOR SPLITTER, REFERENCE ENTITYMANAGER.GENERATE ENTITITES
                //i might have to make a coroutine called generateAmountOnPoint(int amountOfAsteroids, Vector3 position
                //Projectile laser = GameObject.Instantiate<Projectile>(laserPrefab, this.transform.position, this.transform.rotation);
                //GameObject smallerAsteroid = GameObject.Instantiate(smallAsteroidPrefab, this.transform.position, this.transform.rotation); //THIS IS HOW THE EXPLOSION SHOULD WORK, BUT WE SHOULD PULL FROM OBJECT POOL
            }
            else
            {
                DropEntities();
            }

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

    private void DropEntities()
    {
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 spawnPoint = (Random.insideUnitCircle * (size/2)) + (Vector2) transform.position;
            droppedEntityManager.SpawnEntity(spawnPoint.x, spawnPoint.y);
        }
    }
    private void DropEntitiesGoldAsteroid()
    {
        for (int i = 0; i < numDropsGoldAsteroid; i++)
        {
            Vector2 spawnPoint = (Random.insideUnitCircle * (size / 2)) + (Vector2)transform.position;
            droppedEntityManager.SpawnEntity(spawnPoint.x, spawnPoint.y);
        }
    }

    public void ChangeAsteroidType()
    {
        if (asteroidVersions == true)
        {
            int RandomRoll = Random.Range(0, 100);
            if (RandomRoll > 90)
            {
                isExplosive = true;
                //isGold = true;
                m_spriteRenderer.color = UnityEngine.Color.red; //CHANGE THE SPRITE INSTEAD OF THE COLOR
                //AsteroidSpriteRenderer = GetComponent<SpriteRenderer>();
                //AsteroidSpriteRenderer.color = Color.Red;
            }
            else if (RandomRoll > 80)
            {
                isGold = true;
                //asteroidRenderer.color = new UnityEngine.Color(236f, 223f, 72f, 255f);
                m_spriteRenderer.color = UnityEngine.Color.yellow;
            }
        }

    }
}
