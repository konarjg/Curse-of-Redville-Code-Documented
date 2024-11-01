using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI NameText;

    private int NoteId;
    private Note Note;

    public string GetName()
    {
        return Note.GetName();
    }

    public string GetContents()
    {
        return Note.GetContents();
    }

    public void SetState(int noteId, Note note)
    {
        NoteId = noteId;
        Note = note;
    }

    public void Select()
    {
        NameText.text = Note.GetName() + "*";
    }

    public void Unselect()
    {
        NameText.text = Note.GetName();
    }

    public void UpdateState()
    {
        NameText.text = Note.GetName();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Journal.Instance.SelectNote(NoteId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        NameText.color = Color.gray;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NameText.color = Color.white;
        NameText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
