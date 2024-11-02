using System.Collections;
using System.Collections.Generic;
//C#-owy SQL do struktur danych
using System.Linq;
//Prawdopodobnie mo�na usun��
using Unity.VisualScripting;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private List<float> FlickerCooldowns; //Lista mo�liwych cooldown�w migania latarki
    [SerializeField]
    private List<float> FlickerTimes; //Lista mo�liwych czas�w trwania migni�cia latarki
    [SerializeField]
    private float FlickerCooldown; //Aktualny cooldown migania
    [SerializeField]
    private float FlickerTime; //Aktualny czas migni�cia

    private Light Light; //�r�d�o �wiat�a latarki
    private bool IsFlickering; //Czy latarka miga?


    private void InitTimes()
    {
        //Mieszanie listy cooldown�w oraz czas�w losowo i wzi�cie 0 elementu (generowanie losowe bez powt�rze�)
        FlickerCooldown = FlickerCooldowns.OrderBy(key => Random.Range(int.MinValue, int.MaxValue)).ToList()[0];  
        FlickerTime = FlickerTimes.OrderBy(key => Random.Range(int.MinValue, int.MaxValue)).ToList()[0];
    }

    //Inicjalizacja
    private void Start()
    {
        Light = GetComponent<Light>();
        IsFlickering = false;
        InitTimes();
    }

    //Miganie latarki
    private void Flicker()
    {
        if (FlickerTime > 0f)
        {
            Light.enabled = false;
            FlickerTime -= Time.deltaTime;
            return;
        }

        IsFlickering = false;
        InitTimes();
    }

    //Cooldown migania
    private void Cooldown()
    {
        if (FlickerCooldown > 0f)
        {
            Light.enabled = true;
            FlickerCooldown -= Time.deltaTime;
            return;
        }

        IsFlickering = true;
    }

    //Po��czenie w ca�o�� co klatk�
    private void Update()
    {
        switch (IsFlickering)
        {
            case true:
                Flicker();
                break;

            case false:
                Cooldown();
                break;
        }
    }
}
