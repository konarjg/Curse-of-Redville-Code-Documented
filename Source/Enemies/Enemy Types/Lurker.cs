using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Lurker : EnemyController
{
    [SerializeField]
    private float TeleportRadius;
    [SerializeField]
    private bool IsVisible;

    private List<float> Angles = new List<float>();

    private void OnEnemySetVisible(bool visible)
    {
        IsVisible = visible;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EnemyVisibility.EnemySetVisibleEvent += OnEnemySetVisible;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EnemyVisibility.EnemySetVisibleEvent -= OnEnemySetVisible;
    }

    protected override void Awake()
    {
        base.Awake();

        for (float angle = 0f; angle <= 360f; angle += 30f)
        {
            Angles.Add(angle);
        }
    }

    private Vector3 CirclePosition(Vector3 center, float angle)
    {
        var x = center.x + TeleportRadius * Mathf.Cos(angle);
        var z = center.z + TeleportRadius * Mathf.Sin(angle);

        return new Vector3(x, center.y, z);
    }

    protected override void Chase()
    {
        base.Chase();

        if (IsVisible)
        {
            Pathfinding.isStopped = true;
            Animator.SetBool("Chase", false);
        }
    }

    protected override void Special()
    {
        var angles = Angles.OrderBy(key => Random.Range(float.MinValue, float.MaxValue));
        var center = PlayerController.Instance.GetWorldPosition();

        foreach (var angle in angles)
        {
            if (Pathfinding.Warp(CirclePosition(center, angle)))
            {
                transform.LookAt(center);
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
                break;
            }
        }

        base.Special();
    }
}
