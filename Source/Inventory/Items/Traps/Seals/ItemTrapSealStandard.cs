using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Standard Seal Trap Item", menuName = "Curse of Redville/Items/Traps/Seals/Standard")]
//Standardowa pieczęć, unieruchamia przeciwnika kosztem psychiki
public class ItemTrapSealStandard : ItemTrapSeal
{
    //Raczej rozumiesz
    public delegate void OnStandardSealTrapTriggered(Trap trap);
    public static event OnStandardSealTrapTriggered StandardSealTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        StandardSealTrapTriggeredEvent?.Invoke(trap);
    }
}
