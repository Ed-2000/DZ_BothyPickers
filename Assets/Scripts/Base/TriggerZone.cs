using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public event Action<Bot> BotIsBack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bot bot))
            BotIsBack?.Invoke(bot);
    }
}
