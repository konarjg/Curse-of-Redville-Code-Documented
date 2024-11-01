using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Standard Trap Item", menuName = "Curse of Redville/Items/Traps/Standard")]
public class ItemTrapStandard : ItemTrap
{
    [SerializeField]
    private float ImmobilizeTime;

    public delegate void OnStandardTrapTriggered(Trap trap, float immobilizeTime);
    public static event OnStandardTrapTriggered StandardTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        StandardTrapTriggeredEvent?.Invoke(trap, ImmobilizeTime);
    }
}
