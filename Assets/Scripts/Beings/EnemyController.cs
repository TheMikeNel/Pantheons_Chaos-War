using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Attacking))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyController : MonoBehaviour
{
    [Header("Patroling")]
    public Transform patrolZoneCenter;
    public float randomRadius = 10f;
    public float timeOnPosition = 2f;
    public float timeOnPlayer = 1.5f;
    public bool enableRandomTime = true;

    [Header("Attacking")]
    public float attackRange = 2f;
    public float timeBetweenAttacks = 2f;
    public bool enableSpecialAttack = false;
    public float specialAttackRange = 5f;
    public float timeBetweenSpecialAttacks = 1f;

    //Components
    private Animator _anim;
    private NavMeshAgent _agent;
    private Attacking _atk;
    private GameObject _player;
    private Vector3 _currentDestination;

    //States
    private bool _onPosition = false;
    private bool _isTriggered = false;
    private bool _canAttack = false;
    private bool _canSpecialAttack = false;
    private bool _alreadySpecialAttacked;
    private bool _alreadyAttacked;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _atk = GetComponent<Attacking>();

        EventBus.Instance.SpawnEnemyLayerInd?.Invoke(gameObject.layer);
    }

    private void FixedUpdate()
    {
        if (!_isTriggered) Patroling();
        else PursuitPlayer();

        _canAttack = _isTriggered && (_player.transform.position - transform.position).magnitude <= attackRange;
        _canSpecialAttack = enableSpecialAttack && _isTriggered && (_player.transform.position - transform.position).magnitude <= specialAttackRange;

        if (!_canAttack && _canSpecialAttack && !_alreadySpecialAttacked)
            SpecialAttack();
        if (_canAttack && !_alreadyAttacked) 
            Attack();
    }

    private void Patroling()
    {
        if (!_onPosition && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            SetOnPosition(true);
            Invoke(nameof(ChangeRandomDestination), GetWaitingTime());
        }
    }


    private void PursuitPlayer()
    {
        if (!_onPosition)
        {
            SetCurrentDestination(_player.transform.position);

            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                SetOnPosition(true);
                Invoke(nameof(ResetOnPosition), timeOnPlayer);
            }            
        }
    }

    private void SetOnPosition(bool onPos)
    {
        _onPosition = onPos;
        _anim.SetBool("Move", !_onPosition);
    }

    private void ResetOnPosition()
    { SetOnPosition(false); }

    private void SetIsTriggered(bool isTrig, GameObject trigObj)
    {
        _isTriggered = isTrig;
        _player = trigObj;
        if (_player && _player.TryGetComponent(out Health target) && _atk) _atk.SetTarget(target);
        if (_isTriggered) ResetOnPosition();
        if (!_isTriggered) ChangeRandomDestination();
    }
    private void SpecialAttack()
    {
        if (!_alreadySpecialAttacked)
        {
            _anim.SetTrigger("SpecialAttack");
            _alreadySpecialAttacked = true;
            _alreadyAttacked = true;
            Invoke(nameof(ResetSpecialAttack), timeBetweenSpecialAttacks);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Attack()
    {
        if (!_alreadyAttacked)
        {
            _anim.SetTrigger("Attack");
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetSpecialAttack() => _alreadySpecialAttacked = false;

    private void ResetAttack() => _alreadyAttacked = false;

    private void ChangeRandomDestination()
    {
        //Debug.Log($"Change Destination");
        SetCurrentDestination(GetRandomPositionOnNavMesh());
        ResetOnPosition();
    }

    private bool SetCurrentDestination(Vector3 destination)
    {
        if (destination != _currentDestination)
        {
            NavMeshPath path = new();
            _agent.CalculatePath(destination, path);

            //Debug.Log($"Check path... Distance: {(destination - transform.position).magnitude}, Position: {destination}");

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                _currentDestination = destination;
                _agent.SetDestination(_currentDestination);
                return true;
            }
        }

        return false;
    }

    private Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randDirect = patrolZoneCenter ? Random.insideUnitSphere * randomRadius + patrolZoneCenter.position : Random.insideUnitSphere * randomRadius;
        NavMesh.SamplePosition(randDirect, out NavMeshHit hit, randomRadius, NavMesh.AllAreas);
        return hit.position;
    }

    private float GetWaitingTime() => enableRandomTime ? Random.Range(timeOnPosition * 0.5f, timeOnPosition * 2) : timeOnPosition;

    private void OnTriggerEnter(Collider other)
    { if (!_isTriggered && other.gameObject.CompareTag("Player")) SetIsTriggered(true, other.gameObject); }
    private void OnTriggerExit(Collider other)
    { if (_isTriggered && other.gameObject.CompareTag("Player")) SetIsTriggered(false, null); }
    private void OnDrawGizmos()
    { Gizmos.color = Color.red; Gizmos.DrawSphere(_currentDestination, 1f); }
}
