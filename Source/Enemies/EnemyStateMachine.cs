using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy State Machine", menuName = "Curse of Redville/Enemies/State Machine")]
public class EnemyStateMachine : ScriptableObject
{
    [SerializeField]
    private Vector2 StalkChaseCooldownRange;
    [SerializeField]
    private Vector2 StalkSpecialCooldownRange;
    [SerializeField]
    private Vector2 ChaseSpecialCooldownRange;
    [SerializeField]
    private Vector2 ChaseStalkCooldownRange;

    private float GenerateCooldown(Vector2 range)
    {
        var step = (range.y - range.x) / 50f;
        var cooldowns = new List<float>();

        for (float cooldown = range.x; cooldown <= range.y; cooldown += step)
        {
            cooldowns.Add(cooldown);
        }

        return cooldowns.OrderBy(x => UnityEngine.Random.Range(float.MinValue, float.MaxValue)).First();
    }

    public float GenerateStalkChaseCooldown()
    {
        return GenerateCooldown(StalkChaseCooldownRange);
    }

    public float GenerateStalkSpecialCooldown()
    {
        return GenerateCooldown(StalkSpecialCooldownRange);
    }

    public float GenerateChaseSpecialCooldown()
    {
        return GenerateCooldown(ChaseSpecialCooldownRange);
    }

    public float GenerateChaseStalkCooldown()
    {
        return GenerateCooldown(ChaseStalkCooldownRange);
    }
}
