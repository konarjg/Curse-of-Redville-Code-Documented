using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float MouseSensitivity;
    [SerializeField]
    private List<InputAction> Actions = new();

    public static InputManager Instance;

    private void Awake()
    {
        Instance = this; 
    }

    public float GetMouseSensitivity()
    {
        return MouseSensitivity;
    }

    public InputAction GetAction(string name)
    {
        return Actions.Find(action => action.GetName() == name);
    }
}
