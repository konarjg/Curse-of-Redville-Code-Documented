using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputAction
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private KeyCode Binding;
    [SerializeField]
    private bool JustPress;
    
    public string GetName()
    {
        return Name;
    }

    public bool IsTriggered()
    {
        switch (JustPress)
        {
            case true:
                return Input.GetKeyDown(Binding);

            case false:
                return Input.GetKey(Binding);
        }
    }
}
