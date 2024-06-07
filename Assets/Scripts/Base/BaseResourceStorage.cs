using System.Collections.Generic;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private List<Resource> _resources = new List<Resource>();

    public void AddResource(Resource resource)
    {
        _resources.Add(resource);
    }

    public int GetResourcesCount()
    {
        return _resources.Count;
    }
}