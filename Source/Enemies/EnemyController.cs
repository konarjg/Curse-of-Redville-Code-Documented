using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    protected Animator Animator;
    protected NavMeshAgent Pathfinding;
    protected AudioSource SoundPlayer;

    public delegate void OnStateFinished(bool returnToPreviousState);
    public static event OnStateFinished StateFinishedEvent;

    public delegate void OnPlayerKilled(Vector3 enemyPosition);
    public static event OnPlayerKilled PlayerKilledEvent;

    public delegate void OnSetImmobilized(bool immobilized);
    public static event OnSetImmobilized SetImmobilizedEvent;

    private IEnumerator Immobilize(float immobilizeTime)
    {
        float time = immobilizeTime;

        SetImmobilizedEvent?.Invoke(true);
        Animator.SetBool("Trap Triggered", true);
        Pathfinding.isStopped = true;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        SetImmobilizedEvent?.Invoke(false);
        Pathfinding.isStopped = false;
        Animator.SetBool("Trap Triggered", false);
    }

    private void OnOccultistTrapTriggered(Trap trap, float damage, float immobilizeTime)
    {
        //TODO damage
        StartCoroutine(Immobilize(immobilizeTime));
    }

    private void OnStateChanged(EnemyState firstState, EnemyState finalState)
    {
        switch (finalState)
        {
            case EnemyState.Stalk:
                Stalk();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Special:
                Special();
                break;

            case EnemyState.Kill:
                Kill();
                break;
        }
    }

    protected void Kill()
    {
        PlayerKilledEvent?.Invoke(transform.position);
        Pathfinding.SetDestination(transform.position);
        Animator.SetBool("Chase", false);
        Animator.SetTrigger("Kill");
        SoundPlayer.Play();
    }    

    protected virtual void Stalk()
    {
        Animator.SetBool("Chase", false);
        Pathfinding.SetDestination(transform.position);
        Pathfinding.isStopped = true;
        StateFinishedEvent?.Invoke(false);
    }

    protected virtual void Chase()
    {
        Animator.SetBool("Chase", true);

        var player = PlayerController.Instance.GetWorldPosition();

        Pathfinding.isStopped = false;
        Pathfinding.SetDestination(player);
        StateFinishedEvent?.Invoke(false);
    }

    protected virtual void Special()
    {
        StateFinishedEvent?.Invoke(true);
    }

    protected virtual void OnEnable()
    {
        EnemyBehaviourTree.StateChangedEvent += OnStateChanged;
        ItemTrapOccultist.OccultistTrapTriggeredEvent += OnOccultistTrapTriggered;
    }

    protected virtual void OnDisable()
    {
        EnemyBehaviourTree.StateChangedEvent -= OnStateChanged;
        ItemTrapOccultist.OccultistTrapTriggeredEvent -= OnOccultistTrapTriggered;
    }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Pathfinding = GetComponent<NavMeshAgent>();
        SoundPlayer = GetComponent<AudioSource>();
    }
}

