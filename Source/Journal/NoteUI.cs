using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

//Interfejs notatki (View)
public class NoteUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI NameText; //Tekst z nazw¹ notatki

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

    //Ustawienie stanu interfejsu na konkretn¹ notatkê i id
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

    //Po klikniêciu w nazwê notatki w interfejsie dziennika wyœwietl jej zawartoœæ
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Journal.Instance.SelectNote(NoteId);
    }

    //Po najechaniu na nazwê notatki przyciemnij kolor
    public void OnPointerEnter(PointerEventData eventData)
    {
        NameText.color = Color.gray;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
    }

    //Po zabraniu myszki z nazwy notatki rozjaœnij kolor
    public void OnPointerExit(PointerEventData eventData)
    {
        NameText.color = Color.white;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
