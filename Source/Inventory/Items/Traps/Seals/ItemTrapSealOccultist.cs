using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Seal Trap Item", menuName = "Curse of Redville/Items/Traps/Seals/Occultist Seal")]
public class ItemTrapSealOccultist : ItemTrapSeal
{
    public delegate void OnOccultistSealTrapTriggered(Trap trap);
    public static event OnOccultistSealTrapTriggered OccultistSealTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        OccultistSealTrapTriggeredEvent?.Invoke(trap);
    }
}
