using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Klasa obs�uguj�ca rozstawion� pu�apk�, dziedziczy po WorldItem, poniewa� mo�na j� podnie�� tak jak przedmiot
public class Trap : WorldItem
{
    [SerializeField]
    private ItemTrap TrapItem; //Przedmiot reprezentuj�cy t� pu�apk� w ekwipunku
    [SerializeField]
    private float GravityStrength; //Przyspieszenie grawitacyjne

    private void Awake()
    {
        
    }

    //Aktywowanie pu�apki
    public void Activate()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    //Je�li przeciwnik wejdzie w t� pu�apk� uruchomi si� ta metoda
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !other.isTrigger)
        {
            //Wywo�ujemy obs�ug� wej�cia w t� pu�apk�
            TrapItem.Trigger(this);
            //usuwamy pu�apk� ze �wiata gry
            Destroy(gameObject);
        }
    }


    //Aktualizujemy fizyk�
    private void FixedUpdate()
    {
        var physics = GetComponent<CharacterController>();

        //je�li pu�apka nie jest na ziemi to nak�adamy grawitacj�
        if (!physics.isGrounded)
        {
            physics.Move(Physics.gravity.normalized * GravityStrength * Time.fixedDeltaTime);
        }
    }
}
