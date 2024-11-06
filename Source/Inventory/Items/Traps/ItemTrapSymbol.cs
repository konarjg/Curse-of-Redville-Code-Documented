using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Kolejna potrójna abstrakcja, ponieważ są 2 rodzaje symboli
//Symbol to pułapka, która zapewnia bonusy graczowi po wejściu w nią przez przeciwnika
//Nie zadaje obrażeń, ani nie unieruchamia
//Pure utility trap
public abstract class ItemTrapSymbol : ItemTrap
{
    [SerializeField]
    private float StatBonusPercent; //Ile procent bonusu
}
