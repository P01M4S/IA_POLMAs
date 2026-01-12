using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AIENEMIGO : MonoBehaviour
{
    NavMeshAgent _enemy;
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching
    }

    public EnemyState currentState;
    public Transform[] _patrollPoints;
    Transform _player;
    float _detect = 3;
    float _serchTime;
    float _serchWait = 10;
    float _serchRadius = 10;
    Vector3 _playerLastPosition;
    float _detectionAngle = 90;

    void Awake()
    {
        _enemy = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetRandomPoint();
    }

    void Update()
    {
        switch(currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;
            case EnemyState.Chasing:
                Chase();
            break;
            case EnemyState.Searching:
                Search();
            break;
            default:
                Patrol();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }
        if(_enemy.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }
    }
    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Searching;
        }
        _enemy.SetDestination(_player.position);
        _playerLastPosition = _player.position;
    }
    void Search()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }
        _serchTime += Time.deltaTime;

        if(_serchTime < _serchWait)
        {
            if(_enemy.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if(RandomSearchPoint(_playerLastPosition, _serchRadius, out randomPoint))
                {
                    _enemy.SetDestination(randomPoint);
                }
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            _serchTime = 0;
        }
    }
    bool RandomSearchPoint(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 ramdomPoint = center + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(ramdomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    void SetRandomPoint()
    {
        _enemy.SetDestination(_patrollPoints[Random.Range(0, _patrollPoints.Length)].position);
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, _player.position) < _detect)
        {
            return  true;
        }
        else
        {
           return false;
        }*/
        Vector3 directionToPlay = _player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlay);
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if(_player.position == _playerLastPosition)
        {
            return true;
        }

        if(distanceToPlayer > _detect)
        {
            return false;
        }
        if(angleToPlayer > _detectionAngle * 0.5f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlay, out hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _playerLastPosition = _player.position;
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    
}
