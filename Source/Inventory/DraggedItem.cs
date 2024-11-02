using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Przedmiot przeci¹gany w ekwipunku
public class DraggedItem : MonoBehaviour
{
    [SerializeField]
    private Image BackgroundImage; //Obrazek t³a
    [SerializeField]
    private Image ItemIconImage; //Obrazek ikony przedmiotu
    [SerializeField]
    private TextMeshProUGUI CountText; //Tekst iloœci w stacku

    [Space()]
    [SerializeField]
    private Color NormalColor; //Kolor pocz¹tkowy

    private Vector3 LastPosition; //Ostatnia pozycja

    //Inicjalizacja
    private void Start()
    {
        BackgroundImage.color = NormalColor;
    }

    //Aktualizacja stanu na podstawie podanego stacku
    public void UpdateState(ItemStack item)
    {
        var count = item.GetCount();
        ItemIconImage.sprite = item.GetItem().GetIcon();

        //Jeœli iloœæ nie jest zerem ustaw tekst na iloœæ w przeciwnym wypadku wpisz pusty tekst
        CountText.text = count != 0 ? count.ToString() : string.Empty;
    }

    private void Update()
    {
        //Jeœli ostatnia pozycja nie jest pozycj¹ kursora
        if (LastPosition != Input.mousePosition)
        {
            //Ustaw przeci¹gany przedmiot w kursorze
            transform.position = Input.mousePosition;
            //Ustaw ostatni¹ pozycjê na pozycjê kursora
            LastPosition = Input.mousePosition;
        }
    }
}
