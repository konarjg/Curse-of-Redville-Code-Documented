using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Potrójna abstrakcja
//Są dwa rodzaje pieczęci, każda z nich działa trochę inaczej
//Obie konwertują psychikę w jakiś efekt
//Traktujemy je jako po prostu pieczęć, która po prostu jest pułapką, która
//po prostu jest przedmiotem
public abstract class ItemTrapSeal : ItemTrap
{
    [SerializeField]
    private float ConversionRate; //Ile procent psychiki konwertować na efekt
}
