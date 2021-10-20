using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private List<GameObject> _levels = null;

    [Header("References")]
    [SerializeField] private UIManager _uiManager = null;
    [SerializeField] private BulletPool _bulletPool = null;

    private int _levelNum = 0;
    private Level _currentLevel = null;

    public float PlayerHealth => _currentLevel.PlayerHealth; 
    public Action OnWin = null;
    public Action OnFail = null;

    private void Awake()
    {
        LoadPlayerData();
        _uiManager.Initialize(this);
    }

    public void StartGame()
    {
        InstantiateLevel(_levelNum, out _currentLevel);

        _uiManager.Show(EUIPanelType.Game);
        Subscribe();
        _currentLevel.StartLevel(_bulletPool, true);
    }

    public void StopGame()
    {
        Unsubscribe();

        _currentLevel.StopGame();
    }

    private void LoadPlayerData()
    {
        _levelNum = PlayerPrefs.GetInt("LevelNum", 0);
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("LevelNum", _levelNum);
    }

    private void InstantiateLevel(int index, out Level level)
    {
        if (index >= _levels.Count)
        {
            index = index % _levels.Count;
        }

        if (_currentLevel != null)
        {
            Destroy(_currentLevel.gameObject);
            _currentLevel = null;
        }

        level = Instantiate(_levels[index], transform).GetComponent<Level>();
    }

    private void Subscribe()
    {
        _currentLevel.OnGameEnded += OnGameEnded;
    }

    private void Unsubscribe()
    {
        _currentLevel.OnGameEnded -= OnGameEnded;
    }

    private void OnGameEnded(EGameResult result)
    {
        StopGame();

        if (result == EGameResult.Win)
        {
            PlayerWin();
        }
        else
        {
            PlayerFail();
        }
    }

    private void PlayerWin()
    {
        _levelNum++;
        SavePlayerData();
        _uiManager.Show(EUIPanelType.Win);
    }


    private  void PlayerFail()
    {
        _uiManager.Show(EUIPanelType.Fail);
    }

    public void Exit()
    {
        Application.Quit(0);
    }
}
