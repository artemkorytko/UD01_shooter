using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 0f;
    [SerializeField] private float _damage = 0f;

    private Pool<Bullet> _pool = null;
    private Vector3 _direction = Vector3.zero;
    private bool IsActive = false;
    private GameObject _owner = null;

    public void Init(Pool<Bullet> pool, Vector3 direction, GameObject owner)
    {
        _pool = pool;
        _owner = owner;
        _direction = direction;
        IsActive = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!IsActive) return;

        transform.Translate(_speed * Time.deltaTime * _direction, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();

        if (health != null && collision.collider.gameObject != _owner)
        {
            health.Decrease(_damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _pool.Return(this);
        IsActive = false;
        gameObject.SetActive(false);
    }
}
