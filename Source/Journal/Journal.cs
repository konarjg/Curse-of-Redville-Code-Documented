using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Kontroler dziennika
public class Journal : MonoBehaviour
{
    [SerializeField]
    private Transform JournalUI; //Interfejs dziennika
    [SerializeField]
    private TextMeshProUGUI CurrentNoteNameText; //Tekst z nazw¹ aktualnej notatki
    [SerializeField]
    private GameObject NoteUIPrefab; //Potrzebne do dodawania notatki do dziennika
    [SerializeField]
    private TextMeshProUGUI ContentsText; //Miejsce na zawartoœæ notatki

    private List<NoteUI> Notes = new List<NoteUI>();
    private NoteUI CurrentNote;
    public static Journal Instance;

    public void AddNote(string name, string contents)
    {
        var noteUI = Instantiate(NoteUIPrefab).GetComponent<NoteUI>();
        noteUI.transform.SetParent(JournalUI, false);
        noteUI.transform.position += new Vector3(0f, 10f, 0f);
        noteUI.SetState(Notes.Count, new Note(name, contents));
        Notes.Add(noteUI);
        noteUI.UpdateState();
    }

    public void SelectNote(int id)
    {
        if (id >= Notes.Count)
        {
            return;
        }

        foreach (var note in Notes)
        {
            note.Unselect();
        }

        Notes[id].Select();
        ContentsText.text = Notes[id].GetContents();
        CurrentNote = Notes[id];
        CurrentNoteNameText.text = Notes[id].GetName();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (Notes.Count == 0)
        {
            CurrentNote = null;
            ContentsText.text = string.Empty;
            CurrentNoteNameText.text = string.Empty;
            return;
        }

        if (CurrentNote == null)
        {
            SelectNote(0);
            CurrentNote = Notes[0];
        }
    }
}
