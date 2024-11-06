using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol of Speed Trap Item", menuName = "Curse of Redville/Items/Traps/Symbols/Symbol of Speed")]
//Symbol zapewniający bonus do szybkości (tymczasowo)
public class ItemTrapSymbolSpeed : ItemTrapSymbol
{
    public delegate void OnSpeedSymbolTrapTriggered(Trap trap);
    public static event OnSpeedSymbolTrapTriggered SpeedSymbolTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        SpeedSymbolTrapTriggeredEvent?.Invoke(trap);
    }
}