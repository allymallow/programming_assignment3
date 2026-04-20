using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Setting the base class that enemies will inherit from
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected float chaseDistance;
    [SerializeField] protected float giveUpDistance;
    [SerializeField] protected float chaseCheckAngle;
    [SerializeField] protected Animator enemyAnim;
    [SerializeField] protected float attackCooldown;
    [SerializeField] public int score;

    [Header("Audio Feedback")] 
    [SerializeField] private AudioSource baseAudioSource;
    [SerializeField] private AudioClip scoreIncreaseClip;
    

    protected float AttackTimer;
    protected EnemyState CurrentState;
    protected bool IsWaiting = false;
    protected Vector3 DirectionToPlayer;
    protected Transform CurrentTarget;

    public static event Action<int> EnemyDestroyed;

    protected virtual void Start()
    {
        CurrentState = EnemyState.IDLE; //setting the default enemy state
    }

    protected virtual void FixedUpdate()
    {
        //controlling the enemy states with a switch instead of multiple if/else statements
        switch (CurrentState)
        {
         case EnemyState.IDLE:
             HandleIdle();
             break;
         case EnemyState.PATROL:
             HandlePatrol();
             break;
         case EnemyState.CHASE:
             HandleChase();
             break;
         case EnemyState.ATTACK:
             HandleAttack();
             break;
        }
    }

    protected void HandlePatrol()
    {
        enemyAnim.SetBool("Walk", true);
        if (agent.remainingDistance <= 0.2f)
        {
            CurrentState = EnemyState.IDLE;
            enemyAnim.SetBool("Walk", false);
        }

        if (IsPlayerInRange() && IsInFOV())
        {
            CurrentState = EnemyState.CHASE;
            enemyAnim.SetBool("Walk", false);
        }
    }
    
    protected void HandleChase()
    {
        enemyAnim.SetBool("Chase", true);
        agent.SetDestination(playerTransform.position);

        if (IsPlayerInAttackRange())
        {
            OnEnterAttackRange();
            CurrentState = EnemyState.ATTACK;
            enemyAnim.SetBool("Chase", false);
        }
        else if (HasPlayerLeftRange())
        {
            CurrentState = EnemyState.IDLE;
            enemyAnim.SetBool("Chase", false);
        }
    }

    protected void HandleIdle()
    {
        enemyAnim.SetBool("Idle", true);
        if (!IsWaiting)
            StartCoroutine(WaitAndChooseARandomPointAndMove(3f));

        if (IsPlayerInRange() && IsInFOV())
        {
            CurrentState = EnemyState.CHASE;
            enemyAnim.SetBool("Idle", false);
        }
    }

    //abstract void to allow each enemy to set their own versions of attack
    protected abstract void HandleAttack();
    
    private IEnumerator WaitAndChooseARandomPointAndMove(float timeToWait)
    {
        IsWaiting = true;
        yield return new WaitForSeconds(timeToWait);
        CurrentState = EnemyState.PATROL;
        ChooseARandomPointAndMove();
        IsWaiting = false;
    }

    protected virtual void OnEnterAttackRange()
    {
        //default virtual method with no inputs, but it can be overridden in the child class if needed
    }
    
    private void ChooseARandomPointAndMove()
    {
        if(patrolPoints.Length <=0) return;
        CurrentTarget = patrolPoints[UnityEngine.Random.Range(0, patrolPoints.Length)];

        agent.SetDestination(CurrentTarget.position);

    }

    protected bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= chaseDistance;
    }
    
    protected bool HasPlayerLeftRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) >= giveUpDistance;
    }

    //abstract bools so each enemy type can have its own attack range information
    protected abstract bool IsPlayerInAttackRange();
    protected abstract bool HasPlayerLeftAttackRange();
    
    protected bool IsInFOV()
    {
        DirectionToPlayer = (playerTransform.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, DirectionToPlayer) <= chaseCheckAngle;
    }

    protected void OnDestroyed()
    {
        if(baseAudioSource != null && scoreIncreaseClip != null)
            baseAudioSource.PlayOneShot(scoreIncreaseClip); //play sound on enemy death
        
        Destroy(gameObject, scoreIncreaseClip.length); //Destroy the object after the audio clip has finished
        EnemyDestroyed?.Invoke(score);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        OnDestroyed();
    }
}
