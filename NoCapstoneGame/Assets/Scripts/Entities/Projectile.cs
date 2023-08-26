using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("The distance from the edge of the screen to the edge of the projectile after which it will despawn")]
    [SerializeField] protected float offScreenDespawnDistance = 10;
    [Tooltip("The damage dealt to destructable hazards on initial collision")]
    [SerializeField] protected int collisionDamage = 1;
    [Tooltip("The damage done to destructable hazards while overlapped with the projectile")]
    [SerializeField] protected int continuousDamage = 0;
    [Tooltip("The length of time in seconds between each hit ")]
    [SerializeField] protected float damageInterval = 0.05f;
    [Tooltip("Whether or not the projectile should be destroyed when colliding with a hazard that it cannot destroy")]
    [SerializeField] protected bool destroyOnImpact = true;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D projectileBody;
    [SerializeField] protected SpriteRenderer projectileRenderer;
    [SerializeField] protected Collider2D projectileCollider;

    protected float speed;
    protected float startTime;

    virtual protected void Start()
    {
        startTime = Time.time;
    }


    virtual public void Launch(Vector2 vector)
    {
        transform.up = vector;
        speed = vector.magnitude;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        Move();
        if (!CheckInBoundsStatus())
        {
            Destroy(this.gameObject);
        }
    }

    virtual protected void Move()
    {
        float distance = speed * Time.deltaTime;
        projectileBody.position += (Vector2)transform.up * distance;
    }

    bool CheckInBoundsStatus()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(projectileBody.position);
        return Screen.safeArea.Contains(new Vector2(screenPos.x - offScreenDespawnDistance, screenPos.y - offScreenDespawnDistance));
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject == null)
        {
            return;
        }

        bool objectDestroyed = damageableObject.Damage(collisionDamage);
        if (!objectDestroyed && destroyOnImpact)
        {
            Destroy();
        }
    }

    virtual protected void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject == null)
        {
            return;
        }

        damageableObject.Damage(continuousDamage);
    }

    virtual protected void Destroy()
    {
        Destroy(this.gameObject);
    }
}
