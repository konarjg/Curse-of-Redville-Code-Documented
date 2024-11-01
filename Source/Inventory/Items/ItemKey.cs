using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Item", menuName = "Curse of Redville/Items/Key")]
public class ItemKey : Item
{
    [SerializeField]
    private int KeyId;

    public delegate void OnKeyUsed(int keyId);
    public static event OnKeyUsed KeyUsedEvent;

    public override bool Use()
    {
        KeyUsedEvent?.Invoke(KeyId);
        return true;
    }
}
