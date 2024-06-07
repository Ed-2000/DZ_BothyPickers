using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private BotResourcesPicker _resourcesPicker;
    [SerializeField] private Transform _pointForTransportingResources;

    private Base _base;
    private BotMovement _movement;
    private Resource _discoveredResource;

    public Resource DiscoveredResource { get => _discoveredResource; private set => _discoveredResource = value; }

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
    }

    private void OnEnable()
    {
        _resourcesPicker.ResourceDiscovered += ResourceDiscoveredHandler;
        _movement.LostMyGoal += SummonBotToBase;
    }

    private void OnDisable()
    {
        _resourcesPicker.ResourceDiscovered -= ResourceDiscoveredHandler;
        _movement.LostMyGoal -= SummonBotToBase;
    }
 
    public void Init(Base baseObject)
    {
        _base = baseObject;
    }   
    
    public void SetTargetResource(Resource targetResource)
    {
        DiscoveredResource = null;
        _movement.SetTarget(targetResource.transform);
        _resourcesPicker.SetTarget(targetResource);
    }

    private void ResourceDiscoveredHandler(Resource resource)
    {
        if (resource.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.isKinematic = true;

        Transform resourceTransform = resource.transform;
        resourceTransform.SetParent(this.transform);
        resourceTransform.position = _pointForTransportingResources.position;
        DiscoveredResource = resource;

        SummonBotToBase();
    }

    private void SummonBotToBase()
    {
        _movement.SetTarget(_base.transform);
    }
}