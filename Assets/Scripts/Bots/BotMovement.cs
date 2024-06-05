using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _target;

    private void Awake()
    {
        _target = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Move();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Move()
    {
        _navMeshAgent.SetDestination(_target.position);
    }
}
