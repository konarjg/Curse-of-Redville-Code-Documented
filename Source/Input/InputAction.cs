//Biblioteka z funkcjami g≥Ûwnymi C#, odpowiednik iostream, cstdlib itp
using System;
//Biblioteka ze strukturami danych, odpowiednik STL
using System.Collections;
//Biblioteka ze strukturami danych generycznymi, odpowiednik STL
using System.Collections.Generic;
//Biblioteka z funkcjami Unity
using UnityEngine;

//Oznaczenie nieskryptowej klasy, ale wciπø widocznej w edytorze Unity
[Serializable]
public class InputAction
{
    //Pole prywatne widoczne w edytorze Unity
    [SerializeField]
    private string Name; //Nazwa akcji
    [SerializeField]
    private KeyCode Binding; //Przypisany klawisz
    [SerializeField]
    private bool JustPress; //Czy naleøy wcisnπÊ czy przytrzymaÊ klawisz
    
    //Zwraca nazwÍ akcji
    public string GetName()
    {
        return Name;
    }

    //Sprawdza czy akcja zosta≥a wywo≥ana
    public bool IsTriggered()
    {
        //Staram siÍ nie uøywaÊ ifÛw tam gdzie to moøliwe dla poprawy czytelnoúci kodu
        //Zasada clean code > short code
        switch (JustPress)
        {
            //Jeúli JustPress == true to sprawdü czy klawisz zosta≥ wciúniÍty
            case true:
                return Input.GetKeyDown(Binding);

            //W przeciwnym wypadku sprawdü czy zosta≥ przytrzymany
            case false:
                return Input.GetKey(Binding);
        }
    }
}
