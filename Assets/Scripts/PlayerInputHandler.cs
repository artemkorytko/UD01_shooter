using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float _lookSensitivity = 1f;
    
    private bool _isActive = false;
    public bool CanHandling 
    {
        get
        {
            return _isActive;
        }

        set
        {
            _isActive = value;
            Cursor.lockState = _isActive ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public bool IsFireHold()
    {
        return Input.GetButton(Constants.FireName) || Input.GetButtonDown(Constants.FireName);
    }

    public Vector3 GetMovementVector()
    {
        Vector3 direction = new Vector3(Input.GetAxis(Constants.HorizontalAxisName), 0f, Input.GetAxis(Constants.VerticalAxisName));
        return direction.normalized;
    }

    public float GetLookHorizontal()
    {
        return Input.GetAxis(Constants.LookHorizontalAxisName) * _lookSensitivity;
    }

    public float GetLookVertical()
    {
        return Mathf.Clamp(Input.GetAxis(Constants.LookVerticalAxisName) * _lookSensitivity, -1f, 1f);
    }
}
