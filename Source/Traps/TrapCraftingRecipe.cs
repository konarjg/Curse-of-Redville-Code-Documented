using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Crafting Recipe", menuName = "Curse of Redville/Crafting/Trap Recipe")]
public class TrapCraftingRecipe : ScriptableObject
{
    [SerializeField]
    private ItemTrap CraftingResult;
    [SerializeField]
    private List<ItemStack> RequiredIngredients;
    [SerializeField]
    private int Tier;

    public ItemTrap GetCraftingResult()
    {
        return CraftingResult;
    }

    public List<ItemStack> GetIngredientList()
    {
        return RequiredIngredients;
    }

    public int GetTier()
    {
        return Tier;
    }

    public Type GetRecipeType()
    {
        return CraftingResult.GetType();
    }
}
