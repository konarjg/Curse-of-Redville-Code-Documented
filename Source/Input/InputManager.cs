using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dziedziczymy po MonoBehaviour, by oznaczyæ klasê jako skrypt Unity
public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float MouseSensitivity; //Czu³oœæ myszy
    [SerializeField]
    private List<InputAction> Actions = new(); //Lista przypisañ klawiszy do akcji

    //Instancja InputManagera w œwiecie gry dostêpna globalnie dla innych skryptów
    public static InputManager Instance;

    //Metoda uruchamiana przy tworzeniu obiektu w Unity (tu¿ przed metod¹ Start)
    private void Awake()
    {
        //Przypisujemy utworzony obiekt do globalnej instancji, by umo¿liwiæ dostêp
        Instance = this; 
    }

    //Zwraca czu³oœæ myszy
    public float GetMouseSensitivity()
    {
        return MouseSensitivity;
    }

    //Zwraca akcjê o podanej nazwie z listy wszystkich akcji
    public InputAction GetAction(string name)
    {
        //Wyszukaj tak¹ akcjê, ¿e jej nazwa jest równa name
        //Odpowiednik SELECT * FROM Actions WHERE Name = name z SQLa
        return Actions.Find(action => action.GetName() == name);
    }
}
