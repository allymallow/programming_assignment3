using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MeleeEnemyController : EnemyBase
{
    [Header("Melee Attack Info")] 
    [SerializeField] private float meleeAttackDistance;
    [SerializeField] public int damage;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip slimeClip;

    public static event Action<int> CausedDamage;

    protected override void HandleAttack() //override parent class HandleAttack method with melee specifics
    {
        enemyAnim.SetBool("Attack", true);

        AttackTimer += Time.fixedDeltaTime;
        if (AttackTimer >= attackCooldown) //only allow the enemy to attack/cause damage if not in cooldown
        {
            if (audioSource != null && slimeClip != null)
                audioSource.PlayOneShot(slimeClip); //play sound on attack
            CausedDamage?.Invoke(damage);
            AttackTimer = 0f; //reset the timer for cooldown 
        }

        if (HasPlayerLeftAttackRange())
        {
            CurrentState = EnemyState.CHASE;
            enemyAnim.SetBool("Attack", false);
        }
    }

    protected override bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= meleeAttackDistance;
    }

    protected override bool HasPlayerLeftAttackRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) >= meleeAttackDistance;
    }
}
