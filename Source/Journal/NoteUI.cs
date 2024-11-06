using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

//Interfejs notatki (View)
public class NoteUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI NameText; //Tekst z nazw� notatki

    private int NoteId; //Id notatki
    private Note Note; //Notatka

    public string GetName()
    {
        return Note.GetName();
    }

    public string GetContents()
    {
        return Note.GetContents();
    }

    //Ustawienie stanu interfejsu na konkretn� notatk� i id
    public void SetState(int noteId, Note note)
    {
        NoteId = noteId;
        Note = note;
    }

    //Wybranie notatki
    public void Select()
    {
        NameText.text = Note.GetName() + "*";
    }

    //Odznaczenie notatki
    public void Unselect()
    {
        NameText.text = Note.GetName();
    }

    //Aktualizacja stanu
    public void UpdateState()
    {
        NameText.text = Note.GetName();
    }

    //Po klikni�ciu w nazw� notatki w interfejsie dziennika wy�wietl jej zawarto��
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Journal.Instance.SelectNote(NoteId);
    }

    //Po najechaniu na nazw� notatki przyciemnij kolor
    public void OnPointerEnter(PointerEventData eventData)
    {
        NameText.color = Color.gray;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
    }

    //Po zabraniu myszki z nazwy notatki rozja�nij kolor
    public void OnPointerExit(PointerEventData eventData)
    {
        NameText.color = Color.white;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
