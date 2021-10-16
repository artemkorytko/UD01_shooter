using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIPanel> _panels = null;
    [SerializeField] private EUIPanelType _initType = EUIPanelType.Main;

    private GameManager _gameManager = null;
    private UIPanel _currentPanel = null;

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        Subscribe();
        Show(_initType);
    }

    public void Show(EUIPanelType panelType)
    {
        if (_currentPanel != null)
        {
            _currentPanel.Hide();
            _currentPanel = null;
        }

        foreach(UIPanel panel in _panels)
        {
            if (panel.Type == panelType)
            {
                panel.Show();
                _currentPanel = panel;
            }
        }
    }

    private void Subscribe()
    {
        _gameManager.OnWin += OnWin;
        _gameManager.OnFail += OnFail;
    }

    private void OnWin()
    {
        Show(EUIPanelType.Win);
    }

    private void OnFail()
    {
        Show(EUIPanelType.Fail);
    }

    public void StartButton()
    {
        _gameManager.StartGame();
        Show(EUIPanelType.Game);
    }

    public void MainMenuButton()
    {
        Show(EUIPanelType.Main);
    }

    public void ExitButton()
    {
        Application.Quit(0);
    }
}
