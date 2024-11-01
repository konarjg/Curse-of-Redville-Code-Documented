using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform Target; //Na co ma spogl�da� kamera
    [SerializeField]
    private bool LockCursor; //Czy blokowa� i chowa� kursor myszy?
    [SerializeField]
    private Vector2 VerticalConstraints; //G�rne ograniczenie obracania

    private bool Paused; //Czy ruch kamer� jest wy��czony?
    private float Sensitivity; //Czu�o�� obracania

    //Prze��cz wy��czenie kamery i stan kursora
    public void TogglePause()
    {
        Paused = !Paused;
        LockCursor = !Paused;
    }

    //Zwr�� obiekt, na kt�ry spogl�da kamera
    public Transform GetTarget()
    {
        return Target;
    }

    //Wymuszone spojrzenie na obiekt o podanej pozycji pod podanym k�tem
    public void LookAt(Vector3 position, float tilt = 0f)
    {
        //Sp�jrz na pozycj�
        transform.LookAt(position);
        //Ustaw rotacj� obiektu na (-tilt, aktualna rotacja y, 0)
        transform.eulerAngles = new Vector3(-tilt, transform.eulerAngles.y, 0f);
        //Ustaw rotacj� celu na (0, aktualna rotacja y, 0) by nie zepsu� ruchu gracza
        Target.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    //Metoda Start uruchamiana zaraz po Awake przy tworzeniu obiektu w Unity
    private void Start()
    {
        //Z InputManagera odczytaj warto�� czu�o�ci myszy
        Sensitivity = InputManager.Instance.GetMouseSensitivity();
    }

    //Wymuszenie g�rnych ogranicze� rotacji
    //ref -> podanie przez zwyk�� referencj�
    //in -> podanie przez referencj� z wymuszon� zewn�trzn� zmian� warto�ci
    //out -> podanie przez referencj� z wymuszon� wewn�trzn� zmian� warto�ci
    private void ClampRotation(ref Vector3 targetRotation)
    {
        //K�ty Eulera s� w granicach (0, 360),
        //zamieniamy je na przedzia� (-180, 180)
        //convertedX = 
        /*{
              x_euler > 180: x_euler - 360
              else: x_euler
        }*/

        var convertedX = targetRotation.x;

        if (convertedX > 180f)
            convertedX -= 360f;
        
        //Wymuszamy ograniczenia g�rne i dolne
        if (convertedX < VerticalConstraints.x)
        {
            targetRotation.x = VerticalConstraints.x;
        }
        else if (convertedX > VerticalConstraints.y)
        {
            targetRotation.x = VerticalConstraints.y;
        }
    }

    //Obracanie kamery o zadane k�ty
    private void Rotate(Vector2 input)
    {
        //Wyliczamy poprawny krok obrotu w tej klatce
        //Time.deltaTime to czas, kt�ry up�yn�� od ostatniej klatki
        //(ZMIENNY W METODZIE UPDATE I LATEUPDATE, STA�Y W METODZIE FIXEDUPDATE)
        var step = new Vector3(-input.y, input.x, 0) * Sensitivity * Time.deltaTime;
        
        //Wyliczenie kroku pod��ania za celem w tej klatce
        var followStep = new Vector3(0, input.x, 0) * Sensitivity * Time.deltaTime;

        //Docelowa rotacja
        var targetRotation = transform.eulerAngles + step;
        //Docelowa rotacja pod��ania za celem
        var followTargetRotation = Target.eulerAngles + followStep;

        //Wymuszenie ogranicze�
        ClampRotation(ref targetRotation);

        //W�a�ciwy obr�t poprzez liniow� interpolacj� (smooth rotation)
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 10);
        Target.eulerAngles = Vector3.Lerp(Target.eulerAngles, followTargetRotation, 10);
    }

    //Zaktualizowanie stanu kursora, raczej zrozumia�e
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

    //Odczytanie ruchu myszk�
    private void ReadInput()
    {
        //Osie X i Y wektora ruchu myszk� (warto�ci od 0 do 1)
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        //Obracamy kamer� o odpowiedni wektor
        Rotate(new Vector2(inputX, inputY));
    }

    //��czymy wszystko w ca�o�� co klatk�
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
