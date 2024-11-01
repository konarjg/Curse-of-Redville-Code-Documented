using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TrapCrafting : MonoBehaviour
{
    [SerializeField]
    private GameObject TrapRecipeUIPrefab;
    [SerializeField]
    private Transform OccultistTrapsUI;
    [SerializeField]
    private Transform StandardTrapsUI;
    [SerializeField]
    private Transform SymbolsTrapsUI;
    [SerializeField]
    private Transform SealsTrapsUI;

    [Space()]
    [SerializeField]
    private GameObject DescriptionUI;
    [SerializeField]
    private TextMeshProUGUI DescriptionText;
    [SerializeField]
    private TextMeshProUGUI DescriptionNameText;
    [SerializeField]
    private Button CraftButton;

    private int CurrentRecipeId = -1;
    private List<TrapCraftingRecipe> Recipes = new List<TrapCraftingRecipe>();
    public static TrapCrafting Instance;

    public delegate void OnToggleRecipeClick();
    public static event OnToggleRecipeClick ToggleRecipeClickEvent;

    private void Awake()
    {
        Instance = this;
        HideDescription();
    }

    private void LateUpdate()
    {
        if (CurrentRecipeId == -1)
        {
            return;
        }

        CraftButton.interactable = CanCraft(CurrentRecipeId);
    }

    private string CreateDescription(int recipeId)
    {
        var recipe = Recipes[recipeId];
        var description = string.Empty;

        foreach (var ingredient in recipe.GetIngredientList())
        {
            var name = ingredient.GetItem().GetName();
            var count = Inventory.Instance.GetItemCount(ingredient.GetItem());
            var required = ingredient.GetCount();

            description += string.Format("{0} {1}/{2}\n", name, count, required);
        }

        return description;
    }

    public void ToggleDescription(int recipeId)
    {
        switch (DescriptionUI.activeInHierarchy)
        {
            case true:
                HideDescription();
                break;

            case false:
                ShowDescription(recipeId);
                break;
        }
    }

    public void ShowDescription(int recipeId)
    {
        var recipe = Recipes[recipeId];

        var name = recipe.GetCraftingResult().GetName();
        var description = CreateDescription(recipeId);

        DescriptionUI.SetActive(true);
        DescriptionNameText.text = name;
        DescriptionText.text = description;
        CurrentRecipeId = recipeId;

        ToggleRecipeClickEvent?.Invoke();
    }

    public void HideDescription()
    {
        DescriptionUI.SetActive(false);
        CurrentRecipeId = -1;

        ToggleRecipeClickEvent?.Invoke();
    }

    private void UpdateDescription()
    {
        DescriptionText.text = CreateDescription(CurrentRecipeId);
    }

    private void CreateTrapRecipeUI(Transform parent, int id)
    {
        var recipeUI = Instantiate(TrapRecipeUIPrefab).GetComponent<TrapCraftingRecipeUI>();
        recipeUI.transform.SetParent(parent, false);
        recipeUI.SetState(id, Recipes[id].GetCraftingResult().GetName());
    }

    public void DiscoverTrap(TrapCraftingRecipe recipe)
    {
        int id = Recipes.Count;
        Recipes.Add(recipe);

        switch (recipe.GetRecipeType())
        {
            case Type occultist when occultist == typeof(ItemTrapOccultist):
                CreateTrapRecipeUI(OccultistTrapsUI, id);
                break;

            case Type standard when standard == typeof(ItemTrapStandard):
                CreateTrapRecipeUI(StandardTrapsUI, id);
                break;

            case Type symbol when symbol == typeof(ItemTrapSymbol):
                CreateTrapRecipeUI(SymbolsTrapsUI, id);
                break;

            case Type seal when seal == typeof(ItemTrapSeal):
                CreateTrapRecipeUI(SealsTrapsUI, id);
                break;
        }
    }

    public bool CanCraft(int id)
    {
        if (id >= Recipes.Count)
        {
            return false;
        }

        var recipe = Recipes[id];

        foreach (var ingredient in recipe.GetIngredientList())
        {
            if (!Inventory.Instance.ContainsItemStack(ingredient.GetItem(), ingredient.GetCount()))
            {
                return false;
            }
        }

        return true;
    }

    public bool TryCraft(int id)
    {
        if (!CanCraft(id))
        {
            return false;
        }

        var recipe = Recipes[id];

        int remaining;
        Inventory.Instance.TryAddItem(recipe.GetCraftingResult(), 1, out remaining);

        foreach (var ingredient in recipe.GetIngredientList())
        {
            Inventory.Instance.RemoveItemStack(ingredient.GetItem(), ingredient.GetCount());
        }

        return true;
    }

    public void Craft()
    {
        TryCraft(CurrentRecipeId);
        UpdateDescription();
    }

    public void Back()
    {
        HideDescription();
    }
}
