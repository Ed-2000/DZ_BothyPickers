using System;
using UnityEngine;

public class BotResourcesPicker : MonoBehaviour
{
    private Resource _targetResource;

    public event Action<Resource> ResourceDiscovered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource) && _targetResource == resource)
            ResourceDiscovered?.Invoke(resource);
    }

    public void SetTarget(Resource targetResource)
    {
        _targetResource = targetResource;
    }
}