using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private Light[] _lights;

    private bool _flashStatus = true;

    public void FlashStatusChange()
    {
        if(_flashStatus)
            DisableFlashLight();
        else
            EnableFlashLight();
    }
    
    private void EnableFlashLight()
    {
        foreach (var light in _lights)
        {
            light.gameObject.SetActive(true);
        }
    }

    private void DisableFlashLight()
    {
        foreach (var light in _lights)
        {
            light.gameObject.SetActive(false);
        }
    }
}
