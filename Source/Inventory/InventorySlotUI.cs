using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Korzystam ze wzorca projektowego controller - view, który rozdziela logikê od wyœwietlania
//Ta klasa odpowiada za zachowanie i wyœwietlanie slotu
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private InventorySlot Slot;
    [SerializeField]
    private Sprite EmptyIcon;
    [SerializeField]
    private Image SlotImage;
    [SerializeField]
    private Image ItemImage;
    [SerializeField]
    private TextMeshProUGUI CountText;

    [Space()]
    [SerializeField]
    private Color NormalColor;
    [SerializeField]
    private Color HoverColor;

    private int SlotId;
    private bool Hovered;
    private int PreviousCount;

    public void SetSlotId(int slotId)
    {
        SlotId = slotId;
    }

    private void ResetState()
    {
        PreviousCount = -1;
        SlotImage.color = NormalColor;
        Hovered = false;
    }

    private void Start()
    {
        ResetState();
    }

    private void OnDisable()
    {
        ResetState();
    }

    public int GetCount()
    {
        return Slot.GetCount();
    }

    public bool ContainsItem(Item item)
    {
        if (Slot.GetItemStack() == null)
        {
            return false;
        }

        return Slot.GetItemStack().GetItem().GetName() == item.GetName();
    }

    private void UpdateDisplay()
    {
        var currentCount = Slot.GetCount();

        if (PreviousCount != currentCount)
        {
            CountText.text = currentCount != 0 ? currentCount.ToString() : string.Empty;
            PreviousCount = currentCount;

            switch (Slot.GetItemStack())
            {
                case not null:
                    ItemImage.sprite = Slot.GetItemStack().GetItem().GetIcon();
                    break;

                case null:
                    ItemImage.sprite = EmptyIcon;
                    break;
            }
        }
    }

    private void UpdateDragAndDrop()
    {
        if (Input.GetMouseButtonUp(0) && Hovered)
        {
            var dragItem = Inventory.Instance.GetDragItem();

            if (dragItem == null)
            {
                return;
            }

            Slot.AddItemStack(ref dragItem);
            Inventory.Instance.RemoveDragItem();

            if (dragItem.GetCount() != 0)
            {
                Inventory.Instance.SetDragItem(SlotId, dragItem);
            }
        }

        if (Slot.GetItemStack() == null || !Hovered)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Slot.UseItem();
        }

        if (Input.GetMouseButtonDown(2))
        {
            var stack = Slot.RemoveItemStack();
            Inventory.Instance.DropItem(stack.GetItem(), stack.GetCount());
        }
    }

    private void UpdateDescription()
    {
        if (!Inventory.Instance.IsDescriptionVisible() && Slot.GetItemStack() != null && Hovered)
        {
            Inventory.Instance.ShowDescription(Slot.GetItemStack().GetItem().GetName(), Slot.GetItemStack().GetItem().GetDescription(), transform.GetChild(2).position);
        }
    }

    private void Update()
    {
        UpdateDisplay();
        UpdateDescription();
        UpdateDragAndDrop();
    }

    public void RemoveItemStack(int count)
    {
        if (Slot.GetItemStack() == null)
        {
            return;
        }

        Slot.SetCount(Slot.GetCount() - count);
    }

    public bool ContainsItemStack(Item item, int count)
    {
        if (Slot.GetItemStack() == null)
        {
            return false;
        }

        var stack = Slot.GetItemStack();

        if (stack.GetItem().GetName() != item.GetName())
        {
            return false;
        }

        if (count > stack.GetCount())
        {
            return false;
        }

        return true;
    }

    public bool TryAddItem(ItemStack stack, out int remainingItems)
    {
        bool result = Slot.TryAddItemStack(ref stack);
        remainingItems = stack.GetCount();

        return result;
    }

    public void AddItem(ItemStack stack)
    {
        Slot.AddItemStack(ref stack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotImage.color = HoverColor;
        Hovered = true;

        if (Slot.GetItemStack() != null)
        {
            Inventory.Instance.ShowDescription(Slot.GetItemStack().GetItem().GetName(), Slot.GetItemStack().GetItem().GetDescription(), transform.GetChild(2).position);
        }
        else
        {
            Inventory.Instance.HideDescription();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SlotImage.color = NormalColor;
        Hovered = false;
        Inventory.Instance.HideDescription();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Slot.HasItemStack() || !Hovered || eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Inventory.Instance.GetDragItem() != null)
        {
            return;
        }

        Inventory.Instance.SetDragItem(SlotId, Slot.RemoveItemStack());
    }
}
