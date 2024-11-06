using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Korzystam ze wzorca projektowego controller - view, kt�ry rozdziela logik� od wy�wietlania
//Ta klasa odpowiada za zachowanie i wy�wietlanie slotu
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private InventorySlot Slot; //Odniesienie do kontrolera
    [SerializeField]
    private Sprite EmptyIcon; //Tekstura pustej ikony
    [SerializeField]
    private Image SlotImage; //Obraz slotu
    [SerializeField]
    private Image ItemImage; //Obraz przedmiotu
    [SerializeField]
    private TextMeshProUGUI CountText; //Tekst ilo�ci w stacku

    [Space()]
    [SerializeField]
    private Color NormalColor; //Normalny kolor
    [SerializeField]
    private Color HoverColor; //Kolor po najechaniu myszk�

    private int SlotId; //Id slotu
    private bool Hovered; //Czy kursor znajduje si� na slocie
    private int PreviousCount; //Poprzednia ilo��

    //Raczej zrozumia�e
    public void SetSlotId(int slotId)
    {
        SlotId = slotId;
    }

    //Raczej zrozumia�e
    private void ResetState()
    {
        PreviousCount = -1;
        SlotImage.color = NormalColor;
        Hovered = false;
    }

    private void Start()
    {
        ResetState();
    }

    private void OnDisable()
    {
        ResetState();
    }

    public int GetCount()
    {
        return Slot.GetCount();
    }

    //Czy slot zawiera przedmiot o typie item
    //Zauwa�, �e operujemy na kontrolerze nie UI
    public bool ContainsItem(Item item)
    {
        if (Slot.GetItemStack() == null)
        {
            return false;
        }

        return Slot.GetItemStack().GetItem().GetName() == item.GetName();
    }

    //Aktualizacja widoku
    private void UpdateDisplay()
    {
        //Obecna ilo��
        var currentCount = Slot.GetCount();

        //Je�li obecna ilo�� jest inna ni� poprzednia
        if (PreviousCount != currentCount)
        {
            //Zmie� tekst ilo�ci na now� ilo�� lub pusty string
            CountText.text = currentCount != 0 ? currentCount.ToString() : string.Empty;
            //Zaktualizuj poprzedni� ilo�� na now� ilo��
            PreviousCount = currentCount;

            //Je�li slot nie zawiera przedmiot�w wy�wietl pust� ikon�
            //w przeciwnym wypadku wy�wietl ikon� przedmiotu
            switch (Slot.GetItemStack())
            {
                case not null:
                    ItemImage.sprite = Slot.GetItemStack().GetItem().GetIcon();
                    break;

                case null:
                    ItemImage.sprite = EmptyIcon;
                    break;
            }
        }
    }

    //Zaktualizuj przeci�gni�cie i upuszczenie
    //Ponownie operujemy tylko na kontrolerze nie na UI
    private void UpdateDragAndDrop()
    {
        //Je�li puszczono lewy przycisk myszy i kursor znajduje si� na slocie
        if (Input.GetMouseButtonUp(0) && Hovered)
        {
            //Uzyskujemy przeci�gany przedmiot 
            var dragItem = Inventory.Instance.GetDragItem();

            //Je�li nie ma takiego przedmiotu nic nie robimy (aktualnie nic nie przeci�gamy)
            if (dragItem == null)
            {
                return;
            }

            //Dodaj przedmiot do slotu i usu� przeci�gany przedmiot
            Slot.AddItemStack(ref dragItem);
            Inventory.Instance.RemoveDragItem();

            //je�li nie uda�o si� wrzuci� ca�ego stacku ponownie ustaw przeci�gany przedmiot
            if (dragItem.GetCount() != 0)
            {
                Inventory.Instance.SetDragItem(SlotId, dragItem);
            }
        }

        //Je�li w przedmiocie nie ma nic lub nie ma na nim kursora nic nie r�b
        if (Slot.GetItemStack() == null || !Hovered)
        {
            return;
        }

        //Je�li klikniemy prawy przycisk myszy, u�yj przedmiotu w slocie
        if (Input.GetMouseButtonDown(1))
        {
            Slot.UseItem();
        }

        //Je�li naci�ni�to �rodkowy przycisk myszy (zostanie to zmienione na X) upu�� przedmiot na ziemi�
        if (Input.GetMouseButtonDown(2))
        {
            var stack = Slot.RemoveItemStack();
            Inventory.Instance.DropItem(stack.GetItem(), stack.GetCount());
        }
    }

    //Aktualizuj opis przedmiotu
    private void UpdateDescription()
    {
        //Je�li najechali�my na slot i jest w nim przedmiot wy�wietl opis przedmiotu
        if (!Inventory.Instance.IsDescriptionVisible() && Slot.GetItemStack() != null && Hovered)
        {
            Inventory.Instance.ShowDescription(Slot.GetItemStack().GetItem().GetName(), Slot.GetItemStack().GetItem().GetDescription(), transform.GetChild(2).position);
        }
    }

    private void Update()
    {
        UpdateDisplay();
        UpdateDescription();
        UpdateDragAndDrop();
    }

    //Zmniejsz ilo�� przedmiotu w stacku
    public void RemoveItemStack(int count)
    {
        if (Slot.GetItemStack() == null)
        {
            return;
        }

        Slot.SetCount(Slot.GetCount() - count);
    }

    //Czy slot zawiera okre�lon� ilo�� danego przedmiotu
    public bool ContainsItemStack(Item item, int count)
    {
        //Je�li nie ma przedmiotu w slocie zwr�� fa�sz
        if (Slot.GetItemStack() == null)
        {
            return false;
        }

        //Uzyskaj stack ze slotu
        var stack = Slot.GetItemStack();

        //Je�li nie jest to dany przedmiot zwr�� fa�sz
        if (stack.GetItem().GetName() != item.GetName())
        {
            return false;
        }

        //Je�li dana ilo�� jest wi�ksza ni� ilo�� w stacku zwr�� fa�sz
        if (count > stack.GetCount())
        {
            return false;
        }

        return true;
    }

    //Spr�buj doda� przedmiot i zwr�� ile nie uda�o si� doda�
    public bool TryAddItem(ItemStack stack, out int remainingItems)
    {
        bool result = Slot.TryAddItemStack(ref stack);
        remainingItems = stack.GetCount();

        return result;
    }

    //Dodaj przedmiot do slotu
    public void AddItem(ItemStack stack)
    {
        Slot.AddItemStack(ref stack);
    }

    //Zdarzenie najechania kursorem na slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotImage.color = HoverColor;
        Hovered = true;

        if (Slot.GetItemStack() != null)
        {
            Inventory.Instance.ShowDescription(Slot.GetItemStack().GetItem().GetName(), Slot.GetItemStack().GetItem().GetDescription(), transform.GetChild(2).position);
        }
        else
        {
            Inventory.Instance.HideDescription();
        }
    }

    //Zdarzenie wyj�cia kursora ze slotu
    public void OnPointerExit(PointerEventData eventData)
    {
        SlotImage.color = NormalColor;
        Hovered = false;
        Inventory.Instance.HideDescription();
    }

    //Zdarzenie klikni�cia w slot
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Slot.HasItemStack() || !Hovered || eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Inventory.Instance.GetDragItem() != null)
        {
            return;
        }

        Inventory.Instance.SetDragItem(SlotId, Slot.RemoveItemStack());
    }
}
