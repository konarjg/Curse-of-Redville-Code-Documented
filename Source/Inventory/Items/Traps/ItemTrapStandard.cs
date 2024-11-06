using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Standard Trap Item", menuName = "Curse of Redville/Items/Traps/Standard")]
//Pułapka standardowa, tylko unieruchamia
public class ItemTrapStandard : ItemTrap
{
    [SerializeField]
    private float ImmobilizeTime; //Czas unieruchomienia

    //Zdarzenie po wejściu w ten typ pułapki
    public delegate void OnStandardTrapTriggered(Trap trap, float immobilizeTime);
    public static event OnStandardTrapTriggered StandardTrapTriggeredEvent;

    //To już raczej rozumiesz
    public override void Trigger(Trap trap)
    {
        StandardTrapTriggeredEvent?.Invoke(trap, ImmobilizeTime);
    }
}
