using System;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private BotResourcesPicker _resourcesPicker;
    [SerializeField] private Transform _pointForTransportingResources;
    [SerializeField] private Transform _targetTransform;

    private BotHangar _hangar;
    private BotMovement _movement;
    private Resource _discoveredResource;
    private bool _isFree = true;

    public event Action<Bot> ArrivedAtSpecifiedPosition;

    public Resource DiscoveredResource { get => _discoveredResource; private set => _discoveredResource = value; }
    public bool IsFree { get => _isFree; private set => _isFree = value; }

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
    }

    private void OnEnable()
    {
        _resourcesPicker.ResourceDiscovered += ResourceDiscoveredHandler;
        _movement.ArrivedAtSpecifiedPosition += ArrivedAtSpecifiedPositionHandler;
    }

    private void OnDisable()
    {
        _resourcesPicker.ResourceDiscovered -= ResourceDiscoveredHandler;
        _movement.ArrivedAtSpecifiedPosition -= ArrivedAtSpecifiedPositionHandler;
    }

    public void Init(BotHangar hangar, Transform parent)
    {
        _hangar = hangar;
        transform.position = _hangar.transform.position;
        transform.SetParent(parent);
    }

    public void SetTargetResource(Resource targetResource)
    {
        DiscoveredResource = null;
        _targetTransform = targetResource.transform;
        _movement.SetTarget(_targetTransform);
        _resourcesPicker.SetTarget(targetResource);
    }

    public void SendToBuildNewBase(Vector3 position)
    {
        _targetTransform.position = position;
        _movement.SetTarget(_targetTransform);
    }

    private void ResourceDiscoveredHandler(Resource resource)
    {
        if (resource.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.isKinematic = true;

        Transform resourceTransform = resource.transform;
        resourceTransform.SetParent(this.transform);
        resourceTransform.position = _pointForTransportingResources.position;
        DiscoveredResource = resource;

        SummonBotToHangar();
    }

    private void ArrivedAtSpecifiedPositionHandler()
    {
        ArrivedAtSpecifiedPosition?.Invoke(this.GetComponent<Bot>());
    }

    private void SummonBotToHangar()
    {
        _movement.SetTarget(_hangar.transform);
    }
}