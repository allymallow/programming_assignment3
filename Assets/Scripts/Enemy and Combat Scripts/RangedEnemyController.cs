using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class RangedEnemyController : EnemyBase
{
    [Header("Ranged Attack Info")]
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rangedAttackDistance;
    [SerializeField] private float tooCloseDistance;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;

    protected override void OnEnterAttackRange()
    {
        agent.ResetPath(); // stop the enemy from moving too close to the player for ranged
    }

    protected override void HandleAttack() //overriding parent class HandleAttack for ranged specifics
    {
        enemyAnim.SetBool("Attack", true);
        
        //making the enemy face the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
        
        //and back away if they are too close to shoot
        if(IsTooClose())
            agent.SetDestination(transform.position - direction * 3F);

        AttackTimer += Time.fixedDeltaTime; 
        if (AttackTimer >= attackCooldown) // only allow the enemy to shoot if the cooldown has ended
        {
            Shoot();
            AttackTimer = 0f; //reset the timer once the enemy has created projectile
        }

        if (HasPlayerLeftAttackRange())
        {
            CurrentState = EnemyState.CHASE;
            enemyAnim.SetBool("Attack", false);
        }
    }

    protected override bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= rangedAttackDistance;
    }
    
    protected override bool HasPlayerLeftAttackRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) >= rangedAttackDistance;
    }

    private bool IsTooClose()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= tooCloseDistance;
    }

    private void Shoot()
    {
        //null check in case of missing projectile or fire point on the enemy
        if (enemyProjectile == null || firePoint == null) return;
        
        Vector3 direction = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(enemyProjectile, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        }
        
        //Play audio when the enemy shoots projectiles
        if(audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }
}
