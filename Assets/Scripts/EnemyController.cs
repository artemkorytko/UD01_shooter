using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string _groundLayer = "Ground";
    [SerializeField] private float _healthOnStart = 100f;
    [SerializeField] private float _deadHeight = -30f;
    [SerializeField] private float _dotProductForShoot = 0.9f;
    [SerializeField] private bool _useAutoInitialization = false;

    [Header("Movement settings")]
    [SerializeField] private float _forwardSpeed = 0f;
    [SerializeField] private float _rotationSpeed = 0f;
    [SerializeField] private float _minAngle = 0f;
    [SerializeField] private float _distanceToGround = 0.1f;

    [Header("References")]
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Weapon _weapon = null;
    [SerializeField] private EnemyAI _ai = null;
    [SerializeField] private ParticleSystem _explosionParticleSystem = null;
    [SerializeField] private BulletPool _pool = null;

    private Vector2 _cameraRotationXBorders = new Vector2(-89f, 89f);
    private CharacterController _controller = null;
    private Health _health = null;
    private bool _isActive = false;
    private LayerMask _groundLayerMask = 0;
    private MoveData _moveData = null;
    private float _rotationForAiming = 90f;

    public Action<EnemyController> OnDie = null;

    public bool IsActive
    {
        get
        {
            return _isActive;
        }

        set
        {
            _isActive = value;
        }
    }

    public bool IsAlive { get; private set; }

    private void Awake()
    {
        if (_useAutoInitialization)
        {
            IsActive = true;
            Init(_pool);
        }
    }

    public void Init(BulletPool pool)
    {
        IsAlive = true;
        
        _controller = GetComponent<CharacterController>();
        _health = GetComponent<Health>();

        _health.SetMaxValue(_healthOnStart);
        _health.SetCurrentValue(_healthOnStart);
        _health.OnDie += Dead;
        if (_weapon != null) _weapon.Init(pool);
        _ai.Init();

        _groundLayerMask = LayerMask.GetMask(_groundLayer);
    }

    private void Update()
    {
        if (!_isActive) return;

        UpdateMoveData();
        UpdateWeapon();
        UpdateMovement();
        UpdateLook();
        CheckGround();
    }

    private void UpdateMoveData()
    {
        _moveData = _ai.GeMoveData();
    }

    private void UpdateWeapon()
    {
        if (Vector3.Dot(transform.forward, _moveData.LookDirection) >= _dotProductForShoot)
        {
            if (_weapon != null) _weapon.Shoot();
        }
    }

    private void UpdateMovement()
    {
        if (_moveData.MoveDirection == Vector3.zero) return;

        _controller.Move(_forwardSpeed * Time.deltaTime * _moveData.MoveDirection);
    }

    private void UpdateLook()
    {
        if (_moveData.LookDirection == Vector3.zero) return;

        Vector3 rhs = _moveData.LookDirection;
        rhs.y = 0f;
        rhs.Normalize();

        float rotationCoefY = 0f;
        float angle = Vector3.Angle(transform.forward, rhs);

        if (angle >= _minAngle)
        {
            Vector3 crossY = Vector3.Cross(_camera.forward, rhs);
            rotationCoefY = (crossY.y > 0f ? 1f : -1f);
        }

        transform.Rotate(rotationCoefY * _rotationSpeed * Time.deltaTime * Vector3.up, Space.Self);

        if (angle <= _rotationForAiming)
        {
            rhs = _camera.InverseTransformDirection(_moveData.LookDirection);
            rhs.x = 0f;
            rhs.Normalize();

            float rotationCoeffX = 0f;

            if (Vector3.Angle(Vector3.forward, rhs) >= _minAngle)
            {
                Vector3 crossX = Vector3.Cross(Vector3.forward, rhs);
                rotationCoeffX = (crossX.x > 0f ? 1f : -1f);
            }

            Vector3 rotation = _camera.localRotation.eulerAngles;
            rotation.x = rotation.x > 180f ? rotation.x - 360f : rotation.x;
            rotation.x += rotationCoeffX * _rotationSpeed * Time.deltaTime;
            rotation.x = Mathf.Clamp(rotation.x, _cameraRotationXBorders.x, _cameraRotationXBorders.y);
            _camera.transform.localRotation = Quaternion.Euler(rotation);
        }

        Debug.DrawLine(_camera.position, _camera.position + _camera.forward * 2f, Color.green);
    }

    private void CheckGround()
    {
        Vector3 bottomHemisphere = transform.position + transform.up * _controller.radius;
        Vector3 topHemisphere = transform.position + transform.up * (_controller.height - _controller.radius);

        if (Physics.CapsuleCast(bottomHemisphere, topHemisphere, _controller.radius, Vector3.down, out RaycastHit hit,
        _distanceToGround + _controller.skinWidth, _groundLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance > _controller.skinWidth)
            {
                _controller.Move(Vector3.down * hit.distance);
            }
        }
        else
        {
            _controller.Move(Physics.gravity.y * Time.deltaTime * Vector3.up);
            CheckDeadFall();
        }
    }

    private void CheckDeadFall()
    {
        if (transform.position.y <= _deadHeight)
        {
            Dead();
        }
    }

    private void Dead()
    {
        _isActive = false;
        IsAlive = false;
        _health.OnDie -= Dead;
        _controller.enabled = false;
        _camera.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        if (_weapon != null) _weapon.SetUnuse();

        gameObject.SetActive(false);
        if (_explosionParticleSystem != null)
        {
            _explosionParticleSystem.transform.SetParent(transform.parent);
            _explosionParticleSystem.Play();
        }

        OnDie?.Invoke(this);
    }
}
