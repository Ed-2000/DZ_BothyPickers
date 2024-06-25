using System.Collections.Generic;
using UnityEngine;

public class ResourceAllocatore : MonoBehaviour
{
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    
    private List<Resource> _reservedResources = new List<Resource>();

    public List<Resource> GetFree(List<Resource> resources)
    {
        for (int i = 0; i < _reservedResources.Count; i++)
        {
            if (resources.Contains(_reservedResources[i]))
                resources.Remove(_reservedResources[i]);
        }

        return resources;
    }

    public void AddToReserved(Resource resource)
    {
        _reservedResources.Add(resource);
    }

    public void RemoveFromReserved(Resource resource)
    {
        _reservedResources.Remove(resource);
        _resourcesSpawner.Release(resource);
    }

    public bool ContainsInReserved(Resource resource)
    {
        return _reservedResources.Contains(resource);
    }
}