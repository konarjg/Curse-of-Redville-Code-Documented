using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Curse of Redville/Items/Consumable")]
public class ItemConsumable : Item
{
    [SerializeField]
    private float SanityIncrease;

    public delegate void OnSanityIncreased(float value);
    public event OnSanityIncreased SanityIncreasedEvent;

    public override bool Use()
    {
        SanityIncreasedEvent?.Invoke(SanityIncrease);
        return true;
    }
}
