using System;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static event Action<int> EnemyDestroyed; // event to allow GameManager to increment score
    public int Score = 1; //value to be incremented in GameManager
    
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void OnTriggerEnter(Collider other)
    {
            OnDestroyed(); // destroy the enemy when hit by trigger object (arrow)
    }

    void OnDestroyed()
    {
        Destroy(gameObject);
        EnemyDestroyed?.Invoke(Score); //broadcast event
        
    }
}
