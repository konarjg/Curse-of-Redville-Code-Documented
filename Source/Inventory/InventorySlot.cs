using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

//Korzystam ze wzorca projektowego controller - view, który rozdziela logikê od wyœwietlania
//Ta klasa odpowiada za sam¹ logikê slotu
[Serializable]
public class InventorySlot
{
    private ItemStack StoredItem; //Przechowywany stack w slocie

    //Usuniêcie stacku ze slotu i zwrócenie go
    public ItemStack RemoveItemStack()
    {
        ItemStack stack = StoredItem;
        StoredItem = null;

        return stack;
    }

    //Sprawdzenie iloœci przedmiotu w slocie
    public int GetCount()
    {
        if (StoredItem == null)
        {
            return 0;
        }

        return StoredItem.GetCount();
    }

    //Ustawienie iloœci przedmiotu w slocie
    public void SetCount(int count)
    {
        if (count == 0)
        {
            StoredItem = null;
            return;
        }

        StoredItem.SetCount(count);
    }

    //Czy posiada jakikolwiek stack w slocie
    public bool HasItemStack()
    {
        return StoredItem != null;
    }

    //Zwraca stack z tego slotu
    public ItemStack GetItemStack()
    {
        return StoredItem;
    }

    //Spróbuj dodaæ stack do slotu
    //Stack przez referencjê, poniewa¿ mo¿e coœ zostaæ
    public bool TryAddItemStack(ref ItemStack item)
    {
        //Jeœli slot jest pusty to po prostu dodaj przedmiot do slotu w ca³oœci
        if (StoredItem == null)
        {
            StoredItem = new ItemStack(item.GetItem(), item.GetCount());
            item.SetCount(0);
            return true;
        }

        //Jeœli przedmioty ró¿ni¹ siê typami to nie mo¿esz dodaæ tu przedmiotu
        if (StoredItem.GetItem().GetName() != item.GetItem().GetName())
        {
            return false;
        }

        //Nowa iloœæ
        int newCount = StoredItem.GetCount() + item.GetCount();

        //Jeœli nowa iloœæ jest wiêksza ni¿ limit stacku
        if (newCount > 99)
        {
            StoredItem.SetCount(99);
            //Tyle ile zosta³o ponad limit zwróæ z powrotem
            item.SetCount(newCount - 99);
            return true;
        }

        //Przedmiot tego typu jest ju¿ w slocie i nie przekracza limitu po prostu zwiêksz iloœæ
        StoredItem.SetCount(newCount);
        item.SetCount(0);

        return true;
    }

    //Wymusza dodanie stacku do slotu
    //Dzia³a podobnie jak wy¿ej
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

    //U¿yj przedmiotu w tym slocie
    public void UseItem()
    {
        //Próbuje wywo³aæ abstrakcyjn¹ metodê Use na przedmiocie
        bool used = StoredItem.GetItem().Use();

        //Jeœli nie uda³o siê tego zrobiæ to wyjdŸ z metody
        if (!used)
        {
            return;
        }

        //Jeœli przedmiot jest jednorazowy to zmniejsz iloœæ o 1
        if (StoredItem.GetItem().IsConsumable())
        {
            StoredItem.SetCount(StoredItem.GetCount() - 1);
        }

        //Jeœli wyczerpano ca³y zapas to usuñ przedmiot z ekwipunku
        if (StoredItem.GetCount() == 0)
        {
            StoredItem = null;
        }
    }
}
