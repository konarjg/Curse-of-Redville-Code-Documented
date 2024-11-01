using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class InventorySlot
{
    private ItemStack StoredItem;

    public ItemStack RemoveItemStack()
    {
        ItemStack stack = StoredItem;
        StoredItem = null;

        return stack;
    }

    public int GetCount()
    {
        if (StoredItem == null)
        {
            return 0;
        }

        return StoredItem.GetCount();
    }

    public void SetCount(int count)
    {
        if (count == 0)
        {
            StoredItem = null;
            return;
        }

        StoredItem.SetCount(count);
    }

    public bool HasItemStack()
    {
        return StoredItem != null;
    }

    public ItemStack GetItemStack()
    {
        return StoredItem;
    }

    public bool TryAddItemStack(ref ItemStack item)
    {
        if (StoredItem == null)
        {
            StoredItem = new ItemStack(item.GetItem(), item.GetCount());
            item.SetCount(0);
            return true;
        }

        if (StoredItem.GetItem().GetName() != item.GetItem().GetName())
        {
            return false;
        }

        int newCount = StoredItem.GetCount() + item.GetCount();

        if (newCount > 99)
        {
            StoredItem.SetCount(99);
            item.SetCount(newCount - 99);
            return true;
        }

        StoredItem.SetCount(newCount);
        item.SetCount(0);

        return true;
    }

    public void AddItemStack(ref ItemStack item)
    {
        if (StoredItem == null)
        {
            StoredItem = new ItemStack(item.GetItem(), item.GetCount());
            item.SetCount(0);
            return;
        }

        if (StoredItem.GetItem().GetName() != item.GetItem().GetName())
        {
            return;
        }

        int newCount = StoredItem.GetCount() + item.GetCount();

        if (newCount > 99)
        {
            StoredItem.SetCount(99);
            item.SetCount(newCount - 99);
            return;
        }

        StoredItem.SetCount(newCount);
        item.SetCount(0);
    }

    public void UseItem()
    {
        bool used = StoredItem.GetItem().Use();

        if (!used)
        {
            return;
        }

        if (StoredItem.GetItem().IsConsumable())
        {
            StoredItem.SetCount(StoredItem.GetCount() - 1);
        }

        if (StoredItem.GetCount() == 0)
        {
            StoredItem = null;
        }
    }
}
