using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Guide Page", menuName = "Curse of Redville/Items/Occultist Guide")]
//Receptura do wytwarzania pu�apek, dziedziczy po notatce, poniewa� zawiera� te� b�dzie tekst
public class ItemTrapResearch : ItemNote
{
    [SerializeField]
    private TrapCraftingRecipe RecipeUnlocked; //Odno�nik do receptury

    //Zdarzenie po odkryciu tej pu�apki
    public delegate void OnTrapDiscovered(TrapCraftingRecipe recipe);
    public static event OnTrapDiscovered TrapDiscoveredEvent;

    public override bool Use() 
    {
        //Z klasy nadrz�dnej wywo�ujemy Use, by doda� t� notatk� do dziennika
        base.Use();
        //Nast�pnie odkrywamy t� receptur�
        TrapDiscoveredEvent?.Invoke(RecipeUnlocked);
        return true;
    }
}
