using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

//Abstrakcyjna klasa przedmiot opisuj¹ca typ przedmiotów
//Stosujemy polimorfizm, by traktowaæ ka¿dy bardziej konkretny typ przedmiotu
//Jako po prostu przedmiot
//ScriptableObject -> mo¿liwoœæ tworzenia obiektów z poziomu Unity w folderze z assetami
public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private string Description;
    [SerializeField]
    private Sprite Icon;
    [SerializeField]
    private GameObject ModelPrefab; //Prefabrykat modelu przedmiotu w œwiecie gry (do spawnowania)
    [SerializeField]
    private bool Consumable; //Czy po u¿yciu przedmiot ma znikn¹æ

    //Porównanie dwóch przedmiotów
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

    //Abstrakcyjna metoda u¿ycia przedmiotu
    //Jej specyficzne dzia³anie opisuj¹ klasy pochodne
    //Dziêki temu mo¿emy napisaæ item.Use() nie znaj¹c dok³adnego typu przedmiotu
    //Kompilator wywo³a odpowiedni¹ specyfikacjê tej metody zgodnie z polimorfizmem
    public abstract bool Use();
}
