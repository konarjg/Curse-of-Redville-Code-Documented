using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Przedmiot przeci�gany w ekwipunku
public class DraggedItem : MonoBehaviour
{
    [SerializeField]
    private Image BackgroundImage; //Obrazek t�a
    [SerializeField]
    private Image ItemIconImage; //Obrazek ikony przedmiotu
    [SerializeField]
    private TextMeshProUGUI CountText; //Tekst ilo�ci w stacku

    [Space()]
    [SerializeField]
    private Color NormalColor; //Kolor pocz�tkowy

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

        //Je�li ilo�� nie jest zerem ustaw tekst na ilo�� w przeciwnym wypadku wpisz pusty tekst
        CountText.text = count != 0 ? count.ToString() : string.Empty;
    }

    private void Update()
    {
        //Je�li ostatnia pozycja nie jest pozycj� kursora
        if (LastPosition != Input.mousePosition)
        {
            //Ustaw przeci�gany przedmiot w kursorze
            transform.position = Input.mousePosition;
            //Ustaw ostatni� pozycj� na pozycj� kursora
            LastPosition = Input.mousePosition;
        }
    }
}
