using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform Target; //Na co ma spogl¹daæ kamera
    [SerializeField]
    private bool LockCursor; //Czy blokowaæ i chowaæ kursor myszy?
    [SerializeField]
    private Vector2 VerticalConstraints; //Górne ograniczenie obracania

    private bool Paused; //Czy ruch kamer¹ jest wy³¹czony?
    private float Sensitivity; //Czu³oœæ obracania

    //Prze³¹cz wy³¹czenie kamery i stan kursora
    public void TogglePause()
    {
        Paused = !Paused;
        LockCursor = !Paused;
    }

    //Zwróæ obiekt, na który spogl¹da kamera
    public Transform GetTarget()
    {
        return Target;
    }

    //Wymuszone spojrzenie na obiekt o podanej pozycji pod podanym k¹tem
    public void LookAt(Vector3 position, float tilt = 0f)
    {
        //Spójrz na pozycjê
        transform.LookAt(position);
        //Ustaw rotacjê obiektu na (-tilt, aktualna rotacja y, 0)
        transform.eulerAngles = new Vector3(-tilt, transform.eulerAngles.y, 0f);
        //Ustaw rotacjê celu na (0, aktualna rotacja y, 0) by nie zepsuæ ruchu gracza
        Target.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    //Metoda Start uruchamiana zaraz po Awake przy tworzeniu obiektu w Unity
    private void Start()
    {
        //Z InputManagera odczytaj wartoœæ czu³oœci myszy
        Sensitivity = InputManager.Instance.GetMouseSensitivity();
    }

    //Wymuszenie górnych ograniczeñ rotacji
    //ref -> podanie przez zwyk³¹ referencjê
    //in -> podanie przez referencjê z wymuszon¹ zewnêtrzn¹ zmian¹ wartoœci
    //out -> podanie przez referencjê z wymuszon¹ wewnêtrzn¹ zmian¹ wartoœci
    private void ClampRotation(ref Vector3 targetRotation)
    {
        //K¹ty Eulera s¹ w granicach (0, 360),
        //zamieniamy je na przedzia³ (-180, 180)
        //convertedX = 
        /*{
              x_euler > 180: x_euler - 360
              else: x_euler
        }*/

        var convertedX = targetRotation.x;

        if (convertedX > 180f)
            convertedX -= 360f;
        
        //Wymuszamy ograniczenia górne i dolne
        if (convertedX < VerticalConstraints.x)
        {
            targetRotation.x = VerticalConstraints.x;
        }
        else if (convertedX > VerticalConstraints.y)
        {
            targetRotation.x = VerticalConstraints.y;
        }
    }

    //Obracanie kamery o zadane k¹ty
    private void Rotate(Vector2 input)
    {
        //Wyliczamy poprawny krok obrotu w tej klatce
        //Time.deltaTime to czas, który up³yn¹³ od ostatniej klatki
        //(ZMIENNY W METODZIE UPDATE I LATEUPDATE, STA£Y W METODZIE FIXEDUPDATE)
        var step = new Vector3(-input.y, input.x, 0) * Sensitivity * Time.deltaTime;
        
        //Wyliczenie kroku pod¹¿ania za celem w tej klatce
        var followStep = new Vector3(0, input.x, 0) * Sensitivity * Time.deltaTime;

        //Docelowa rotacja
        var targetRotation = transform.eulerAngles + step;
        //Docelowa rotacja pod¹¿ania za celem
        var followTargetRotation = Target.eulerAngles + followStep;

        //Wymuszenie ograniczeñ
        ClampRotation(ref targetRotation);

        //W³aœciwy obrót poprzez liniow¹ interpolacjê (smooth rotation)
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 10);
        Target.eulerAngles = Vector3.Lerp(Target.eulerAngles, followTargetRotation, 10);
    }

    //Zaktualizowanie stanu kursora, raczej zrozumia³e
    private void UpdateCursorState()
    {
        if (LockCursor == Cursor.visible)
        {
            switch (LockCursor)
            {
                case true:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;

                case false:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
    }

    //Odczytanie ruchu myszk¹
    private void ReadInput()
    {
        //Osie X i Y wektora ruchu myszk¹ (wartoœci od 0 do 1)
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        //Obracamy kamerê o odpowiedni wektor
        Rotate(new Vector2(inputX, inputY));
    }

    //£¹czymy wszystko w ca³oœæ co klatkê
    //LateUpdate idealne dla kamery
    private void LateUpdate()
    {
        UpdateCursorState();

        if (Paused)
        {
            return;
        }

        ReadInput();
    }
}
