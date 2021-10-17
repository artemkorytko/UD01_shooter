using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxBulletsCount = 10;
    [SerializeField] private float _reloadTimeToBullet = 0.5f;
    [SerializeField] private float _changeStateTime = 0.5f;
    [SerializeField] private float _shootDelay = 0.2f;
    [SerializeField] private float _throwForce = 10f;

    [Header("References")]
    [SerializeField] private BulletPool _pool = null;
    [SerializeField] private Transform _normalState = null;
    [SerializeField] private Transform _reloadingState = null;
    [SerializeField] private Transform _startBulletPoint = null;
    [SerializeField] private GameObject _ownerObject = null;
    [SerializeField] private ParticleSystem _particleSystem = null;

    private int _currentBulletsCount = 0;
    private Coroutine _changeSateCoroutine = null;
    private Coroutine _reloadingCoroutine = null;
    private float _timer = 0f;

    public void Init(BulletPool pool)
    {
        _currentBulletsCount = _maxBulletsCount;
        _pool = pool;
    }

    public void Shoot()
    {
        if (_currentBulletsCount > 0 && _reloadingCoroutine == null && CanShoot(Time.deltaTime))
        {
            Bullet bullet = _pool.GetObject();
            bullet.transform.position = _startBulletPoint.position;
            bullet.Init(_pool, transform.forward, _ownerObject);

            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                _particleSystem.Play();
            }

            _currentBulletsCount--;
            _timer = _shootDelay;
        }

        if (_currentBulletsCount == 0 && _reloadingCoroutine == null)
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
            }

            _changeSateCoroutine = StartCoroutine(ChangingState(_reloadingState));
            _reloadingCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    public void SetUnuse()
    {
        transform.SetParent(_ownerObject.transform);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(_throwForce * transform.forward, ForceMode.Force);
    }

    private bool CanShoot(float time)
    {
        if (_timer > 0f)
        {
            _timer -= time;
            return false;
        }

        return true;
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitWhile(() => _changeSateCoroutine != null);

        for (int i = _currentBulletsCount; i < _maxBulletsCount; i++)
        {
            yield return new WaitForSeconds(_reloadTimeToBullet);
            _currentBulletsCount++;
        }

        _changeSateCoroutine = StartCoroutine(ChangingState(_normalState));
        yield  return new WaitWhile(() => _changeSateCoroutine != null);

        _reloadingCoroutine = null;
    }

    private IEnumerator ChangingState(Transform stateTransform)
    {
        transform.DOLocalMove(stateTransform.localPosition, _changeStateTime);
        transform.DOLocalRotateQuaternion(stateTransform.localRotation, _changeStateTime);

        yield return new WaitForSeconds(_changeStateTime);

        _changeSateCoroutine = null;
    }
}
