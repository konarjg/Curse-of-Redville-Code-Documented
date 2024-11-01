using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Note Item", menuName = "Curse of Redville/Items/Note")]
public class ItemNote : Item
{
    [SerializeField]
    private string Contents;

    public delegate void OnNoteAddedToJournal(string name, string contents);
    public static event OnNoteAddedToJournal NoteAddedToJournalEvent;

    public string GetContents()
    {
        return Contents;    
    }

    public override bool Use()
    {
        NoteAddedToJournalEvent?.Invoke(GetName(), Contents);
        return true;
    }
}
