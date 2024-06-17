using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Vector3 _targetPosition;
    private float _distanceToCheck = 1.0f;

    public event Action ArrivedAtSpecifiedPosition;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _targetPosition = new Vector3();
        _target = transform;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(transform.position - _targetPosition) <= _distanceToCheck)
        {
            ArrivedAtSpecifiedPosition?.Invoke();
            MoveTo(_target.position);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _targetPosition = target.position;

        MoveTo(_targetPosition);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        _navMeshAgent.SetDestination(targetPosition);
        _targetPosition = targetPosition;
    }
}