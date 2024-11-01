using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacer : MonoBehaviour
{
    [SerializeField]
    private float TrapSpeed;

    private InputAction RotateInputAction;
    private Trap PlacedTrap;
    private bool IsInputPaused;

    public static TrapPlacer Instance;

    private void Awake()
    {
        Instance = this;
        IsInputPaused = false;
        PlacedTrap = null;
    }

    private void Start()
    {
        RotateInputAction = InputManager.Instance.GetAction("Rotate Trap");
    }

    private void FixedUpdate()
    {
        if (PlacedTrap == null)
        {
            return;
        }

        var trapMovement = PlacedTrap.GetComponent<CharacterController>();
        var playerPosition = PlayerController.Instance.GetWorldPosition();
        var forward = PlayerController.Instance.GetForwardDirection();
        var position = playerPosition + 3f * forward;

        if (trapMovement.transform.position != position)
        {
            var move = position - trapMovement.transform.position;

            trapMovement.Move(move * TrapSpeed * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (IsInputPaused)
        {
            return;
        }

        if (PlacedTrap == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlacedTrap.Activate();
            PlacedTrap = null;
        }

        if (RotateInputAction.IsTriggered())
        {
            PlacedTrap.transform.Rotate(Vector3.up, 30f);
        }
    }

    public void TogglePauseInput()
    {
        IsInputPaused = !IsInputPaused;
    }

    public bool CanPlaceTrap()
    {
        return PlacedTrap == null;
    }

    public void StartPlacingTrap(GameObject trapPrefab)
    {
        var playerPosition = PlayerController.Instance.GetWorldPosition();
        var playerForward = PlayerController.Instance.GetForwardDirection();

        var position = playerPosition + playerForward * 3f;

        var trap = Instantiate(trapPrefab, position, Quaternion.identity).GetComponent<Trap>();

        PlacedTrap = trap;
    }
}
