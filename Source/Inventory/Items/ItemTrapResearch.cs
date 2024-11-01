using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Occultist Guide Page", menuName = "Curse of Redville/Items/Occultist Guide")]
public class ItemTrapResearch : ItemNote
{
    [SerializeField]
    private TrapCraftingRecipe RecipeUnlocked;

    public delegate void OnTrapDiscovered(TrapCraftingRecipe recipe);
    public static event OnTrapDiscovered TrapDiscoveredEvent;

    public override bool Use() 
    {
        base.Use();
        TrapDiscoveredEvent?.Invoke(RecipeUnlocked);
        return true;
    }
}
