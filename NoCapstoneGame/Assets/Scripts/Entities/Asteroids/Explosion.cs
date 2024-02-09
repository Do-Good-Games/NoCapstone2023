using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : Entity
{

    [Tooltip("The damage dealt to destructable hazards on initial collision")]
    [SerializeField] protected int collisionDamage = 10;

    [SerializeField] int framesToLive;
    [SerializeField] float downwardMovement;
    [SerializeField] Animator animator;
    public int index;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Debug.Log("check1 spawning explosion");

        this.upwardsSpeed = downwardMovement;

        animator.SetInteger("Index", index);
    }

    public override void DestroyEntity()
    {
        Destroy();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
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
