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
    [SerializeField] private Camera playerCamera; //getting player camera location for raycast
    [SerializeField] private float maxAimDistance; //setting the max distance the raycast will check
    [SerializeField] LayerMask aimCollisionMask;
    
    [Header("Audio Feedback Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    
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
        
        Vector3 aimPoint = FindAimPoint();
        
        _shootDirection = (aimPoint - shootPoint.position).normalized;

        _arrow = Instantiate(shootObject, shootPoint.position, Quaternion.LookRotation(_shootDirection));
        _arrow.GetComponent<Rigidbody>().AddForce(shootForce * _shootDirection);
        
        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }
    
    private Vector3 FindAimPoint()
    {
        Ray aimingRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(aimingRay, out RaycastHit hit, maxAimDistance, aimCollisionMask))
        {
            return hit.point;
        }

        return aimingRay.GetPoint(maxAimDistance);
    }
    
}
