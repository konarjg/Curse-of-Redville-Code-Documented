using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField]
    private Item Item;
    [SerializeField]
    private int Count;

    public delegate void OnItemPickedUp(WorldItem target, Item item, int count);
    public static event OnItemPickedUp ItemPickedUpEvent;

    public delegate void OnItemPickUpAllowed(WorldItem target);
    public static event OnItemPickUpAllowed ItemPickUpAllowedEvent;

    public delegate void OnItemPickUpForbidden();
    public static event OnItemPickUpForbidden ItemPickUpForbiddenEvent;

    public string GetItemName()
    {
        return Item.GetName();
    }

    public int GetItemCount()
    {
        return Count;
    }

    public void PickUp()
    {
        ItemPickedUpEvent?.Invoke(this, Item, Count);
    }

    public void SetCount(int count)
    {
        Count = count;

        if (Count == 0)
        {
            ItemPickUpForbiddenEvent?.Invoke();
            Destroy(gameObject);
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            ItemPickUpAllowedEvent?.Invoke(this);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ItemPickUpForbiddenEvent?.Invoke();
        }
    }

    private void OnDestroy()
    {
        ItemPickUpForbiddenEvent?.Invoke();
    }
}
