using UnityEngine;

public class Arrows : MonoBehaviour
{
    private Rigidbody _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        Invoke(nameof(DestroyAfter), 5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ensuring arrows shoot in the direction camera and player are facing
        _rb.rotation = Quaternion.LookRotation(_rb.linearVelocity); 
    }

    void DestroyAfter()
    {
        Destroy(gameObject);
    }
}