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
    [SerializeField] private int numDropsGoldAsteroid;


    [Header("Interaction")] 
	[SerializeField] private float health;
    [SerializeField] private float size;
	[SerializeField] private string laserTag;
    [SerializeField] private int score;
    [SerializeField] private bool isExplosive = false;
    [SerializeField] private bool isGold = false;
    [SerializeField] private bool isSplitter = false;
    [SerializeField] private bool asteroidVersions = true;


    [Header("Sound")]
    [SerializeField] public AudioSource destroySound;

    public override void Start()
    {

        base.Start();
        this.transform.localScale = new Vector2(size, size);

        if(asteroidVersions == true)
        {
            int RandomRoll = Random.Range(0, 100);
            if (RandomRoll > 90)
            {
                Debug.Log("spawn explosive");
                isExplosive = true;
                //isGold = true;
                asteroidRenderer.color = UnityEngine.Color.red ; //CHANGE THE SPRITE INSTEAD OF THE COLOR
                //AsteroidSpriteRenderer = GetComponent<SpriteRenderer>();
                //AsteroidSpriteRenderer.color = Color.Red;
            }
            else if(RandomRoll > 80)
            {
                Debug.Log("spawn gold");
                isGold = true;
                //asteroidRenderer.color = new UnityEngine.Color(236f, 223f, 72f, 255f);
                asteroidRenderer.color = UnityEngine.Color.yellow;
            }
        }

    }

    public void setVariables(float health, float size, EntityManager droppedEntityManager, int numDrops)
    {
        this.health = health;
        this.size = size;
        this.droppedEntityManager = droppedEntityManager;
        this.numDrops = numDrops;
        this.numDropsGoldAsteroid = numDrops*5;
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
        //Debug.Log("destroyed from lazer");
        destroySound.Play();
        gameManager.UpdateScore(score);
        //asteroidRenderer.enabled = false;
        //asteroidCollider.enabled = false;
        if (droppedEntityManager != null && numDrops > 0)
        {
            if(isGold)
            {
                Debug.Log("dropping gold!");
                DropEntitiesGoldAsteroid();
            }
            else if(isExplosive)
            {
                Debug.Log("exploding");
                //take current position and place an explosion object here
                GameObject explosion = GameObject.Instantiate(GameManager.Instance.explosionPrefab, this.transform.position, this.transform.rotation);
            }


            else if(isSplitter)
            {
                Debug.Log("splitting");
                //GameObject entity = EntityManager.instance.GenerateAmountOnPoint(4, this.transform.position); //THIS SHOULD WORK, I JUST NEED TO CREATE AN INSTANCE FOR ENTITY MANAGER
                //alternatively I could store some small asteroids and spawn them in here.

                //MAKE METHOD FOR SPLITTER, REFERENCE ENTITYMANAGER.GENERATE ENTITITES
                //i might have to make a coroutine called generateAmountOnPoint(int amountOfAsteroids, Vector3 position
                //Projectile laser = GameObject.Instantiate<Projectile>(laserPrefab, this.transform.position, this.transform.rotation);
                //GameObject smallerAsteroid = GameObject.Instantiate(smallAsteroidPrefab, this.transform.position, this.transform.rotation); //THIS IS HOW THE EXPLOSION SHOULD WORK, BUT WE SHOULD PULL FROM OBJECT POOL
            }
            else
            {
                Debug.Log("dropping normally");
                DropEntities();
            }

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
    private void DropEntitiesGoldAsteroid()
    {
        Debug.Log("am i alive?");
        for (int i = 0; i < numDropsGoldAsteroid; i++)
        {
            Debug.Log("dropped");
            Vector2 spawnPoint = (Random.insideUnitCircle * (size / 2)) + (Vector2)transform.position;
            droppedEntityManager.SpawnEntity(spawnPoint.x, spawnPoint.y);
        }
    }
}
