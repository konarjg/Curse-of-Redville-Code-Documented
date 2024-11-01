using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTrap : Item
{
    [SerializeField]
    private GameObject TrapPrefab;

    public delegate void OnStartPlacingTrap(GameObject trapPrefab);
    public static event OnStartPlacingTrap StartPlacingTrapEvent;

    public override bool Use()
    {
        if (!TrapPlacer.Instance.CanPlaceTrap())
        {
            return false;
        }

        StartPlacingTrapEvent?.Invoke(TrapPrefab);
        return true;
    }

    public abstract void Trigger(Trap trap);
}
