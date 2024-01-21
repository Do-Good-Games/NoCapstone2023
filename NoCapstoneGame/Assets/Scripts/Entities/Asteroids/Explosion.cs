using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Entity
{

    [Tooltip("The damage dealt to destructable hazards on initial collision")]
    [SerializeField] protected int collisionDamage = 10;

    [SerializeField] int framesToLive;
    [SerializeField] float downwardMovement;


    // Start is called before the first frame update
    void Start()
    {
        this.upwardsSpeed = -10000;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Debug.Log("moving explosion " + this.transform.position.x + " " + this.transform.position.y + " " + this.transform.position.z);
        //transform.position = transform.position + new Vector3(0, this.transform.position.y - (downwardMovement + GameManager.Instance.relativeSpeed), 0);

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
