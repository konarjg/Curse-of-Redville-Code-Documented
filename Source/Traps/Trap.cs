using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Klasa obs³uguj¹ca rozstawion¹ pu³apkê, dziedziczy po WorldItem, poniewa¿ mo¿na j¹ podnieœæ tak jak przedmiot
public class Trap : WorldItem
{
    [SerializeField]
    private ItemTrap TrapItem; //Przedmiot reprezentuj¹cy t¹ pu³apkê w ekwipunku
    [SerializeField]
    private float GravityStrength; //Przyspieszenie grawitacyjne

    private void Awake()
    {
        
    }

    //Aktywowanie pu³apki
    public void Activate()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    //Jeœli przeciwnik wejdzie w t¹ pu³apkê uruchomi siê ta metoda
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            //Wywo³ujemy obs³ugê wejœcia w t¹ pu³apkê
            TrapItem.Trigger(this);
            //usuwamy pu³apkê ze œwiata gry
            Destroy(gameObject);
        }
    }


    //Aktualizujemy fizykê
    private void FixedUpdate()
    {
        var physics = GetComponent<CharacterController>();

        //jeœli pu³apka nie jest na ziemi to nak³adamy grawitacjê
        if (!physics.isGrounded)
        {
            physics.Move(Physics.gravity.normalized * GravityStrength * Time.fixedDeltaTime);
        }
    }
}
