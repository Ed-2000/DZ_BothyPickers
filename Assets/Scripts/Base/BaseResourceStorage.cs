using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseResourceStorage : MonoBehaviour
{
    private List<Resource> _resources = new List<Resource>();

    public void AddResource(Resource resource)
    {
        _resources.Add(resource);
        print(_resources.Count);
    }
}