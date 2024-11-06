using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Korzystam ze wzorca projektowego controller - view, który rozdziela logikê od wyœwietlania
//Ta klasa odpowiada za zachowanie i wyœwietlanie slotu
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
    private TextMeshProUGUI CountText; //Tekst iloœci w stacku

    [Space()]
    [SerializeField]
    private Color NormalColor; //Normalny kolor
    [SerializeField]
    private Color HoverColor; //Kolor po najechaniu myszk¹

    private int SlotId; //Id slotu
    private bool Hovered; //Czy kursor znajduje siê na slocie
    private int PreviousCount; //Poprzednia iloœæ

    //Raczej zrozumia³e
    public void SetSlotId(int slotId)
    {
        SlotId = slotId;
    }

    //Raczej zrozumia³e
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
    //Zauwa¿, ¿e operujemy na kontrolerze nie UI
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
        //Obecna iloœæ
        var currentCount = Slot.GetCount();

        //Jeœli obecna iloœæ jest inna ni¿ poprzednia
        if (PreviousCount != currentCount)
        {
            //Zmieñ tekst iloœci na now¹ iloœæ lub pusty string
            CountText.text = currentCount != 0 ? currentCount.ToString() : string.Empty;
            //Zaktualizuj poprzedni¹ iloœæ na now¹ iloœæ
            PreviousCount = currentCount;

            //Jeœli slot nie zawiera przedmiotów wyœwietl pust¹ ikonê
            //w przeciwnym wypadku wyœwietl ikonê przedmiotu
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

    //Zaktualizuj przeci¹gniêcie i upuszczenie
    //Ponownie operujemy tylko na kontrolerze nie na UI
    private void UpdateDragAndDrop()
    {
        //Jeœli puszczono lewy przycisk myszy i kursor znajduje siê na slocie
        if (Input.GetMouseButtonUp(0) && Hovered)
        {
            //Uzyskujemy przeci¹gany przedmiot 
            var dragItem = Inventory.Instance.GetDragItem();

            //Jeœli nie ma takiego przedmiotu nic nie robimy (aktualnie nic nie przeci¹gamy)
            if (dragItem == null)
            {
                return;
            }

            //Dodaj przedmiot do slotu i usuñ przeci¹gany przedmiot
            Slot.AddItemStack(ref dragItem);
            Inventory.Instance.RemoveDragItem();

            //jeœli nie uda³o siê wrzuciæ ca³ego stacku ponownie ustaw przeci¹gany przedmiot
            if (dragItem.GetCount() != 0)
            {
                Inventory.Instance.SetDragItem(SlotId, dragItem);
            }
        }

        //Jeœli w przedmiocie nie ma nic lub nie ma na nim kursora nic nie rób
        if (Slot.GetItemStack() == null || !Hovered)
        {
            return;
        }

        //Jeœli klikniemy prawy przycisk myszy, u¿yj przedmiotu w slocie
        if (Input.GetMouseButtonDown(1))
        {
            Slot.UseItem();
        }

        //Jeœli naciœniêto œrodkowy przycisk myszy (zostanie to zmienione na X) upuœæ przedmiot na ziemiê
        if (Input.GetMouseButtonDown(2))
        {
            var stack = Slot.RemoveItemStack();
            Inventory.Instance.DropItem(stack.GetItem(), stack.GetCount());
        }
    }

    //Aktualizuj opis przedmiotu
    private void UpdateDescription()
    {
        //Jeœli najechaliœmy na slot i jest w nim przedmiot wyœwietl opis przedmiotu
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

    //Zmniejsz iloœæ przedmiotu w stacku
    public void RemoveItemStack(int count)
    {
        if (Slot.GetItemStack() == null)
        {
            return;
        }

        Slot.SetCount(Slot.GetCount() - count);
    }

    //Czy slot zawiera okreœlon¹ iloœæ danego przedmiotu
    public bool ContainsItemStack(Item item, int count)
    {
        //Jeœli nie ma przedmiotu w slocie zwróæ fa³sz
        if (Slot.GetItemStack() == null)
        {
            return false;
        }

        //Uzyskaj stack ze slotu
        var stack = Slot.GetItemStack();

        //Jeœli nie jest to dany przedmiot zwróæ fa³sz
        if (stack.GetItem().GetName() != item.GetName())
        {
            return false;
        }

        //Jeœli dana iloœæ jest wiêksza ni¿ iloœæ w stacku zwróæ fa³sz
        if (count > stack.GetCount())
        {
            return false;
        }

        return true;
    }

    //Spróbuj dodaæ przedmiot i zwróæ ile nie uda³o siê dodaæ
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

    //Zdarzenie wyjœcia kursora ze slotu
    public void OnPointerExit(PointerEventData eventData)
    {
        SlotImage.color = NormalColor;
        Hovered = false;
        Inventory.Instance.HideDescription();
    }

    //Zdarzenie klikniêcia w slot
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
