using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int health;
    
    [Header("Player Movement")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpVelocity = 10f;

    [Space(10)]
    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    
    
    [Header("Aim Movement")]
    [SerializeField] private float moveSpeedAimed = 2;
    [SerializeField] private float rotationSpeedAimed = 10;
    [SerializeField] private Transform aimTrack;
    [SerializeField] private float maxAimHeight;
    [SerializeField] private float minAimHeight;

    public event Action OnJumpEvent;
    public event Action<PlayerState> OnStateUpdated;
    
    [Header("Camera")]
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _camForward;
    private Vector3 _camRight;
    private Vector3 _moveDirection;
    private CharacterController _characterController;
    
    private Quaternion _targetRotation;
    private Vector3 _velocity;
    private bool _isGrounded;

    private Vector3 _defaultAimTrackerPosition;
    private Vector3 _tempAimTrackerPosition;
    
    private PlayerState _currentState;

    public static event Action OnPlayerDied;
    public static event Action<int> OnHealthChanged; 
    

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    public Vector3 GetPlayerVelocity()
    {
        return _velocity;
    }

    void OnEnable()
    {
        MeleeEnemyController.CausedDamage += TakeDamage;
    }
    
    void OnDisable()
    {
        MeleeEnemyController.CausedDamage -= TakeDamage;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _currentState = PlayerState.EXPLORE; // setting the player's default state
        OnStateUpdated?.Invoke(_currentState);

        _defaultAimTrackerPosition = aimTrack.localPosition;
        
        OnHealthChanged?.Invoke(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentState == PlayerState.EXPLORE)
        {
             CalculateMovementExplore();
        }
        else if (_currentState == PlayerState.AIM)
        {
            CalculateMovementAim();
            UpdateAimTrack();
        }
       
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -0.2f;
        }
    }
    
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
    
    
    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if(_isGrounded)
        {
            _velocity.y = jumpVelocity;
            OnJumpEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //take damage from the melee(slime) enemy when they hit the player
        if (other.TryGetComponent<MeleeEnemyController>(out MeleeEnemyController enemy))
        {
            TakeDamage(enemy.damage);
        }
    }

    public void OnAim(InputValue value)
    {
        //setting the state based on whether or not "shift" or Right mouse is pressed
        _currentState = value.isPressed ? PlayerState.AIM : PlayerState.EXPLORE;
        OnStateUpdated?.Invoke(_currentState);

        //rotate the player to the camera direction when entering aim state
        if (_currentState == PlayerState.AIM)
        {
            _camForward = playerCamera.transform.forward;
            _camForward.y = 0;
            _camForward.Normalize();
            transform.rotation = Quaternion.LookRotation(_camForward);

            OnStateUpdated?.Invoke(_currentState);
        }
    }

    private void CalculateMovementExplore()
    {
        _camForward = playerCamera.transform.forward;
        _camRight = playerCamera.transform.right;
        _camForward.y = 0;
        _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();

        _moveDirection = _moveInput.x * _camRight + _moveInput.y * _camForward;

        if(_moveDirection.sqrMagnitude > 0.01f)
        {
            _targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        //Calculate gravity
        _velocity = Vector3.up * _velocity.y + _moveDirection * moveSpeed;
        _velocity.y += gravity * Time.deltaTime;
    }

    private void CalculateMovementAim()
    {
        //rotate the player around the Y axis based on horizontal input
        transform.Rotate(Vector3.up, rotationSpeed * _lookInput.x * Time.deltaTime);
        
        //WASD relates to where player is currently facing for strafe
        _moveDirection =  _moveInput.x * transform.right + _moveInput.y * transform.forward;

        _velocity = _velocity.y * Vector3.up + moveSpeedAimed * _moveDirection;
        _velocity.y += gravity * Time.deltaTime;
    }

    private float _tempAimTrackY;

    private void UpdateAimTrack()
    {
        _tempAimTrackerPosition = aimTrack.localPosition;
        _tempAimTrackerPosition.y += _lookInput.y * rotationSpeedAimed * Time.deltaTime;
        _tempAimTrackerPosition.y = Mathf.Clamp(_tempAimTrackerPosition.y, minAimHeight, maxAimHeight);
        aimTrack.localPosition = _tempAimTrackerPosition;
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.SphereCast(
            transform.position + groundCheckOffset,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged?.Invoke(health);
        
        if (health <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance/2, 
                    new Vector3(1.5f* groundCheckRadius, groundCheckDistance , 1.5f * groundCheckRadius) );
    }
}