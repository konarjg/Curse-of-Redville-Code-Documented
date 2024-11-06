using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Trap Item", menuName = "Curse of Redville/Items/Traps/Occultist")]
//Pułapka okultystyczna
//Unieruchamia i zadaje obrażenia
public class ItemTrapOccultist : ItemTrap
{
    [SerializeField]
    private float FixationDamage; //Wartość obrażeń
    [SerializeField]
    private float ImmobilizeTime; //Czas unieruchomienia
    
    //Zdarzenie po wejściu w tą pułapkę
    public delegate void OnOccultistTrapTriggered(Trap trap, float damage, float immobilizeTime);
    public static event OnOccultistTrapTriggered OccultistTrapTriggeredEvent;

    //Specyfikacja metody Trigger z klasy nadrzędnej ItemTrap
    public override void Trigger(Trap trap)
    {
        OccultistTrapTriggeredEvent?.Invoke(trap, FixationDamage, ImmobilizeTime);
    }
}
