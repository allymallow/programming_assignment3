using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float projectileLifetime;

    void Start()
    {
        //destroy the projectile after lifetime timeframe if it doesn't hit the player character
        Invoke(nameof(DestroyAfter), projectileLifetime); 
    }

    void OnTriggerEnter(Collider other)
    {
        //damage the player on collision
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);//destroy the projectile if it does hit the player
        }
    }

    void DestroyAfter()
    {
        Destroy(gameObject); 
    }
}
