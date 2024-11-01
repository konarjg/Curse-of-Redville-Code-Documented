using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private bool LockCursor;
    [SerializeField]
    private Vector2 VerticalConstraints;

    private bool Paused;
    private float Sensitivity;

    public void TogglePause()
    {
        Paused = !Paused;
        LockCursor = !Paused;
    }

    public Transform GetTarget()
    {
        return Target;
    }

    public void LookAt(Vector3 position, float tilt = 0f)
    {
        transform.LookAt(position);
        transform.eulerAngles = new Vector3(-tilt, transform.eulerAngles.y, 0f);
        Target.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    private void Start()
    {
        Sensitivity = InputManager.Instance.GetMouseSensitivity();
    }

    private void ClampRotation(ref Vector3 targetRotation)
    {
        var convertedX = targetRotation.x;

        if (convertedX > 180f)
            convertedX -= 360f;

        if (convertedX < VerticalConstraints.x)
        {
            targetRotation.x = VerticalConstraints.x;
        }
        else if (convertedX > VerticalConstraints.y)
        {
            targetRotation.x = VerticalConstraints.y;
        }
    }

    private void Rotate(Vector2 input)
    {
        var step = new Vector3(-input.y, input.x, 0) * Sensitivity * Time.deltaTime;
        var followStep = new Vector3(0, input.x, 0) * Sensitivity * Time.deltaTime;

        var targetRotation = transform.eulerAngles + step;
        var followTargetRotation = Target.eulerAngles + followStep;

        ClampRotation(ref targetRotation);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 10);
        Target.eulerAngles = Vector3.Lerp(Target.eulerAngles, followTargetRotation, 10);
    }

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

    private void ReadInput()
    {
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        Rotate(new Vector2(inputX, inputY));
    }

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
