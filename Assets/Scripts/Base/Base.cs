using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;
using System.Collections.Generic;

[RequireComponent(typeof(ResourceAllocatore), typeof(BaseResourceStorage), typeof(BaseBotCreator))]
[RequireComponent(typeof(BaseUI), typeof(MarkerSetter), typeof(BaseScanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private List<BotHangar> _freeBotHangars;
    [SerializeField] private List<Bot> _freeBots;
    [SerializeField] private List<Bot> _busyBots;
    [SerializeField] private int _countOfResourcesToCreateBot = 3;
    [SerializeField] private int _countOfResourcesToCreateBase = 5;
    [SerializeField] private int _maxCountOfBots = 8;

    private BaseScanner _scanner;
    private ResourceAllocatore _resourceAllocatore;
    private BaseResourceStorage _resourceStorage;
    private BaseBotCreator _botCreator;
    private BaseUI _userInterface;
    private MarkerSetter _markerSetter;
    private Vector3 _newBasePosition;
    private bool _isMarkerSet = false;
    private Base _basePrefab;
    private NavMeshSurface _navMeshSurface;
    private ResourcesSpawner _resourcesSpawner;

    private void Start()
    {
        _triggerZone.BotIsBack += BotCameBackHandler;
        _markerSetter.Set += MarkerSetHandler;
    }

    private void Update()
    {
        if (_freeBots.Count == 0)
            return;

        SendBotToPicking();
    }

    private void OnDestroy()
    {
        _triggerZone.BotIsBack -= BotCameBackHandler;
        _markerSetter.Set -= MarkerSetHandler;
    }

    public void Init(BaseSpawner baseSpawner, ResourcesSpawner resourcesSpawner, NavMeshSurface navMeshSurface, ResourceAllocatore resourceAllocatore)
    {
        _scanner = GetComponent<BaseScanner>();
        _resourceStorage = GetComponent<BaseResourceStorage>();
        _botCreator = GetComponent<BaseBotCreator>();
        _userInterface = GetComponent<BaseUI>();
        _markerSetter = GetComponent<MarkerSetter>();

        _baseSpawner = baseSpawner;
        _resourcesSpawner = resourcesSpawner;
        _navMeshSurface = navMeshSurface;
        _resourceAllocatore = resourceAllocatore;

        _resourcesSpawner.RecalculateSpawnPositions();
        ReloadNavMesh();
    }

    public void AddBot(Bot bot)
    {
        _freeBots.Add(bot);
        bot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
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

                    _freeBots.Remove(bot);
                    bot.ArrivedAtSpecifiedPosition += CreateNewBase;
                    bot.SendToBuildNewBase(_newBasePosition);
                }
            }
            else if (_freeBots.Count + _busyBots.Count < _maxCountOfBots && _freeBotHangars.Count != 0 && _resourceStorage.ResouresCount >= _countOfResourcesToCreateBot)
            {
                Bot newBot = _botCreator.CreateBot();
                _resourceStorage.TakeResources(_countOfResourcesToCreateBot);
                newBot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
                _freeBots.Add(newBot);
            }

            if (_resourceAllocatore.Contains(resource))
                _resourceAllocatore.RemoveFromReserved(resource);

            _userInterface.DrawResources(_resourceStorage.ResouresCount);

            _resourcesSpawner.Release(resource);
        }
    }

    private void MarkerSetHandler(Vector3 markerPosition)
    {
        _isMarkerSet = true;
        _newBasePosition = markerPosition;
        _newBasePosition.y = transform.position.y;
    }

    private void CreateNewBase(Bot bot)
    {
        bot.ArrivedAtSpecifiedPosition -= CreateNewBase;
        Base newBase = _baseSpawner.Spawn(_newBasePosition);
        newBase.AddBot(bot);
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
        Resource resource = GetNearestResource();

        if (resource)
        {
            _resourceAllocatore.AddToReserved(resource);

            _freeBots[0].SetTargetResource(resource);
            _busyBots.Add(_freeBots[0]);
            _freeBots.RemoveAt(0);
        }
    }

    private Resource GetNearestResource()
    {
        List<Resource> resources = _resourceAllocatore.GetFree(_scanner.Scan());
        Resource resource = null;

        if (resources.Count > 0)
        {
            List<float> distances = new List<float>();

            for (int i = 0; i < resources.Count; i++)
                distances.Add(Vector3.SqrMagnitude(resources[i].transform.position - transform.position));

            int index = distances.IndexOf(distances.Min());

            resource = resources[index];
        }

        return resource;
    }

    private void ReloadNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
}