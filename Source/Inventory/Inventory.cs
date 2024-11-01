using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Transform InventoryUI;
    [SerializeField]
    private GameObject SlotPrefab;
    [SerializeField]
    private int Rows;
    [SerializeField]
    private int Columns;

    [Space()]
    [SerializeField]
    private DraggedItem DraggedItemUI;
    [SerializeField]
    private GameObject DescriptionUI;
    [SerializeField]
    private TextMeshProUGUI DescriptionNameText;
    [SerializeField]
    private TextMeshProUGUI DescriptionText;

    private List<InventorySlotUI> Slots = new List<InventorySlotUI>();
    private ItemStack DragItem;

    public static Inventory Instance;
    
    private void CreateInventoryUI()
    {
        int slotId = 0;

        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                var slot = Instantiate(SlotPrefab);
                slot.transform.SetParent(InventoryUI, false);
                Slots.Add(slot.GetComponent<InventorySlotUI>());
                Slots[slotId].SetSlotId(slotId);
                slotId++;
            }
        }
    }

    public int GetItemCount(Item item)
    {
        int count = 0;

        foreach (var slot in Slots)
        {
            if (slot.ContainsItem(item))
            {
                count += slot.GetCount();
            }
        }

        return count;
    }

    public bool TryAddItem(Item item, int count, out int remainingItems)
    {
        foreach (var slot in Slots)
        {
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems) && remainingItems == 0)
            {
                return true;
            }
        }

        foreach (var slot in Slots)
        {
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems))
            {
                return true;
            }
        }

        remainingItems = count;
        return false;
    }

    public void RemoveDragItem()
    {
        DragItem = null;
        DraggedItemUI.gameObject.SetActive(false);
    }

    public void SetDragItem(int slotId, ItemStack stack)
    {
        if (DragItem == null)
        {
            DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
            DraggedItemUI.gameObject.SetActive(true);
            DraggedItemUI.UpdateState(DragItem);
            return;
        }

        DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
        Slots[slotId].AddItem(DragItem);
        DraggedItemUI.gameObject.SetActive(true);
        DraggedItemUI.UpdateState(DragItem);
    }

    public ItemStack GetDragItem()
    {
        return DragItem;
    }

    public GameObject GetUI()
    {
        return InventoryUI.gameObject;
    }

    public bool IsDescriptionVisible()
    {
        return DescriptionUI.activeInHierarchy;
    }

    public void ShowDescription(string name, string description, Vector3 position)
    {
        DescriptionUI.SetActive(true);
        DescriptionUI.transform.position = position;
        DescriptionNameText.text = name;
        DescriptionText.text = description;
    }

    public void HideDescription()
    {
        DescriptionUI.SetActive(false);
    }

    public void DropItem(Item item, int count)
    {
        var position = PlayerController.Instance.GetWorldPosition() + 1.5f * PlayerController.Instance.GetForwardDirection();
        var model = Instantiate(item.GetModel(), position, Quaternion.identity).GetComponent<WorldItem>();
        model.SetCount(count);
    }

    public bool ContainsItemStack(Item item, int count)
    {
        foreach (var slot in Slots)
        {
            if (slot.ContainsItemStack(item, count))
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveItemStack(Item item, int count)
    {
        foreach (var slot in Slots)
        {
            if (slot.ContainsItemStack(item, count))
            {
                slot.RemoveItemStack(count);
            }
        }
    }

    private void Awake()
    {
        CreateInventoryUI();
        DragItem = null;
        Instance = this;
    }
}
