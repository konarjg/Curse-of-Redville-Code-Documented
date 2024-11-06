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
    private TextMeshProUGUI CurrentNoteNameText; //Tekst z nazw� aktualnej notatki
    [SerializeField]
    private GameObject NoteUIPrefab; //Potrzebne do dodawania notatki do dziennika
    [SerializeField]
    private TextMeshProUGUI ContentsText; //Miejsce na zawarto�� notatki

    private List<NoteUI> Notes = new List<NoteUI>(); //Lista notatek (ich interfejs�w)
    private NoteUI CurrentNote; //Aktualnie zaznaczona notatka (jej interfejs)
    public static Journal Instance; //Globalny dost�p do dziennika

    //Dodanie notatki
    public void AddNote(string name, string contents)
    {
        //Spawnujemy interfejs notatki i ustawiamy mu odpowiedni stan nast�pnie dodajemy j� do listy
        var noteUI = Instantiate(NoteUIPrefab).GetComponent<NoteUI>();
        noteUI.transform.SetParent(JournalUI, false);
        noteUI.transform.position += new Vector3(0f, 10f, 0f);
        noteUI.SetState(Notes.Count, new Note(name, contents));
        Notes.Add(noteUI);
        noteUI.UpdateState();
    }

    //Wybieranie aktualnie czytanej notatki
    public void SelectNote(int id)
    {
        //Niepoprawne id
        if (id >= Notes.Count)
        {
            return;
        }

        //Odznacz wszystkie notatki
        foreach (var note in Notes)
        {
            note.Unselect();
        }

        //Zaznacz aktualn� notatk� i wy�wietl jej zawarto��
        Notes[id].Select();
        ContentsText.text = Notes[id].GetContents();
        CurrentNote = Notes[id];
        CurrentNoteNameText.text = Notes[id].GetName();
    }

    private void Awake()
    {
        Instance = this;
    }

    //Interfejs tak jak kamer� w teorii powinno si� aktualizowa� w LateUpdate
    private void LateUpdate()
    {
        //je�li nie mamy notatek to zresetuj stan dziennika
        if (Notes.Count == 0)
        {
            CurrentNote = null;
            ContentsText.text = string.Empty;
            CurrentNoteNameText.text = string.Empty;
            return;
        }

        //Je�li nie mamy �adnej wybranej notatki wybierz notatk� 1
        if (CurrentNote == null)
        {
            SelectNote(0);
            CurrentNote = Notes[0];
        }
    }
}
