using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Guide Page", menuName = "Curse of Redville/Items/Occultist Guide")]
//Receptura do wytwarzania pu³apek, dziedziczy po notatce, poniewa¿ zawieraæ te¿ bêdzie tekst
public class ItemTrapResearch : ItemNote
{
    [SerializeField]
    private TrapCraftingRecipe RecipeUnlocked; //Odnoœnik do receptury

    //Zdarzenie po odkryciu tej pu³apki
    public delegate void OnTrapDiscovered(TrapCraftingRecipe recipe);
    public static event OnTrapDiscovered TrapDiscoveredEvent;

    public override bool Use() 
    {
        //Z klasy nadrzêdnej wywo³ujemy Use, by dodaæ t¹ notatkê do dziennika
        base.Use();
        //Nastêpnie odkrywamy t¹ recepturê
        TrapDiscoveredEvent?.Invoke(RecipeUnlocked);
        return true;
    }
}
