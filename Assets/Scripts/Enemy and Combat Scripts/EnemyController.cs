using System;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static event Action<int> EnemyDestroyed; // event to allow GameManager to increment score
    [SerializeField] public int Score; //value to be incremented in GameManager
    
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            OnDestroyed(); //Destroy enemy when hit by arrow
        }
    }

    void OnDestroyed()
    {
        Destroy(gameObject);
        EnemyDestroyed?.Invoke(Score); //broadcast event
        
    }
}
