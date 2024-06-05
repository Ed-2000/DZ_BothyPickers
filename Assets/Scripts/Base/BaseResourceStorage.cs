using System.Collections.Generic;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private List<Resource> _resources = new List<Resource>();

    public void AddResource(Resource resource)
    {
        _resources.Add(resource);
    }

    public List<Resource> GetResources()
    {
        List<Resource> resources = new List<Resource>();

        for (int i = 0; i < _resources.Count; i++)
            resources.Add(_resources[i]);

        return resources;
    }
}