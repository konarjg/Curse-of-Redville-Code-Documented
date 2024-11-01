using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ItemStack
{
    [SerializeField]
    private Item Item;
    [SerializeField]
    private int Count;

    public ItemStack(Item item, int count)
    {
        Item = item;
        Count = count;
    }

    public Item GetItem()
    {
        return Item;
    }

    public int GetCount()
    {
        return Count;
    }

    public void SetItem(Item item)
    {
        Item = item;
    }

    public void SetCount(int count)
    {
        Count = count;
    }
}