using System;
using UnityEngine;

[RequireComponent(typeof(Bot))]
public class BotCollisionHandler : MonoBehaviour
{
    private Resource _targetResource;
    private Bot _bot;

    public event Action<Resource> ResourceDiscovered;

    private void Awake()
    {
        _bot = GetComponent<Bot>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Resource resource) && _bot.TargetResource == resource)
        {
            ResourceDiscovered?.Invoke(resource);
            Destroy(resource.gameObject);
            //Transform resourceTransform = resource.transform;
            //resourceTransform.SetParent(this.transform);

            //Vector3 position = resourceTransform.position;
            //position.y += resourceTransform.localScale.y;
            //resourceTransform.position = position;
        }
    }

    public void Init(Resource targetResource)
    {
        _targetResource = targetResource;
    }
}