using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lokalizacja menu tworzenia tego przedmiotu w edytorze Unity
[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Curse of Redville/Items/Consumable")]
//Dziedziczymy po abstrakcyjnej klasie Item
//Ten typ przedmiotu opisuje tabletki z litem lub inne przedmioty zwi�kszaj�ce psychik�
public class ItemConsumable : Item
{
    [SerializeField]
    private float SanityIncrease; //Warto�� zwi�kszenia psychiki

    //Zdarzenie wywo�ane po u�yciu tego przedmiotu
    public delegate void OnSanityIncreased(float value);
    public event OnSanityIncreased SanityIncreasedEvent;

    //Specyfikacja metody Use z klasy nadrz�dnej Item
    //S�owo override oznacza, �e przeci��amy metod� z klasy nadrz�dnej
    public override bool Use()
    {
        //Wywo�ujemy zdarzenie
        SanityIncreasedEvent?.Invoke(SanityIncrease);
        return true;
    }
}
