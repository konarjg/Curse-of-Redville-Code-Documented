using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notatka w dzienniku, kontroler w modelu Controller - View
public class Note : MonoBehaviour
{
    private string Name; //Nazwa notatki
    private string Contents; //Zawartoœæ notatki

    //Konstruktor
    public Note(string name, string contents)
    {
        Name = name;
        Contents = contents;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetContents()
    {
        return Contents;
    }
}
