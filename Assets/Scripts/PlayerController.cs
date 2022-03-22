using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string _groundLayer = "Ground";
    [SerializeField] private float _healthOnStart = 100f;
    [SerializeField] private float _deadHeight = -30f;
    [SerializeField] private bool _useAutoInitialization = false;

    [Header("Movement settings")]
    [SerializeField] private float _forwardSpeed = 0f;
    [SerializeField] private float _rotationSpeed = 0f;
    [SerializeField] private float _distanceToGround = 0.1f;

    [Header("References")]
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Weapon _weapon = null;
    [SerializeField] private BulletPool _pool = null;

    private Vector2 _cameraRotationXBorders = new Vector2(-60f, 60f);
    private CharacterController _controller = null; 
    private PlayerInputHandler _inputHandler = null;
    private Health _health = null;
    private bool _isActive = false;
    private LayerMask _groundLayerMask = 0;

    public float HealthValue => _health.CurrentValueNormalized;
    public Action OnDie = null;

    public bool IsActive 
    {
        get
        {
            return _isActive;
        }

        set
        {
            _isActive = value;
            _inputHandler.CanHandling = _isActive;
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
        _inputHandler = GetComponent<PlayerInputHandler>();
        _health = GetComponent<Health>();

        _health.SetMaxValue(_healthOnStart);
        _health.SetCurrentValue(_healthOnStart);
        _health.OnDie += Dead;
        if (_weapon != null) _weapon.Init(pool);

        _groundLayerMask = LayerMask.GetMask(_groundLayer);
    }

    private void Update()
    {
        if (!_isActive) return;

        UpdateWeapon();
        UpdateMovement();
        UpdateLook();
        CheckGround();
    }

    private void UpdateWeapon()
    {
        if (!_inputHandler.IsFireHold()) return;

        if (_weapon != null) _weapon.Shoot();
    }

    private void UpdateMovement()
    {
        Vector3 movementDirection = transform.TransformDirection(_inputHandler.GetMovementVector());

        _controller.Move(_forwardSpeed * Time.deltaTime * movementDirection);
    }

    private void UpdateLook()
    {
        Vector3 rotation = _camera.localRotation.eulerAngles;
        rotation.x = rotation.x > 180f ? rotation.x - 360f : rotation.x;
        rotation.x -= _inputHandler.GetLookVertical() * _rotationSpeed;
        rotation.x = Mathf.Clamp(rotation.x, _cameraRotationXBorders.x, _cameraRotationXBorders.y);
        _camera.transform.localRotation = Quaternion.Euler(rotation);
        
        transform.Rotate(_inputHandler.GetLookHorizontal() * _rotationSpeed * Vector3.up, Space.Self);
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

        OnDie?.Invoke();
    }
}
