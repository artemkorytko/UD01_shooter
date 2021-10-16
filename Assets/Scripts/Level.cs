using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _player = null;
    [SerializeField] private List<EnemyController> _enemies = null;

    private int _aliveEnemiesCount = 0;

    public float PlayerHealth => _player.HealthValue;
    public Action<EGameResult> OnGameEnded = null;

    public void StartLevel(BulletPool pool, bool init = false)
    {
        if (init) InitEntities(pool);

        Subcribe();

        _player.IsActive = true;

        _aliveEnemiesCount = _enemies.Count;
        SetEnemiesState(true);
    }

    public void StopGame()
    {
        Unsubscribe();

        _player.IsActive = false;
        SetEnemiesState(false);
    }

    private void InitEntities(BulletPool pool)
    {
        _player.Init(pool);

        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].Init(pool);
        }
    }

    private void Subcribe()
    {
        _player.OnDie += OnPlayerDie;

        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].OnDie += OnEnemyDie;
        }
    }

    private void Unsubscribe()
    {
        if (_player.IsAlive) _player.OnDie -= OnPlayerDie;

        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].IsAlive) _enemies[i].OnDie += OnEnemyDie;
        }
    }

    private void OnPlayerDie()
    {
        OnGameEnded?.Invoke(EGameResult.Fail);

        Unsubscribe();
    }

    private void OnEnemyDie(EnemyController enemy)
    {
        enemy.OnDie -= OnEnemyDie;

        _aliveEnemiesCount--;

        if (_aliveEnemiesCount == 0)
        {
            OnGameEnded?.Invoke(EGameResult.Win);
        }

        Unsubscribe();
    }

    private void SetEnemiesState(bool state)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].IsAlive)
            {
                _enemies[i].IsActive = state;
            }
        }
    }
}
