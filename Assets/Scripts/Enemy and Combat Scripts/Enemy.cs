using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float giveUpDistance;
    [SerializeField] private float chaseCheckAngle;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] public int damage;
    
    private EnemyState _currentState;
    private Transform _currentTarget;
    private bool _isWaiting = false;
    Vector3 _directionToPlayer;
    
    void Start()
    {
        _currentState = EnemyState.IDLE;
    }

    void FixedUpdate()
    {
        if(_currentState == EnemyState.IDLE)
        {
            enemyAnim.SetBool("Idle", true);
            if(!_isWaiting)
                StartCoroutine(WaitAndChooseARandomPointAndMove(5));

            //check for the player to chase
            if(IsPlayerInRange() && IsInFOV())
            {
                _currentState = EnemyState.CHASE;
                enemyAnim.SetBool("Idle", false);
            }
           
        }
        else if(_currentState == EnemyState.PATROL)
        {
            enemyAnim.SetBool("Walk", true);
            if(agent.remainingDistance <= .2f)
            {
                _currentState = EnemyState.IDLE;
                enemyAnim.SetBool("Walk", false);
            }

            // check for the player to chase
            if(IsPlayerInRange() && IsInFOV())
            {
                _currentState = EnemyState.CHASE;
                enemyAnim.SetBool("Walk", false);
            }

        }else if(_currentState == EnemyState.CHASE)
        {
            enemyAnim.SetBool("Chase", true);
            agent.SetDestination(playerTransform.position);

            // give up
            if(HasPlayerLeftRange())
            {
                _currentState = EnemyState.IDLE;
                enemyAnim.SetBool("Chase", false);
            }
        }
    }

    private IEnumerator WaitAndChooseARandomPointAndMove(float timeToWait)
    {
        _isWaiting = true;
        yield return new WaitForSeconds(timeToWait);
        _currentState = EnemyState.PATROL;
        ChooseARandomPointAndMove();
        _isWaiting = false;
    }


    private void ChooseARandomPointAndMove()
    {
        if(patrolPoints.Length <=0) return;
        _currentTarget = patrolPoints[Random.Range(0, patrolPoints.Length)];

        agent.SetDestination(_currentTarget.position);

    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= chaseDistance;
    }

    private bool HasPlayerLeftRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) >= giveUpDistance;
    }

  
    private bool IsInFOV()
    {
        _directionToPlayer = (playerTransform.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, _directionToPlayer) <= chaseCheckAngle;
    }
}
