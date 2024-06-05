using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public event Action<Bot> BotIsBack;
    public event Action<Resource> ResourceDiscovered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bot bot))
            BotIsBack?.Invoke(bot);
        else if (other.TryGetComponent(out Resource resource))
            ResourceDiscovered?.Invoke(resource);
    }
}
