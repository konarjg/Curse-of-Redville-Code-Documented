using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrapCraftingRecipeUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI ResultText;
    [SerializeField]
    private Color NormalColor;
    [SerializeField]
    private Color HoverColor;

    private bool IsHovered;
    private bool IsActive;
    private int RecipeId;

    private void OnToggleRecipeClick()
    {
        IsActive = !IsActive;
    }    


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != 0 || !IsActive)
        {
            return;
        }

        TrapCrafting.Instance.ShowDescription(RecipeId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }

    public void SetState(int recipeId, string name)
    {
        RecipeId = recipeId;
        ResultText.text = name;
    }

    private Color ChooseColor()
    {
        if (!IsActive)
        {
            return Color.gray;
        }

        switch (IsHovered)
        {
            case true:
                return HoverColor;

            case false:
                return NormalColor;
        }
    }

    private void UpdateColor()
    {
        ResultText.color = ChooseColor();
    }

    private void OnEnable()
    {
        TrapCrafting.ToggleRecipeClickEvent += OnToggleRecipeClick;
    }

    private void OnDisable()
    {
        TrapCrafting.ToggleRecipeClickEvent -= OnToggleRecipeClick;
    }

    private void Awake()
    {
        IsActive = true;
    }

    private void LateUpdate()
    {
        UpdateColor();
    }
}
