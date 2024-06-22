using System.Collections.Generic;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private List<Resource> _resources = new List<Resource>();

    public int ResouresCount { get => _resources.Count; }

    public void AddResource(Resource resource)
    {
        _resources.Add(resource);
    }

    public void TakeResources(int count)
    {
        for (int i = 0; i < count; i++)
            _resources.RemoveAt(0);
    }
}