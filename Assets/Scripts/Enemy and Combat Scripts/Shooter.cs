using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("Shooter Settings")]
    [SerializeField] private InputAction shootInput; // setting the input that allows player to shoot
    [SerializeField] private Transform shootPoint; // where we are shooting the arrow from
    [SerializeField] private GameObject shootObject; // what prefab we are using as a projectile
    [SerializeField] private Transform aimTrack; // allowing me to include the aimtrack in inspector
    [SerializeField] private float shootForce; // how much force to use when shooting
    
    private GameObject _arrow;
    private Vector3 _shootDirection;
    private PlayerState _currentState;
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
    
    private void OnEnable()
    {
        shootInput.Enable();
        shootInput.performed += Shoot;
        //getting and updating to the current state
        _playerController.OnStateUpdated += StateUpdate;
    }



    private void OnDisable()
    {
        shootInput.performed -= Shoot;
        _playerController.OnStateUpdated -= StateUpdate;
    }
    
        void StateUpdate(PlayerState state)
        {
            _currentState = state;
        }
        
    private void Shoot(InputAction.CallbackContext context)
    {
        if(_currentState != PlayerState.AIM) return;

        //calculate the direction
        _shootDirection = aimTrack.position - shootPoint.position;
        _shootDirection.Normalize();

        //create a new arrow
        _arrow = Instantiate(shootObject, shootPoint.position, Quaternion.LookRotation(_shootDirection));

        // apply a force
        _arrow.GetComponent<Rigidbody>().AddForce(shootForce * _shootDirection);
    }
    
}
