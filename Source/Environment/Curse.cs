using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour
{
    [SerializeField]
    private CurseState CurrentCurseState; //Stan Kl¹twa/£aska
    [SerializeField]
    [Header("Curse Time Duration In Minutes")]
    private float CurseTimeDuration; //Czas trwania stanu Kl¹twy/£aski

    [Space()]
    [SerializeField]
    private Color CurseFogColor;  //Kolor mg³y podczas Kl¹twy
    [SerializeField]
    private Light Sun; //G³ówne Ÿród³o œwiat³a (bardzo ciemne, by dodaæ klimat)
    [SerializeField]
    private Light Flashlight; //Latarka gracza

    //Zdarzenie uruchamiane, gdy liniowa interpolacja œrodowiska ulegnie zmianie
    public delegate void OnCurseLerpChanged(CurseState currentState, float time);
    public static OnCurseLerpChanged CurseLerpChangedEvent;


    //Interpolacja liniowa koloru nieba, mg³y i œwiat³a w zadanym czasie
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

    //Dwie metody poni¿ej wywo³uj¹ same siebie nawzajem, powoli interpoluj¹ kolor œrodowiska na kolor aktualne stanu
    //A nastêpnie zmieniaj¹ stan na zmianê po up³ywie odpowiedniego czasu
    private IEnumerator SetCurseTime()
    {
        float time = CurseTimeDuration;

        //Póki czas nie jest zerem odejmuj czas od ostatniej klatki i kontynuuj wykonanie metody rekurencyjnie
        //To taki trick, ¿eby zrobiæ dodatkowy osobny void Update w skrypcie
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

        //Póki czas nie jest zerem odejmuj czas od ostatniej klatki i kontynuuj wykonanie metody rekurencyjnie
        //To taki trick, ¿eby zrobiæ dodatkowy osobny void Update w skrypcie
        while (time > 0)
        {
            LerpEnvironment(Color.black, Color.black, 0.003f, Color.white, time);
            time -= Time.deltaTime;
            yield return null;
        }

        CurrentCurseState = CurseState.Grace;

        //Wywo³anie asynchronicznej ko-rutyny
        //Asynchroniczna ko-rutyna -> zestaw instrukcji, które maj¹ wykonywaæ siê zadan¹ liczbê razy równolegle z reszt¹ programu
        //nie zak³ócaj¹c przebiegu programu
        StartCoroutine(SetCurseTime());
    }


    //Inicjalizacja
    void Start()
    {
        //Konwersja na sekundy
        CurseTimeDuration *= 60f;
        CurrentCurseState = CurseState.Grace;
        ResetEnvironment();

        //Wywo³anie asynchronicznej ko-rutyny
        //Asynchroniczna ko-rutyna -> zestaw instrukcji, które maj¹ wykonywaæ siê zadan¹ liczbê razy równolegle z reszt¹ programu
        //nie zak³ócaj¹c przebiegu programu
        StartCoroutine(SetCurseTime());
    }
}