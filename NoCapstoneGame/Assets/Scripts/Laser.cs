using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Rigidbody2D laserBody;

    [Tooltip("The distance the projectile travels each second")]
    [SerializeField] public float speed = 10;
    [Tooltip("The distance from the edge of the screen to the TIP of the laser after which the laser will despawn")]
    [SerializeField] public float offScreenDespawnDistance = 10;
    [Tooltip("The damage dealt to destructable hazards")]
    [SerializeField] public int damage = 1;

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
        laserBody.position = new Vector2(laserBody.position.x, laserBody.position.y + deltaPos);
    }

    bool CheckInBoundsStatus()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(laserBody.position);
        return Screen.safeArea.Contains(new Vector2(screenPos.x, screenPos.y - offScreenDespawnDistance));
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
