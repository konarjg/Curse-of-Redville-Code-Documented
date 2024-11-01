using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Physics Constants")]
    [SerializeField]
    private float JumpHeightMultiplier;
    [SerializeField]
    private float GroundedCooldown;
    [SerializeField]
    private float GravityStrength;
    [SerializeField]
    private float StandingHeight;
    [SerializeField]
    private float CrouchingHeight;

    [Space()]
    [SerializeField]
    private float JumpSpeed;
    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float RunSpeed;
    [SerializeField]
    private float CrouchSpeed;

    [Space()]
    [SerializeField]
    private GameObject Flashlight;

    [Space()]
    [SerializeField]
    private GameObject PickUpTooltipUI;

    private CharacterController Character;
    private CameraController Camera;

    private InputAction InventoryInputAction;
    private InputAction JournalInputAction;
    private InputAction CraftingInputAction;
    private InputAction FlashlightInputAction;
    private InputAction RunInputAction;
    private InputAction CrouchInputAction;
    private InputAction JumpInputAction;
    private InputAction ItemPickUpInputAction;

    private float Speed;
    private float VerticalVelocity;
    private float GroundedTimer;
    private bool InputPaused;

    private WorldItem PickUpTarget;

    public static PlayerController Instance;

    public bool IsRunning { get => Speed == RunSpeed; }
    public bool IsCrouching { get => Speed == CrouchSpeed; }
    public bool IsFlashlightOn { get => Flashlight.activeInHierarchy; }

    public delegate void OnPlayerKillable();
    public static event OnPlayerKillable PlayerKillableEvent;

    public Camera GetCamera()
    {
        return Camera.GetComponent<Camera>();
    }

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
        Instance = this;
        VerticalVelocity = 0f;
        Speed = WalkSpeed;
        Flashlight.SetActive(false);
        InputPaused = false;

        Inventory.Instance.gameObject.SetActive(false);
        Journal.Instance.gameObject.SetActive(false);
        TrapCrafting.Instance.gameObject.SetActive(false);

        Character = GetComponent<CharacterController>();
        Camera = GetComponentInChildren<CameraController>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            PlayerKillableEvent?.Invoke();
        }    
    }

    private void ToggleInputPause()
    {
        InputPaused = !InputPaused;
        Camera.TogglePause();
        TrapPlacer.Instance.TogglePauseInput();
    }

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

    private void Jump()
    {
        VerticalVelocity += Mathf.Sqrt(JumpHeightMultiplier * JumpSpeed * GravityStrength);
        GroundedTimer = 0f;
    }

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

    private void PickUpItem()
    {
        PickUpTarget.PickUp();
    }

    #endregion

    #region Event Handlers

    private void OnTrapDiscovered(TrapCraftingRecipe recipe)
    {
        TrapCrafting.Instance.DiscoverTrap(recipe);
    }

    private void OnStartPlacingTrap(GameObject trapPrefab)
    {
        TrapPlacer.Instance.StartPlacingTrap(trapPrefab);
    }
     
    private void OnItemPickedUp(WorldItem target, Item item, int count)
    {
        int remaining;

        Inventory.Instance.TryAddItem(item, count, out remaining);

        target.SetCount(remaining);
    }

    private void OnNoteAddedToJournal(string name, string contents)
    {
        Journal.Instance.AddNote(name, contents);
    }

    private void OnItemPickUpAllowed(WorldItem target)
    {
        var text = string.Format("Pick Up {0} x{1}", target.GetItemName(), target.GetItemCount());

        PickUpTarget = target;
        PickUpTooltipUI.SetActive(true);
        PickUpTooltipUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void OnItemPickUpForbidden()
    {
        PickUpTarget = null;
        PickUpTooltipUI.SetActive(false);
    }

    private void OnPlayerKilled(Vector3 enemyPosition)
    {
        Camera.LookAt(enemyPosition, 15f);
        enabled = false;
        Camera.enabled = false;
    }

    #endregion

    public Transform GetAim()
    {
        return Camera.GetTarget();
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public Vector3 GetForwardDirection()
    {
        return Camera.GetTarget().forward;
    }

    public void LookAt(Vector3 position)
    {
        Camera.LookAt(position);
    }

    private void ReadInputCrouching()
    {
        if (CrouchInputAction.IsTriggered())
        {
            StandUp();
        }
    }

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

    private void Update()
    {
        ReadInput();
    }
}