using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour
{
    [SerializeField]
    private CurseState CurrentCurseState; //Stan Kl�twa/�aska
    [SerializeField]
    [Header("Curse Time Duration In Minutes")]
    private float CurseTimeDuration; //Czas trwania stanu Kl�twy/�aski

    [Space()]
    [SerializeField]
    private Color CurseFogColor;  //Kolor mg�y podczas Kl�twy
    [SerializeField]
    private Light Sun; //G��wne �r�d�o �wiat�a (bardzo ciemne, by doda� klimat)
    [SerializeField]
    private Light Flashlight; //Latarka gracza

    //Zdarzenie uruchamiane, gdy liniowa interpolacja �rodowiska ulegnie zmianie
    public delegate void OnCurseLerpChanged(CurseState currentState, float time);
    public static OnCurseLerpChanged CurseLerpChangedEvent;


    //Interpolacja liniowa koloru nieba, mg�y i �wiat�a w zadanym czasie
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

    //Dwie metody poni�ej wywo�uj� same siebie nawzajem, powoli interpoluj� kolor �rodowiska na kolor aktualne stanu
    //A nast�pnie zmieniaj� stan na zmian� po up�ywie odpowiedniego czasu
    private IEnumerator SetCurseTime()
    {
        float time = CurseTimeDuration;

        //P�ki czas nie jest zerem odejmuj czas od ostatniej klatki i kontynuuj wykonanie metody rekurencyjnie
        //To taki trick, �eby zrobi� dodatkowy osobny void Update w skrypcie
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

        //P�ki czas nie jest zerem odejmuj czas od ostatniej klatki i kontynuuj wykonanie metody rekurencyjnie
        //To taki trick, �eby zrobi� dodatkowy osobny void Update w skrypcie
        while (time > 0)
        {
            LerpEnvironment(Color.black, Color.black, 0.003f, Color.white, time);
            time -= Time.deltaTime;
            yield return null;
        }

        CurrentCurseState = CurseState.Grace;

        //Wywo�anie asynchronicznej ko-rutyny
        //Asynchroniczna ko-rutyna -> zestaw instrukcji, kt�re maj� wykonywa� si� zadan� liczb� razy r�wnolegle z reszt� programu
        //nie zak��caj�c przebiegu programu
        StartCoroutine(SetCurseTime());
    }


    //Inicjalizacja
    void Start()
    {
        //Konwersja na sekundy
        CurseTimeDuration *= 60f;
        CurrentCurseState = CurseState.Grace;
        ResetEnvironment();

        //Wywo�anie asynchronicznej ko-rutyny
        //Asynchroniczna ko-rutyna -> zestaw instrukcji, kt�re maj� wykonywa� si� zadan� liczb� razy r�wnolegle z reszt� programu
        //nie zak��caj�c przebiegu programu
        StartCoroutine(SetCurseTime());
    }
}