using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private string Name;
    private string Contents;

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
