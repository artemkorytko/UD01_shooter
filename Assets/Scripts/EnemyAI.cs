using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _minDotProduct = 0f;
    [SerializeField] private float _minDistanceToTarget = 2f;
    [SerializeField] private float _maxDistanceToTarget = 10f;

    private Transform _targetEntity = null;
    private MoveData _moveData = null;
    private LayerMask _entityLayerMask = 0;

    public MoveData GeMoveData()
    {
        return _moveData;
    }

    public void Init()
    {
        _moveData = new MoveData();
        _entityLayerMask = LayerMask.GetMask("Entity");
    }

    private void Update()
    {
        if (_targetEntity == null) return;

        if (IsTargetInViewZone(_targetEntity))
        {
            _moveData.LookDirection = GetTargetLookDirection(_targetEntity);
            _moveData.MoveDirection = GetMoveDirection(_targetEntity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            _targetEntity = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            _targetEntity = null;
        }
    }

    private bool IsTargetInViewZone(Transform target)
    {
        bool result = false;
        Vector3 direction = (target.position - transform.position).normalized;

        if (Vector3.Dot(transform.forward, direction) > _minDotProduct)
        {
            result = true;
        }

        Ray ray = new Ray(transform.position + Vector3.up, direction);

        if (result && Physics.Raycast(ray, out RaycastHit raycastHit, 50f, _entityLayerMask))
        {
            if (raycastHit.collider.gameObject.GetComponent<PlayerController>() == null)
            {
                result = false;
            }
        }

        return result;
    }

    private Vector3 GetTargetLookDirection(Transform target)
    {
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.Normalize();

        return lookDirection;
    }

    private Vector3 GetMoveDirection(Transform target)
    {
        float distance = Vector3.Distance(target.position, transform.position);
        Vector3 moveDirection = (target.position - transform.position).normalized;
        moveDirection.y = 0f;

        if (distance < _minDistanceToTarget)
        {
            moveDirection *= -1f;
        }

        if (distance > _minDistanceToTarget && distance < _maxDistanceToTarget)
        {
            moveDirection = Vector3.zero;
        }

        return moveDirection.normalized;
    }
}

[System.Serializable]
public class MoveData
{
    public Vector3 LookDirection;
    public Vector3 MoveDirection;
}
