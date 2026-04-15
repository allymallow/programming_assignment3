using UnityEngine;

public class Arrows : MonoBehaviour
{
    [SerializeField] private float arrowLifetime;
    private Rigidbody _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        Invoke(nameof(DestroyAfter), arrowLifetime);
    }
    
    void FixedUpdate()
    {
        //ensuring arrows shoot in the direction camera and player are facing, but only if the arrow is moving
        if(_rb.linearVelocity.sqrMagnitude > 0.1f)
          _rb.rotation = Quaternion.LookRotation(_rb.linearVelocity); 
    }

    void DestroyAfter()
    {
        Destroy(gameObject);
    }
}