using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Note Item", menuName = "Curse of Redville/Items/Note")]
//Notatka
public class ItemNote : Item
{
    [SerializeField]
    private string Contents; //Tekst notatki

    //Zdarzenie dodania notatki do dziennika
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
