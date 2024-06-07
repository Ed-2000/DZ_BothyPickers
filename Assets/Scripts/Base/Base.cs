using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseScanner))]
[RequireComponent(typeof(BaseResourceStorage))]
[RequireComponent(typeof(BaseUI))]
public class Base : MonoBehaviour
{
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private List<Bot> _freeBots;
    [SerializeField] private List<Bot> _busyBots;

    private BaseScanner _baseScanner;
    private BaseResourceStorage _resourceStorage;
    private BaseUI _userInterface;
    private List<Resource> _reservedResources;

    private void Awake()
    {
        _baseScanner = GetComponent<BaseScanner>();
        _resourceStorage = GetComponent<BaseResourceStorage>();
        _userInterface = GetComponent<BaseUI>();
        _reservedResources = new List<Resource>();

        foreach (var bot in _freeBots)
            bot.Init(this);
    }

    private void OnEnable()
    {
        _triggerZone.BotIsBack += BotCameBackHandler;
    }

    private void OnDisable()
    {
        _triggerZone.BotIsBack -= BotCameBackHandler;
    }

    private void Update()
    {
        if (_freeBots.Count == 0)
            return;

        SendBotToPicking();
    }

    private void BotCameBackHandler(Bot bot)
    {
        if (bot && _freeBots.Contains(bot) == false && bot.DiscoveredResource)
        {
            _freeBots.Add(bot);
            _busyBots.Remove(bot);

            Resource resource = bot.DiscoveredResource;
            _resourceStorage.AddResource(resource);
            _userInterface.DrawResources(_resourceStorage.GetResourcesCount());

            if (_reservedResources.Contains(resource))
                _reservedResources.Remove(resource);

            _resourcesSpawner.Release(resource);
        }
    }

    private void SendBotToPicking()
    {
        Resource resource = GetRandomResource();

        if (resource)
        {
            _reservedResources.Add(resource);

            _freeBots[0].SetTargetResource(resource);
            _busyBots.Add(_freeBots[0]);
            _freeBots.RemoveAt(0);
        }
    }

    private Resource GetRandomResource()
    {
        List<Resource> resources = _baseScanner.Scan();

        for (int i = 0; i < _reservedResources.Count; i++)
        {
            if (resources.Contains(_reservedResources[i]))
                resources.Remove(_reservedResources[i]);
        }

        return resources[Random.Range(0, resources.Count)];
    }
}