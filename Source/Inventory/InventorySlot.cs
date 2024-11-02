using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

//Korzystam ze wzorca projektowego controller - view, kt�ry rozdziela logik� od wy�wietlania
//Ta klasa odpowiada za sam� logik� slotu
[Serializable]
public class InventorySlot
{
    private ItemStack StoredItem; //Przechowywany stack w slocie

    //Usuni�cie stacku ze slotu i zwr�cenie go
    public ItemStack RemoveItemStack()
    {
        ItemStack stack = StoredItem;
        StoredItem = null;

        return stack;
    }

    //Sprawdzenie ilo�ci przedmiotu w slocie
    public int GetCount()
    {
        if (StoredItem == null)
        {
            return 0;
        }

        return StoredItem.GetCount();
    }

    //Ustawienie ilo�ci przedmiotu w slocie
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

    //Spr�buj doda� stack do slotu
    //Stack przez referencj�, poniewa� mo�e co� zosta�
    public bool TryAddItemStack(ref ItemStack item)
    {
        //Je�li slot jest pusty to po prostu dodaj przedmiot do slotu w ca�o�ci
        if (StoredItem == null)
        {
            StoredItem = new ItemStack(item.GetItem(), item.GetCount());
            item.SetCount(0);
            return true;
        }

        //Je�li przedmioty r�ni� si� typami to nie mo�esz doda� tu przedmiotu
        if (StoredItem.GetItem().GetName() != item.GetItem().GetName())
        {
            return false;
        }

        //Nowa ilo��
        int newCount = StoredItem.GetCount() + item.GetCount();

        //Je�li nowa ilo�� jest wi�ksza ni� limit stacku
        if (newCount > 99)
        {
            StoredItem.SetCount(99);
            //Tyle ile zosta�o ponad limit zwr�� z powrotem
            item.SetCount(newCount - 99);
            return true;
        }

        //Przedmiot tego typu jest ju� w slocie i nie przekracza limitu po prostu zwi�ksz ilo��
        StoredItem.SetCount(newCount);
        item.SetCount(0);

        return true;
    }

    //Wymusza dodanie stacku do slotu
    //Dzia�a podobnie jak wy�ej
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

    //U�yj przedmiotu w tym slocie
    public void UseItem()
    {
        //Pr�buje wywo�a� abstrakcyjn� metod� Use na przedmiocie
        bool used = StoredItem.GetItem().Use();

        //Je�li nie uda�o si� tego zrobi� to wyjd� z metody
        if (!used)
        {
            return;
        }

        //Je�li przedmiot jest jednorazowy to zmniejsz ilo�� o 1
        if (StoredItem.GetItem().IsConsumable())
        {
            StoredItem.SetCount(StoredItem.GetCount() - 1);
        }

        //Je�li wyczerpano ca�y zapas to usu� przedmiot z ekwipunku
        if (StoredItem.GetCount() == 0)
        {
            StoredItem = null;
        }
    }
}
