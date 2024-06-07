using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private NavMeshPath _navMeshPath;
    private Transform _target;
    private Vector3 _oldTargetPosition;

    public event Action LostMyGoal;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshPath = new NavMeshPath();

        _oldTargetPosition = new Vector3();
        _target = transform;
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
        Vector3 targetPosition = _target.position;
        
        //if (!_navMeshAgent.CalculatePath(targetPosition, _navMeshPath))
        //{
        //    LostMyGoal?.Invoke();
        //    print("LostMyGoal" + this.transform.name);
        //    return;
        //}

        if (targetPosition != _oldTargetPosition)
        {
            _navMeshAgent.SetDestination(targetPosition);
            _oldTargetPosition = targetPosition;
        }
    }
}
