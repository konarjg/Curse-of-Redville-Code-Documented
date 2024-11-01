using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : WorldItem
{
    [SerializeField]
    private ItemTrap TrapItem;
    [SerializeField]
    private float GravityStrength;

    private void Awake()
    {
        
    }

    public void Activate()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            TrapItem.Trigger(this);
            Destroy(gameObject);
        }
    }


    private void FixedUpdate()
    {
        var physics = GetComponent<CharacterController>();

        if (!physics.isGrounded)
        {
            physics.Move(Physics.gravity.normalized * GravityStrength * Time.fixedDeltaTime);
        }
    }
}
