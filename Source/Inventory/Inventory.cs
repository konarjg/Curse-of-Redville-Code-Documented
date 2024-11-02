using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Transform InventoryUI; //Po�o�enie interfejsu ekwipunku w �wiecie gry
    [SerializeField]
    private GameObject SlotPrefab; //Prefabrykat slotu w ekwipunku, obiektu oskryptowanego, ale tworzonego dynamicznie kodem
    [SerializeField]
    private int Rows;
    [SerializeField]
    private int Columns;

    [Space()]
    [SerializeField]
    private DraggedItem DraggedItemUI; //Miejsce na przeci�gany przedmiot
    [SerializeField]
    private GameObject DescriptionUI; //Miejsce na opis przedmiotu
    [SerializeField]
    private TextMeshProUGUI DescriptionNameText; //Tekst nazwy przedmiotu w oknie opisu przedmiotu
    [SerializeField]
    private TextMeshProUGUI DescriptionText; //Tekst opisu w oknie opisu przedmiotu

    private List<InventorySlotUI> Slots = new List<InventorySlotUI>(); //Lista slot�w w ekwipunku
    private ItemStack DragItem; //Przeci�gany stack przedmiot�w

    public static Inventory Instance; //Globalny dost�p do ekwipunku gracza
    
    //Tworzy sloty dynamicznie w panelu ekwipunku
    private void CreateInventoryUI()
    {
        //Index slotu
        int slotId = 0;

        //Iterujemy po wszystkich slotach
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                //Spawnujemy slot na bazie prefabrykatu
                var slot = Instantiate(SlotPrefab);
                //Ustawiamy rodzica slotu na panel ekwipunku
                slot.transform.SetParent(InventoryUI, false);
                //Dodajemy slot do listy slot�w
                Slots.Add(slot.GetComponent<InventorySlotUI>());
                //Ustawiamy id slotu na index
                Slots[slotId].SetSlotId(slotId);
                //Przechodzimy o 1 slot dalej
                slotId++;
            }
        }
    }

    //Zwraca ilo�� danego typu przedmiotu jaka znajduje si� sumarycznie w ekwipunku gracza
    public int GetItemCount(Item item)
    {
        int count = 0;

        //Dla ka�dego slotu w slotach 
        foreach (var slot in Slots)
        {
            //Je�li slot zawiera przedmiot o typie item
            if (slot.ContainsItem(item))
            {
                //zwi�ksz sum� o ilo�� danego przedmiotu w obecnym slocie
                count += slot.GetCount();
            }
        }

        return count;
    }

    //Spr�buj doda� przedmiot do ekwipunku
    public bool TryAddItem(Item item, int count, out int remainingItems)
    {
        foreach (var slot in Slots)
        {
            //Je�li uda�o si� doda� do slotu przedmiot i nie pozosta� �aden przedmiot w stacku
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems) && remainingItems == 0)
            {
                return true;
            }
        }

        //W przeciwnym wypadku
        foreach (var slot in Slots)
        {
            //Pr�bujemy rozda� pozosta�� ilo�� ma inne sloty i zwracamy przez out to co nam pozosta�o
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems))
            {
                return true;
            }
        }

        //je�li zosta�a nam ca�a ilo�� to zwracamy to jako dodatkowy argument out (przez referencj� z wymuszon� warto�ci� wewn�trzn�)
        remainingItems = count;
        //oznacza to, �e nie uda�o nam si� umie�ci� przedmiotu w ekwipunku
        return false;
    }

    //Usu� przeci�gany item
    public void RemoveDragItem()
    {
        //Ustaw przeci�gany przedmiot na pust� referencj�
        DragItem = null;
        //Deaktywuj interfejs przeci�ganego przedmiotu na ekranie
        DraggedItemUI.gameObject.SetActive(false);
    }

    //Ustaw przeci�gany przedmiot
    public void SetDragItem(int slotId, ItemStack stack)
    {
        //Je�li przeci�gany przedmiot jest pusty to utw�rz go na bazie przedmiotu pobranego z obecnego slotu
        if (DragItem == null)
        {
            DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
            DraggedItemUI.gameObject.SetActive(true);
            DraggedItemUI.UpdateState(DragItem);
            return;
        }

        //W przeciwnym wypadku zamie� przedmioty miejscami
        DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
        Slots[slotId].AddItem(DragItem);
        DraggedItemUI.gameObject.SetActive(true);
        DraggedItemUI.UpdateState(DragItem);
    }

    //Zwr�� przeci�gany przedmiot
    public ItemStack GetDragItem()
    {
        return DragItem;
    }

    //Zwr�� interfejs
    public GameObject GetUI()
    {
        return InventoryUI.gameObject;
    }

    //Czy okno opisu jest widoczne
    public bool IsDescriptionVisible()
    {
        return DescriptionUI.activeInHierarchy;
    }

    //Poka� okno opisu
    public void ShowDescription(string name, string description, Vector3 position)
    {
        DescriptionUI.SetActive(true);
        DescriptionUI.transform.position = position;
        DescriptionNameText.text = name;
        DescriptionText.text = description;
    }

    //Schowaj okno opisu
    public void HideDescription()
    {
        DescriptionUI.SetActive(false);
    }

    //Upu�� przedmiot na ziemi�
    public void DropItem(Item item, int count)
    {
        var position = PlayerController.Instance.GetWorldPosition() + 1.5f * PlayerController.Instance.GetForwardDirection();
        var model = Instantiate(item.GetModel(), position, Quaternion.identity).GetComponent<WorldItem>();
        model.SetCount(count);
    }

    //Czy ekwipunek zawiera dany stack
    public bool ContainsItemStack(Item item, int count)
    {
        foreach (var slot in Slots)
        {
            if (slot.ContainsItemStack(item, count))
            {
                return true;
            }
        }

        return false;
    }

    //Usu� konkretny stack z ekwipunku
    public void RemoveItemStack(Item item, int count)
    {
        foreach (var slot in Slots)
        {
            if (slot.ContainsItemStack(item, count))
            {
                slot.RemoveItemStack(count);
            }
        }
    }

    //Inicjalizacja
    private void Awake()
    {
        CreateInventoryUI();
        DragItem = null;
        Instance = this;
    }
}
