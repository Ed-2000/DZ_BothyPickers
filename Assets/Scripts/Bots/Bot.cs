using UnityEngine;

public class Bot : MonoBehaviour
{
    private Base _base;
    private BotMovement _movement;
    private BotCollisionHandler _collisionHandler;
    private Resource _targetResource;

    public Resource TargetResource { get => _targetResource; private set => _targetResource = value; }

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
        _collisionHandler = GetComponent<BotCollisionHandler>();
    }

    private void OnEnable()
    {
        _collisionHandler.ResourceDiscovered += ResourceDiscoveredHandler;
    }

    private void OnDisable()
    {
        _collisionHandler.ResourceDiscovered -= ResourceDiscoveredHandler;
    }

    private void ResourceDiscoveredHandler(Resource resource)
    {
        _movement.SetTarget(_base.transform.position);
    }

    public void SetTargetResource(Resource targetResource)
    {
        _movement.SetTarget(targetResource.transform.position);
        TargetResource = targetResource;
    }

    public void Init(Base baseObject)
    {
        _base = baseObject;
    }
}