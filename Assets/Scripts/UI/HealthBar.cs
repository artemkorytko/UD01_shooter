using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _image = null;
    [SerializeField] private GameManager _gameManager = null;

    private void OnEnable()
    {
        _image.fillAmount = 1f;
    }

    private void Update()
    {
        if (_gameManager == null) return;

        _image.fillAmount = _gameManager.PlayerHealth;
    }
}
