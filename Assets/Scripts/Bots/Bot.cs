using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private BotResourcesPicker _resourcesPicker;
    [SerializeField] private Transform _pointForTransportingResources;

    private Base _base;
    private BotMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
    }

    private void OnEnable()
    {
        _resourcesPicker.ResourceDiscovered += ResourceDiscoveredHandler;
    }

    private void OnDisable()
    {
        _resourcesPicker.ResourceDiscovered -= ResourceDiscoveredHandler;
    }

    private void ResourceDiscoveredHandler(Resource resource)
    {
        Transform resourceTransform = resource.transform;
        resourceTransform.SetParent(this.transform);
        resourceTransform.position = _pointForTransportingResources.position;

        _movement.SetTarget(_base.transform.position);
    }

    public void SetTargetResource(Resource targetResource)
    {
        _movement.SetTarget(targetResource.transform.position);
        _resourcesPicker.SetTarget(targetResource);
    }

    public void Init(Base baseObject)
    {
        _base = baseObject;
    }
}