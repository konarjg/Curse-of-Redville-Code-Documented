//Biblioteka z funkcjami g��wnymi C#, odpowiednik iostream, cstdlib itp
using System;
//Biblioteka ze strukturami danych, odpowiednik STL
using System.Collections;
//Biblioteka ze strukturami danych generycznymi, odpowiednik STL
using System.Collections.Generic;
//Biblioteka z funkcjami Unity
using UnityEngine;

//Oznaczenie nieskryptowej klasy, ale wci�� widocznej w edytorze Unity
[Serializable]
public class InputAction
{
    //Pole prywatne widoczne w edytorze Unity
    [SerializeField]
    private string Name; //Nazwa akcji
    [SerializeField]
    private KeyCode Binding; //Przypisany klawisz
    [SerializeField]
    private bool JustPress; //Czy nale�y wcisn�� czy przytrzyma� klawisz
    
    //Zwraca nazw� akcji
    public string GetName()
    {
        return Name;
    }

    //Sprawdza czy akcja zosta�a wywo�ana
    public bool IsTriggered()
    {
        //Staram si� nie u�ywa� if�w tam gdzie to mo�liwe dla poprawy czytelno�ci kodu
        //Zasada clean code > short code
        switch (JustPress)
        {
            //Je�li JustPress == true to sprawd� czy klawisz zosta� wci�ni�ty
            case true:
                return Input.GetKeyDown(Binding);

            //W przeciwnym wypadku sprawd� czy zosta� przytrzymany
            case false:
                return Input.GetKey(Binding);
        }
    }
}
