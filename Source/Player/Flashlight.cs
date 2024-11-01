using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private List<float> FlickerCooldowns;
    [SerializeField]
    private List<float> FlickerTimes;
    [SerializeField]
    private float FlickerCooldown;
    [SerializeField]
    private float FlickerTime;

    private Light Light;
    private bool IsFlickering;


    private void InitTimes()
    {
        FlickerCooldown = FlickerCooldowns.OrderBy(key => Random.Range(int.MinValue, int.MaxValue)).ToList()[0];
        FlickerTime = FlickerTimes.OrderBy(key => Random.Range(int.MinValue, int.MaxValue)).ToList()[0];
    }

    private void Start()
    {
        Light = GetComponent<Light>();
        IsFlickering = false;
        InitTimes();
    }

    private void Flicker()
    {
        if (FlickerTime > 0f)
        {
            Light.enabled = false;
            FlickerTime -= Time.deltaTime;
            return;
        }

        IsFlickering = false;
        InitTimes();
    }

    private void Cooldown()
    {
        if (FlickerCooldown > 0f)
        {
            Light.enabled = true;
            FlickerCooldown -= Time.deltaTime;
            return;
        }

        IsFlickering = true;
    }


    private void Update()
    {
        switch (IsFlickering)
        {
            case true:
                Flicker();
                break;

            case false:
                Cooldown();
                break;
        }
    }
}
