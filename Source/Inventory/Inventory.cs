using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Transform InventoryUI; //Po³o¿enie interfejsu ekwipunku w œwiecie gry
    [SerializeField]
    private GameObject SlotPrefab; //Prefabrykat slotu w ekwipunku, obiektu oskryptowanego, ale tworzonego dynamicznie kodem
    [SerializeField]
    private int Rows;
    [SerializeField]
    private int Columns;

    [Space()]
    [SerializeField]
    private DraggedItem DraggedItemUI; //Miejsce na przeci¹gany przedmiot
    [SerializeField]
    private GameObject DescriptionUI; //Miejsce na opis przedmiotu
    [SerializeField]
    private TextMeshProUGUI DescriptionNameText; //Tekst nazwy przedmiotu w oknie opisu przedmiotu
    [SerializeField]
    private TextMeshProUGUI DescriptionText; //Tekst opisu w oknie opisu przedmiotu

    private List<InventorySlotUI> Slots = new List<InventorySlotUI>(); //Lista slotów w ekwipunku
    private ItemStack DragItem; //Przeci¹gany stack przedmiotów

    public static Inventory Instance; //Globalny dostêp do ekwipunku gracza
    
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
                //Dodajemy slot do listy slotów
                Slots.Add(slot.GetComponent<InventorySlotUI>());
                //Ustawiamy id slotu na index
                Slots[slotId].SetSlotId(slotId);
                //Przechodzimy o 1 slot dalej
                slotId++;
            }
        }
    }

    //Zwraca iloœæ danego typu przedmiotu jaka znajduje siê sumarycznie w ekwipunku gracza
    public int GetItemCount(Item item)
    {
        int count = 0;

        //Dla ka¿dego slotu w slotach 
        foreach (var slot in Slots)
        {
            //Jeœli slot zawiera przedmiot o typie item
            if (slot.ContainsItem(item))
            {
                //zwiêksz sumê o iloœæ danego przedmiotu w obecnym slocie
                count += slot.GetCount();
            }
        }

        return count;
    }

    //Spróbuj dodaæ przedmiot do ekwipunku
    public bool TryAddItem(Item item, int count, out int remainingItems)
    {
        foreach (var slot in Slots)
        {
            //Jeœli uda³o siê dodaæ do slotu przedmiot i nie pozosta³ ¿aden przedmiot w stacku
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems) && remainingItems == 0)
            {
                return true;
            }
        }

        //W przeciwnym wypadku
        foreach (var slot in Slots)
        {
            //Próbujemy rozdaæ pozosta³¹ iloœæ ma inne sloty i zwracamy przez out to co nam pozosta³o
            if (slot.TryAddItem(new ItemStack(item, count), out remainingItems))
            {
                return true;
            }
        }

        //jeœli zosta³a nam ca³a iloœæ to zwracamy to jako dodatkowy argument out (przez referencjê z wymuszon¹ wartoœci¹ wewnêtrzn¹)
        remainingItems = count;
        //oznacza to, ¿e nie uda³o nam siê umieœciæ przedmiotu w ekwipunku
        return false;
    }

    //Usuñ przeci¹gany item
    public void RemoveDragItem()
    {
        //Ustaw przeci¹gany przedmiot na pust¹ referencjê
        DragItem = null;
        //Deaktywuj interfejs przeci¹ganego przedmiotu na ekranie
        DraggedItemUI.gameObject.SetActive(false);
    }

    //Ustaw przeci¹gany przedmiot
    public void SetDragItem(int slotId, ItemStack stack)
    {
        //Jeœli przeci¹gany przedmiot jest pusty to utwórz go na bazie przedmiotu pobranego z obecnego slotu
        if (DragItem == null)
        {
            DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
            DraggedItemUI.gameObject.SetActive(true);
            DraggedItemUI.UpdateState(DragItem);
            return;
        }

        //W przeciwnym wypadku zamieñ przedmioty miejscami
        DragItem = new ItemStack(stack.GetItem(), stack.GetCount());
        Slots[slotId].AddItem(DragItem);
        DraggedItemUI.gameObject.SetActive(true);
        DraggedItemUI.UpdateState(DragItem);
    }

    //Zwróæ przeci¹gany przedmiot
    public ItemStack GetDragItem()
    {
        return DragItem;
    }

    //Zwróæ interfejs
    public GameObject GetUI()
    {
        return InventoryUI.gameObject;
    }

    //Czy okno opisu jest widoczne
    public bool IsDescriptionVisible()
    {
        return DescriptionUI.activeInHierarchy;
    }

    //Poka¿ okno opisu
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

    //Upuœæ przedmiot na ziemiê
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

    //Usuñ konkretny stack z ekwipunku
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
