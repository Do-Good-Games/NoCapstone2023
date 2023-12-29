using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    [Tooltip("The damage dealt to destructable hazards on initial collision")]
    [SerializeField] protected int collisionDamage = 10;

    [SerializeField] int framesToLive;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        framesToLive -= 1;
        if(framesToLive <= 0)
        {
            Destroy();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) //this was made referencing projectile's onTriggerEnter2D
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject == null)
        {
            return;
        }

        bool objectDestroyed = damageableObject.Damage(collisionDamage);
        if (!objectDestroyed)
        {
            Destroy();
        }
    }

    virtual protected void Destroy()
    {
        Destroy(this.gameObject);
    }
}
