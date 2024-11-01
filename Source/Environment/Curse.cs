using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour
{
    [SerializeField]
    private CurseState CurrentCurseState;
    [SerializeField]
    [Header("Curse Time Duration In Minutes")]
    private float CurseTimeDuration;

    [Space()]
    [SerializeField]
    private Color CurseFogColor;
    [SerializeField]
    private Light Sun;
    [SerializeField]
    private Light Flashlight;

    public delegate void OnCurseLerpChanged(CurseState currentState, float time);
    public static OnCurseLerpChanged CurseLerpChangedEvent;


    private void LerpEnvironment(Color skybox, Color fog, float fogDensity, Color sun, float time)
    {
        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), skybox, Time.deltaTime / time));
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fog, Time.deltaTime / time);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, Time.deltaTime / time);
        Sun.color = Color.Lerp(Sun.color, sun, Time.deltaTime / time);
        Flashlight.color = Color.Lerp(Flashlight.color, sun, Time.deltaTime / time);
        CurseLerpChangedEvent?.Invoke(CurrentCurseState, time);
    }

    private void ResetEnvironment()
    {
        RenderSettings.skybox.SetColor("_Tint", Color.black);
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.003f;
        Sun.color = Color.white;
        Sun.intensity = 0.1f;
    }

    private IEnumerator SetCurseTime()
    {
        float time = CurseTimeDuration;

        while (time > 0)
        {
            LerpEnvironment(Color.red, CurseFogColor, 0.1f, Color.red, time);
            time -= Time.deltaTime;
            yield return null;
        }

        CurrentCurseState = CurseState.Curse;
        StartCoroutine(SetGraceTime());
    }

    private IEnumerator SetGraceTime()
    {
        float time = CurseTimeDuration;

        while (time > 0)
        {
            LerpEnvironment(Color.black, Color.black, 0.003f, Color.white, time);
            time -= Time.deltaTime;
            yield return null;
        }

        CurrentCurseState = CurseState.Grace;
        StartCoroutine(SetCurseTime());
    }


    void Start()
    {
        CurseTimeDuration *= 60f;
        CurrentCurseState = CurseState.Grace;
        ResetEnvironment();
        StartCoroutine(SetCurseTime());
    }
}