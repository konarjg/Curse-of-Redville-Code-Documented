using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Podwójna abstrakcja:
//Specyficzny typ pu³apki np. symbol, standardow¹, okultystyczn¹ itp.
//traktujemy nadal jako po prostu pu³apkê
//pu³apkê traktujemy wci¹¿ jako po prostu przedmiot
//pozwala nam to na abstrakcjê metody Use oraz Trigger
public abstract class ItemTrap : Item
{
    [SerializeField]
    private GameObject TrapPrefab; //Odnoœnik do modelu pu³apki (do stawiania pu³apek)

    //Zdarzenie wywo³ane przy rozpoczêciu rozstawiania pu³apki
    public delegate void OnStartPlacingTrap(GameObject trapPrefab);
    public static event OnStartPlacingTrap StartPlacingTrapEvent;

    public override bool Use()
    {
        //Jeœli pu³apka nie mo¿e byæ rozstawiona anuluj u¿ycie przedmiotu
        if (!TrapPlacer.Instance.CanPlaceTrap())
        {
            return false;
        }

        //Wywo³aj zdarzenie
        StartPlacingTrapEvent?.Invoke(TrapPrefab);
        return true;
    }

    //Metoda opisuj¹ca co stanie siê po tym jak przeciwnik wejdzie w t¹ pu³apkê
    public abstract void Trigger(Trap trap);
}
