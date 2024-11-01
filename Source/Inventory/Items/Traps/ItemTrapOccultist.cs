using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Trap Item", menuName = "Curse of Redville/Items/Traps/Occultist")]
public class ItemTrapOccultist : ItemTrap
{
    [SerializeField]
    private float FixationDamage;
    [SerializeField]
    private float ImmobilizeTime;

    public delegate void OnOccultistTrapTriggered(Trap trap, float damage, float immobilizeTime);
    public static event OnOccultistTrapTriggered OccultistTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        OccultistTrapTriggeredEvent?.Invoke(trap, FixationDamage, ImmobilizeTime);
    }
}
