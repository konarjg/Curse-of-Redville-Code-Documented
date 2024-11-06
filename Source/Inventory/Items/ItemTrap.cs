using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Podw�jna abstrakcja:
//Specyficzny typ pu�apki np. symbol, standardow�, okultystyczn� itp.
//traktujemy nadal jako po prostu pu�apk�
//pu�apk� traktujemy wci�� jako po prostu przedmiot
//pozwala nam to na abstrakcj� metody Use oraz Trigger
public abstract class ItemTrap : Item
{
    [SerializeField]
    private GameObject TrapPrefab; //Odno�nik do modelu pu�apki (do stawiania pu�apek)

    //Zdarzenie wywo�ane przy rozpocz�ciu rozstawiania pu�apki
    public delegate void OnStartPlacingTrap(GameObject trapPrefab);
    public static event OnStartPlacingTrap StartPlacingTrapEvent;

    public override bool Use()
    {
        //Je�li pu�apka nie mo�e by� rozstawiona anuluj u�ycie przedmiotu
        if (!TrapPlacer.Instance.CanPlaceTrap())
        {
            return false;
        }

        //Wywo�aj zdarzenie
        StartPlacingTrapEvent?.Invoke(TrapPrefab);
        return true;
    }

    //Metoda opisuj�ca co stanie si� po tym jak przeciwnik wejdzie w t� pu�apk�
    public abstract void Trigger(Trap trap);
}
