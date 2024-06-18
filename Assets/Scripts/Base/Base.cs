using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.AI.Navigation;

[RequireComponent(typeof(BaseScanner))]
[RequireComponent(typeof(BaseResourceStorage))]
[RequireComponent(typeof(BaseBotCreator))]
[RequireComponent(typeof(BaseUI))]
[RequireComponent(typeof(MarkerSetter))]
public class Base : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private int _countOfResourcesToCreateBot = 3;
    [SerializeField] private int _countOfResourcesToCreateBase = 5;
    [SerializeField] private int _maxCountOfBots = 8;
    [SerializeField] private List<BotHangar> _freeBotHangars;
    [SerializeField] private List<Bot> _freeBots;
    [SerializeField] private List<Bot> _busyBots;

    private BaseScanner _baseScanner;
    private BaseResourceStorage _resourceStorage;
    private BaseBotCreator _botCreator;
    private BaseUI _userInterface;
    private MarkerSetter _markerSetter;
    private List<Resource> _reservedResources;
    private Vector3 _newBasePosition;
    private bool _isMarkerSet = false;

    private void Awake()
    {
        _baseScanner = GetComponent<BaseScanner>();
        _resourceStorage = GetComponent<BaseResourceStorage>();
        _botCreator = GetComponent<BaseBotCreator>();
        _userInterface = GetComponent<BaseUI>();
        _markerSetter = GetComponent<MarkerSetter>();
        _reservedResources = new List<Resource>();

        foreach (var bot in _freeBots)
            bot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
    }

    private void OnEnable()
    {
        _triggerZone.BotIsBack += BotCameBackHandler;
        _markerSetter.Set += MarkerSetHandler;
    }

    private void OnDisable()
    {
        _triggerZone.BotIsBack -= BotCameBackHandler;
        _markerSetter.Set -= MarkerSetHandler;
    }

    private void Update()
    {
        if (_freeBots.Count == 0)
            return;

        SendBotToPicking();
    }

    public void Init(ResourcesSpawner resourcesSpawner, NavMeshSurface navMeshSurface, Bot bot, TextMeshProUGUI resourcesCountText, Camera camera)
    {
        _resourcesSpawner = resourcesSpawner;
        _resourcesSpawner.CalculateSpawnPositions();

        _navMeshSurface = navMeshSurface;
        ReloadNavMesh();

        _freeBots.Add(bot);
        bot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
        _userInterface.Init(resourcesCountText);
        _markerSetter.Init(camera);
    }

    private void BotCameBackHandler(Bot bot)
    {
        if (bot && _freeBots.Contains(bot) == false && bot.DiscoveredResource)
        {
            _freeBots.Add(bot);
            _busyBots.Remove(bot);

            Resource resource = bot.DiscoveredResource;
            _resourceStorage.AddResource(resource);

            if (_isMarkerSet == true)
            {
                if (_resourceStorage.ResouresCount >= _countOfResourcesToCreateBase)
                {
                    _isMarkerSet = false;
                    _resourceStorage.TakeResources(_countOfResourcesToCreateBase);

                    GameObject tempGameObject = new GameObject();
                    tempGameObject.transform.position = _newBasePosition;
                    _freeBots.Remove(bot);
                    bot.ArrivedAtSpecifiedPosition += CreateNewBase;
                    bot.SendToBuildNewBase(tempGameObject.transform);
                    //Destroy(tempGameObject);
                }
            }
            else if (_freeBots.Count + _busyBots.Count < _maxCountOfBots && _freeBotHangars.Count != 0 && _resourceStorage.ResouresCount >= _countOfResourcesToCreateBot)
            {
                Bot newBot = _botCreator.CreateBot();
                _resourceStorage.TakeResources(_countOfResourcesToCreateBot);
                newBot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
                _freeBots.Add(newBot);
            }

            if (_reservedResources.Contains(resource))
                _reservedResources.Remove(resource);

            _userInterface.DrawResources(_resourceStorage.ResouresCount);
            _resourcesSpawner.Release(resource);
        }
    }

    private void MarkerSetHandler(Vector3 position)
    {
        _isMarkerSet = true;
        _newBasePosition = position;
        _newBasePosition.y = this.transform.position.y;
    }

    private void CreateNewBase(Bot bot)
    {
        bot.ArrivedAtSpecifiedPosition -= CreateNewBase;

        Base newBase = Instantiate(_basePrefab, _newBasePosition, Quaternion.identity);
        newBase.Init(_resourcesSpawner, _navMeshSurface, bot, _userInterface.ResourcesCount, _markerSetter.Camera);
        _markerSetter.RemoveMarker();
    }

    private BotHangar GetFreeBotHangar()
    {
        BotHangar hangar = _freeBotHangars[0];

        _freeBotHangars.RemoveAt(0);

        return hangar;
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
        Resource resource = null;

        for (int i = 0; i < _reservedResources.Count; i++)
        {
            if (resources.Contains(_reservedResources[i]))
                resources.Remove(_reservedResources[i]);
        }

        if (resources.Count > 0)
            resource = resources[Random.Range(0, resources.Count)];

        return resource;
    }

    private void ReloadNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
}