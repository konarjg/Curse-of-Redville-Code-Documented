using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lokalizacja menu tworzenia tego przedmiotu w edytorze Unity
[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Curse of Redville/Items/Consumable")]
//Dziedziczymy po abstrakcyjnej klasie Item
//Ten typ przedmiotu opisuje tabletki z litem lub inne przedmioty zwiêkszaj¹ce psychikê
public class ItemConsumable : Item
{
    [SerializeField]
    private float SanityIncrease; //Wartoœæ zwiêkszenia psychiki

    //Zdarzenie wywo³ane po u¿yciu tego przedmiotu
    public delegate void OnSanityIncreased(float value);
    public event OnSanityIncreased SanityIncreasedEvent;

    //Specyfikacja metody Use z klasy nadrzêdnej Item
    //S³owo override oznacza, ¿e przeci¹¿amy metodê z klasy nadrzêdnej
    public override bool Use()
    {
        //Wywo³ujemy zdarzenie
        SanityIncreasedEvent?.Invoke(SanityIncrease);
        return true;
    }
}
