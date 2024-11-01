using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    private bool IsVisible
    {
        get
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.GetWorldPosition()) > 30f)
            {
                return false;
            }

            var planes = GeometryUtility.CalculateFrustumPlanes(PlayerController.Instance.GetCamera());
            var point = transform.position;

            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public delegate void OnEnemySetVisible(bool visible);
    public static event OnEnemySetVisible EnemySetVisibleEvent;

    private void Update()
    {
        EnemySetVisibleEvent?.Invoke(IsVisible);
    }
}
