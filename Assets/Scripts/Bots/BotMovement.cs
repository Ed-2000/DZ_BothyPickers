using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Vector3 _target;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Move();
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    private void Move()
    {
        _navMeshAgent.SetDestination(_target);
    }
}
