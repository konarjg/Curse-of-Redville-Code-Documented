using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class Item : ScriptableObject
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private string Description;
    [SerializeField]
    private Sprite Icon;
    [SerializeField]
    private GameObject ModelPrefab;
    [SerializeField]
    private bool Consumable;

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

    public abstract bool Use();
}
