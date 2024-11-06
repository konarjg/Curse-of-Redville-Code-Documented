using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

//Abstrakcyjna klasa przedmiot opisuj�ca typ przedmiot�w
//Stosujemy polimorfizm, by traktowa� ka�dy bardziej konkretny typ przedmiotu
//Jako po prostu przedmiot
//ScriptableObject -> mo�liwo�� tworzenia obiekt�w z poziomu Unity w folderze z assetami
public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private string Description;
    [SerializeField]
    private Sprite Icon;
    [SerializeField]
    private GameObject ModelPrefab; //Prefabrykat modelu przedmiotu w �wiecie gry (do spawnowania)
    [SerializeField]
    private bool Consumable; //Czy po u�yciu przedmiot ma znikn��

    //Por�wnanie dw�ch przedmiot�w
    public override bool Equals(object target)
    {
        return ((Item)target).Name == Name;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }

    public Sprite GetIcon()
    {
        return Icon;
    }

    public bool IsConsumable()
    {
        return Consumable;
    }

    public GameObject GetModel()
    {
        return ModelPrefab;
    }

    //Abstrakcyjna metoda u�ycia przedmiotu
    //Jej specyficzne dzia�anie opisuj� klasy pochodne
    //Dzi�ki temu mo�emy napisa� item.Use() nie znaj�c dok�adnego typu przedmiotu
    //Kompilator wywo�a odpowiedni� specyfikacj� tej metody zgodnie z polimorfizmem
    public abstract bool Use();
}
