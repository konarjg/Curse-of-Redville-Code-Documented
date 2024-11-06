using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol of Sanity Trap Item", menuName = "Curse of Redville/Items/Traps/Symbols/Symbol of Sanity")]
//Symbol zapewniający bonus procentowy do psychiki (tymczasowo)
public class ItemTrapSymbolSanity : ItemTrapSymbol
{
    public delegate void OnSanitySymbolTrapTriggered(Trap trap);
    public static event OnSanitySymbolTrapTriggered SanitySymbolTrapTriggeredEvent;

    public override void Trigger(Trap trap)
    {
        SanitySymbolTrapTriggeredEvent?.Invoke(trap);
    }
}