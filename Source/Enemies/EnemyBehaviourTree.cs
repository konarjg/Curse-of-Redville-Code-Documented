using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviourTree : MonoBehaviour
{
    [SerializeField]
    private EnemyStateMachine StateMachine;
    [SerializeField]
    private EnemyState CurrentState;
    [SerializeField]
    private EnemyState PreviousState;

    [SerializeField]
    private float Cooldown;
    [SerializeField]
    private float SecondaryCooldown;
    [SerializeField]
    private bool IsEnabled;

    private bool InRange;
    private bool IsImmobilized;

    public delegate void OnStateChanged(EnemyState firstState, EnemyState finalState);
    public static event OnStateChanged StateChangedEvent;

    private void OnSetImmobilized(bool immobilized)
    {
        IsImmobilized = immobilized;
    }

    private void OnPlayerKillable()
    {
        if (CurrentState != EnemyState.Chase || IsImmobilized)
        {
            return;
        }    

        StateChangedEvent?.Invoke(CurrentState, EnemyState.Kill);
        IsEnabled = false;
    }

    private void OnStateFinished(bool returnToPreviousState)
    {
        IsEnabled = true;

        if (!returnToPreviousState)
        {
            return;
        }

        switch (PreviousState)
        {
            case EnemyState.Stalk:
                Initialize();
                break;

            case EnemyState.Chase:
                CurrentState = EnemyState.Chase;
                Cooldown = StateMachine.GenerateChaseSpecialCooldown();
                SecondaryCooldown = StateMachine.GenerateChaseStalkCooldown();
                break;
        }
    }

    private void Initialize()
    {
        IsEnabled = true;
        CurrentState = EnemyState.Stalk;
        Cooldown = StateMachine.GenerateStalkChaseCooldown();
        SecondaryCooldown = StateMachine.GenerateStalkSpecialCooldown();
    }

    private void UpdateState()
    {
        if (!IsEnabled)
        {
            return;
        }

        switch (CurrentState)
        {
            case EnemyState.Stalk:
                if (SecondaryCooldown <= 0f)
                {
                    PreviousState = CurrentState;
                    CurrentState = EnemyState.Special;
                    SecondaryCooldown = StateMachine.GenerateStalkSpecialCooldown();
                    IsEnabled = false;
                    break;
                }

                if (Cooldown <= 0f)
                {
                    PreviousState = CurrentState;
                    CurrentState = EnemyState.Chase;
                    Cooldown = StateMachine.GenerateChaseSpecialCooldown();
                    SecondaryCooldown = StateMachine.GenerateChaseStalkCooldown();
                    IsEnabled = false;
                }

                break;

            case EnemyState.Chase:
                if (SecondaryCooldown <= 0f)
                {
                    PreviousState = CurrentState;
                    CurrentState = EnemyState.Stalk;
                    Cooldown = StateMachine.GenerateStalkChaseCooldown();
                    SecondaryCooldown = StateMachine.GenerateStalkSpecialCooldown();
                    IsEnabled = false;
                    break;
                }

                if (Cooldown <= 0f)
                {
                    PreviousState = CurrentState;
                    CurrentState = EnemyState.Special;
                    Cooldown = StateMachine.GenerateChaseSpecialCooldown();
                    SecondaryCooldown = StateMachine.GenerateChaseStalkCooldown();
                    IsEnabled = false;
                }

                break;
        }

        StateChangedEvent?.Invoke(PreviousState, CurrentState);
    }

    private void UpdateCooldowns()
    {
        if (!IsEnabled)
        {
            return;
        }

        Cooldown -= Time.deltaTime;
        SecondaryCooldown -= Time.deltaTime;
    }

    private void OnEnable()
    {
        EnemyController.StateFinishedEvent += OnStateFinished;
        EnemyController.SetImmobilizedEvent += OnSetImmobilized;
        PlayerController.PlayerKillableEvent += OnPlayerKillable;
    }

    private void OnDisable()
    {
        EnemyController.StateFinishedEvent -= OnStateFinished;
        EnemyController.SetImmobilizedEvent -= OnSetImmobilized;
        PlayerController.PlayerKillableEvent -= OnPlayerKillable;
    }

    private void Awake()
    {
        InRange = false;
        Initialize();
    }

    private void Update()
    {
        if (!InRange || IsImmobilized)
        {
            return;
        }

        UpdateState();
        UpdateCooldowns();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InRange = true;
        }
    }
}
