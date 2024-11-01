using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dziedziczymy po MonoBehaviour, by oznaczy� klas� jako skrypt Unity
public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float MouseSensitivity; //Czu�o�� myszy
    [SerializeField]
    private List<InputAction> Actions = new(); //Lista przypisa� klawiszy do akcji

    //Instancja InputManagera w �wiecie gry dost�pna globalnie dla innych skrypt�w
    public static InputManager Instance;

    //Metoda uruchamiana przy tworzeniu obiektu w Unity (tu� przed metod� Start)
    private void Awake()
    {
        //Przypisujemy utworzony obiekt do globalnej instancji, by umo�liwi� dost�p
        Instance = this; 
    }

    //Zwraca czu�o�� myszy
    public float GetMouseSensitivity()
    {
        return MouseSensitivity;
    }

    //Zwraca akcj� o podanej nazwie z listy wszystkich akcji
    public InputAction GetAction(string name)
    {
        //Wyszukaj tak� akcj�, �e jej nazwa jest r�wna name
        //Odpowiednik SELECT * FROM Actions WHERE Name = name z SQLa
        return Actions.Find(action => action.GetName() == name);
    }
}
