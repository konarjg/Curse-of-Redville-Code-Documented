/*
 * Ostrze¿enie, niniejszy plik jest najd³u¿szym plikiem w ca³ym projekcie
 * wymaga d³u¿szego czasu na zrozumienie
 */
using System;
using System.Collections;
using System.Collections.Generic;
//Prawdopodobnie przypadkowy import
using System.Xml.Serialization;
//Pola tekstowe z mo¿liwoœci¹ zagnie¿d¿ania cssa (podobnego do cssa jêzyka)
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Stylistyczne, wypisuje w edytorze nazwê kategorii pól
    [Header("Movement Physics Constants")]
    [SerializeField]
    private float JumpHeightMultiplier; //Mno¿nik wysokoœci skoku
    [SerializeField]
    private float GroundedCooldown; //Cooldown pomiêdzy sprawdzaniem czy gracz jest na ziemi
    [SerializeField]
    private float GravityStrength; //Przyspieszenie grawitacyjne
    [SerializeField]
    private float StandingHeight; //Wysokoœæ hitboxu podczas stania
    [SerializeField]
    private float CrouchingHeight; //Wysokoœæ hitboxu podczas kucania

    [Space()]
    [SerializeField]
    private float JumpSpeed; //Szybkoœæ skoku
    [SerializeField]
    private float WalkSpeed; //Szybkoœæ chodzenia
    [SerializeField]
    private float RunSpeed; //Szybkoœæ biegania
    [SerializeField]
    private float CrouchSpeed; //Szybkoœæ kucania

    [Space()]
    [SerializeField]
    private GameObject Flashlight; //Obiekt latarki

    [Space()]
    [SerializeField]
    private GameObject PickUpTooltipUI; //UI informuj¹ce o mo¿liwoœci podniesienia przedmiotu

    private CharacterController Character; //Cia³o fizyczne gracza (kolizja, poruszanie, skakanie itp.)
    private CameraController Camera; //Odnoœnik do kontrolera kamery

    //Klawisze przypisane do akcji gracza
    private InputAction InventoryInputAction; 
    private InputAction JournalInputAction;
    private InputAction CraftingInputAction;
    private InputAction FlashlightInputAction;
    private InputAction RunInputAction;
    private InputAction CrouchInputAction;
    private InputAction JumpInputAction;
    private InputAction ItemPickUpInputAction;

    //Aktualna szybkoœæ ruchu
    private float Speed;
    //Prêdkoœæ pionowa gracza
    private float VerticalVelocity;
    //Czas do nastêpnego sprawdzenia czy gracz stoi na ziemi
    private float GroundedTimer;
    //Czy zatrzymane jest poruszanie siê
    private bool InputPaused;

    //Aktualny mo¿liwy do podniesienia przedmiot
    private WorldItem PickUpTarget;

    //Globalny dostêp do obiektu gracza, gracz jest tylko jeden
    public static PlayerController Instance;

    //Czy gracz biegnie
    public bool IsRunning { get => Speed == RunSpeed; }
    //Czy gracz kuca
    public bool IsCrouching { get => Speed == CrouchSpeed; }
    //Czy latarka jest w³¹czona
    public bool IsFlashlightOn { get => Flashlight.activeInHierarchy; }

    //Zdarzenie, które uruchamia siê, gdy przeciwnik jest w zasiêgu zamordowania gracza
    //Wzorzec projektowy Observer, sprawia, ¿e obiekty nie musz¹ mieæ o sobie informacji,
    //a wci¹¿ móc komunikowaæ siê ze sob¹
    //zmniejsza powi¹zania miêdzy obiektami
    //u³atwiaj¹c debugowanie i utrzymanie czystego kodu
    public delegate void OnPlayerKillable();
    public static event OnPlayerKillable PlayerKillableEvent;

    //Zwrócenie obiektu kamery
    public Camera GetCamera()
    {
        return Camera.GetComponent<Camera>();
    }

    //Uruchamia siê przy ka¿dym w³¹czeniu tego skryptu
    //Przypisujemy metody obs³uguj¹ce podane zdarzenia
    private void OnEnable()
    {
        WorldItem.ItemPickUpAllowedEvent += OnItemPickUpAllowed;
        WorldItem.ItemPickUpForbiddenEvent += OnItemPickUpForbidden;
        WorldItem.ItemPickedUpEvent += OnItemPickedUp;
        ItemNote.NoteAddedToJournalEvent += OnNoteAddedToJournal;
        ItemTrap.StartPlacingTrapEvent += OnStartPlacingTrap;
        ItemTrapResearch.TrapDiscoveredEvent += OnTrapDiscovered;
        EnemyController.PlayerKilledEvent += OnPlayerKilled;
    }

    //Uruchamia siê przy ka¿dym wy³¹czeniu tego skryptu
    //Usuwamy metody z obs³ugi podanych zdarzeñ
    private void OnDisable()
    {
        WorldItem.ItemPickUpAllowedEvent -= OnItemPickUpAllowed;
        WorldItem.ItemPickUpForbiddenEvent -= OnItemPickUpForbidden;
        WorldItem.ItemPickedUpEvent -= OnItemPickedUp;
        ItemNote.NoteAddedToJournalEvent -= OnNoteAddedToJournal;
        ItemTrap.StartPlacingTrapEvent -= OnStartPlacingTrap;
        ItemTrapResearch.TrapDiscoveredEvent -= OnTrapDiscovered;
        EnemyController.PlayerKilledEvent -= OnPlayerKilled;
    }


    private void Start()
    {
        //Ustawiamy pocz¹tkowy stan wszystkich pól
        Instance = this;
        VerticalVelocity = 0f;
        Speed = WalkSpeed;
        Flashlight.SetActive(false);
        InputPaused = false;

        //Wy³¹czamy wszystkie interfejsy UI
        Inventory.Instance.gameObject.SetActive(false);
        Journal.Instance.gameObject.SetActive(false);
        TrapCrafting.Instance.gameObject.SetActive(false);

        //Pobieramy komponenty obs³uguj¹ce fizykê oraz kamerê
        Character = GetComponent<CharacterController>();
        //children -> podrzêdne obiekty w hierarchii w Unity
        Camera = GetComponentInChildren<CameraController>();

        //Pobieramy obiekty akcji z InputManagera, by móc odczytywaæ ich klawisze
        RunInputAction = InputManager.Instance.GetAction("Run");
        CrouchInputAction = InputManager.Instance.GetAction("Crouch");
        JumpInputAction = InputManager.Instance.GetAction("Jump");
        FlashlightInputAction = InputManager.Instance.GetAction("Flashlight");
        InventoryInputAction = InputManager.Instance.GetAction("Inventory");
        JournalInputAction = InputManager.Instance.GetAction("Journal");
        ItemPickUpInputAction = InputManager.Instance.GetAction("Pick Up Item");
        CraftingInputAction = InputManager.Instance.GetAction("Crafting");
    }

    #region Behaviour

    //Czy jakiœ obiekt z kolizj¹ wszed³ w obszar mordowania
    private void OnTriggerEnter(Collider other)
    {
        //Jeœli jest to przeciwnik uruchamiamy zdarzenie OnKillableEvent,
        //Przeciwnik obs³uguje to zdarzenie bez naszej wiedzy
        if (other.tag == "Enemy")
        {
            PlayerKillableEvent?.Invoke();
        }    
    }

    //Raczej logiczne
    private void ToggleInputPause()
    {
        InputPaused = !InputPaused;
        Camera.TogglePause();
        TrapPlacer.Instance.TogglePauseInput();
    }

    //Obs³uga poruszania siê o podany wektor wejœciowy, raczej zrozumia³e
    private void Move(Vector2 input)
    {
        Vector3 moveX = Camera.GetTarget().right * input.x;
        Vector3 moveY = -Physics.gravity.normalized * VerticalVelocity;
        Vector3 moveZ = Camera.GetTarget().forward * input.y;

        Vector3 moveXZ = (moveX + moveZ) * Speed;
        Vector3 move = (moveXZ + moveY) * Time.deltaTime;

        Character.Move(move);
    }

    private void Walk()
    {
        Speed = WalkSpeed;
    }

    private void Run()
    {
        Speed = RunSpeed;
    }

    private void Crouch()
    {
        Speed = CrouchSpeed;
        Character.height = CrouchingHeight;
        Character.Move(new Vector3(0f, CrouchingHeight - StandingHeight, 0f));
    }

    private void StandUp()
    {
        Speed = WalkSpeed;
        Character.Move(new Vector3(0f, StandingHeight - CrouchingHeight, 0f));
        Character.height = StandingHeight;
    }

    private void ToggleFlashlight()
    {
        Flashlight.SetActive(!Flashlight.activeInHierarchy);
    }

    //Wzór matematyczny na prêdkoœæ skoku, nie pytaj czemu tak, bo sam nie wiem
    private void Jump()
    {
        VerticalVelocity += Mathf.Sqrt(JumpHeightMultiplier * JumpSpeed * GravityStrength);
        GroundedTimer = 0f;
    }

    //W³¹czenie poszczególnych interfejsów
    private void ToggleCrafting()
    {
        ToggleInputPause();
        TrapCrafting.Instance.gameObject.SetActive(!TrapCrafting.Instance.gameObject.activeInHierarchy);
    }

    private void ToggleInventory()
    {
        ToggleInputPause();
        Inventory.Instance.gameObject.SetActive(!Inventory.Instance.gameObject.activeInHierarchy);
    }

    private void ToggleJournal()
    {
        ToggleInputPause();
        Journal.Instance.gameObject.SetActive(!Journal.Instance.gameObject.activeInHierarchy);
    }

    //Podniesienie przedmiotu w zasiêgu
    private void PickUpItem()
    {
        PickUpTarget.PickUp();
    }

    #endregion

    #region Event Handlers

    //Obs³uga zdarzenia odkrycie nowej receptury pu³apki
    private void OnTrapDiscovered(TrapCraftingRecipe recipe)
    {
        TrapCrafting.Instance.DiscoverTrap(recipe);
    }

    //Obs³uga zdarzenia rozpoczêcia stawiania pu³apki
    private void OnStartPlacingTrap(GameObject trapPrefab)
    {
        TrapPlacer.Instance.StartPlacingTrap(trapPrefab);
    }
     
    //Obs³uga zdarzenia podniesienia przedmiotu
    private void OnItemPickedUp(WorldItem target, Item item, int count)
    {
        int remaining;

        Inventory.Instance.TryAddItem(item, count, out remaining);

        target.SetCount(remaining);
    }

    //Obs³uga zdarzenia dodania notatki do dziennika
    private void OnNoteAddedToJournal(string name, string contents)
    {
        Journal.Instance.AddNote(name, contents);
    }

    //Obs³uga zdarzenia zezwolenia na podniesienie przedmiotu w zasiêgu
    private void OnItemPickUpAllowed(WorldItem target)
    {
        var text = string.Format("Pick Up {0} x{1}", target.GetItemName(), target.GetItemCount());

        PickUpTarget = target;
        PickUpTooltipUI.SetActive(true);
        PickUpTooltipUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    //Obs³uga zdarzenia zabronienia na podniesienie przedmiotu w zasiêgu
    private void OnItemPickUpForbidden()
    {
        PickUpTarget = null;
        PickUpTooltipUI.SetActive(false);
    }

    //Obs³uga zdarzenia zabicia gracza przez przeciwnika
    private void OnPlayerKilled(Vector3 enemyPosition)
    {
        Camera.LookAt(enemyPosition, 15f);
        enabled = false;
        Camera.enabled = false;
    }

    #endregion

    //Zwrócenie pozycji centralnie przed graczem
    public Transform GetAim()
    {
        return Camera.GetTarget();
    }

    //Zwrócenie pozycji gracza w œwiecie gry
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    //Zwrócenie kierunku wzroku gracza
    public Vector3 GetForwardDirection()
    {
        return Camera.GetTarget().forward;
    }

    //Spojrzenie na obiekt
    public void LookAt(Vector3 position)
    {
        Camera.LookAt(position);
    }

    //Odczytanie wejœcia podczas kucania
    private void ReadInputCrouching()
    {
        if (CrouchInputAction.IsTriggered())
        {
            StandUp();
        }
    }

    //Odczytanie wejœcia podczas stania
    private void ReadInputStanding()
    {
        if (RunInputAction.IsTriggered())
        {
            Run();
        }
        else
        {
            Walk();
        }

        if (CrouchInputAction.IsTriggered())
        {
            Crouch();
        }
    }

    //Odczytanie wejœcia globalnie
    private void ReadInput()
    {
        if (InventoryInputAction.IsTriggered() && !Journal.Instance.gameObject.activeInHierarchy && !TrapCrafting.Instance.gameObject.activeInHierarchy)
        {
            ToggleInventory();
        }

        if (JournalInputAction.IsTriggered() && !Inventory.Instance.gameObject.activeInHierarchy && !TrapCrafting.Instance.gameObject.activeInHierarchy)
        {
            ToggleJournal();
        }

        if (CraftingInputAction.IsTriggered() && !Inventory.Instance.gameObject.activeInHierarchy && !Journal.Instance.gameObject.activeInHierarchy)
        {
            ToggleCrafting();
        }

        if (InputPaused)
        {
            return;
        }

        if (ItemPickUpInputAction.IsTriggered() && PickUpTarget != null)
        {
            PickUpItem();
        }

        if (IsCrouching)
        {
            ReadInputCrouching();
        }
        else
        {
            ReadInputStanding();
        }

        if (JumpInputAction.IsTriggered() && GroundedTimer > 0)
        {
            Jump();
        }

        if (FlashlightInputAction.IsTriggered())
        {
            ToggleFlashlight();
        }

        float movementInputX = Input.GetAxis("Horizontal");
        float movementInputY = Input.GetAxis("Vertical");

        Move(new Vector2(movementInputX, movementInputY));
        HandlePhysics();
    }

    //Fizyka
    private void HandlePhysics()
    {
        if (Character.isGrounded)
        {
            GroundedTimer = GroundedCooldown;
        }

        if (GroundedTimer > 0)
        {
            GroundedTimer -= Time.deltaTime;
        }

        if (VerticalVelocity < 0f && Character.isGrounded)
        {
            VerticalVelocity = 0f;
        }

        VerticalVelocity -= GravityStrength * Time.deltaTime;
    }

    //Co klatkê odczytujemy wejœcie i realizujemy akcje
    private void Update()
    {
        ReadInput();
    }
}