using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D projectileBody;

    [Tooltip("The distance the projectile travels each second")]
    [SerializeField] public float speed = 10;
    [Tooltip("The distance from the edge of the screen to the edge of the projectile after which it will despawn")]
    [SerializeField] public float offScreenDespawnDistance = 10;
    [Tooltip("The damage dealt to destructable hazards")]
    [SerializeField] public int damage = 1;



    void Spawn()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (!CheckInBoundsStatus())
        {
            Destroy(this.gameObject);
        }
    }

    void Move()
    {
        float deltaPos = speed * Time.deltaTime;
        projectileBody.position = new Vector2(projectileBody.position.x, projectileBody.position.y + deltaPos);
    }

    bool CheckInBoundsStatus()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(projectileBody.position);
        return Screen.safeArea.Contains(new Vector2(screenPos.x - offScreenDespawnDistance, screenPos.y - offScreenDespawnDistance));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject == null)
        {
            return;
        }

        bool objectDestroyed = damageableObject.Damage(damage);
        if (!objectDestroyed)
        {
            Destroy(this.gameObject);
        }
    }
}
