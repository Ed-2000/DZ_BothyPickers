using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;
    private Vector3 _targetPosition;
    private float _distanceToCheck = 1.0f;

    public event Action ArrivedAtSpecifiedPosition;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _targetPosition = new Vector3();
        _targetTransform = transform;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(transform.position - _targetPosition) <= _distanceToCheck)
        {
            ArrivedAtSpecifiedPosition?.Invoke();
            MoveTo(_targetTransform.position);
        }
    }

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
        _targetPosition = target.position;

        MoveTo(_targetPosition);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(targetPosition);
        _targetPosition = targetPosition;
    }
}