using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Resources;

[RequireComponent(typeof(BaseScanner), typeof(BaseResourceStorage), typeof(BaseBotCreator))]
[RequireComponent(typeof(BaseUI), typeof(MarkerPlacer))]
public class Base : MonoBehaviour
{
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private List<BotHangar> _freeBotHangars;
    [SerializeField] private int _countOfResourcesToCreateBot = 3;
    [SerializeField] private int _countOfResourcesToCreateBase = 5;

    private BaseScanner _scanner;
    private ResourceAllocatore _resourceAllocatore;
    private BaseResourceStorage _resourceStorage;
    private BaseBotCreator _botCreator;
    private BaseUI _userInterface;
    private MarkerPlacer _markerPlacer;
    private Vector3 _newBasePosition;
    private bool _isMarkerSet = false;
    private Base _basePrefab;
    private NavMeshSurface _navMeshSurface;
    private List<Bot> _bots = new List<Bot>();

    private void Start()
    {
        _triggerZone.BotIsBack += HandleCameBackBot;
        _markerPlacer.Placed += HandlePlacedMarker;
    }

    private void Update()
    {
        foreach (Bot bot in _bots)
        {
            if (bot.IsFree)
                SendBotToPicking(bot);
        }
    }

    private void OnDestroy()
    {
        _triggerZone.BotIsBack -= HandleCameBackBot;
        _markerPlacer.Placed -= HandlePlacedMarker;
    }

    public void Init(BaseSpawner baseSpawner, NavMeshSurface navMeshSurface, ResourceAllocatore resourceAllocatore)
    {
        _scanner = GetComponent<BaseScanner>();
        _resourceStorage = GetComponent<BaseResourceStorage>();
        _botCreator = GetComponent<BaseBotCreator>();
        _userInterface = GetComponent<BaseUI>();
        _markerPlacer = GetComponent<MarkerPlacer>();

        _baseSpawner = baseSpawner;
        _navMeshSurface = navMeshSurface;
        _resourceAllocatore = resourceAllocatore;

        ReloadNavMesh();
    }

    public void CreateNewBot()
    {
        Bot bot = _botCreator.Create();
        bot.Init(GetFreeBotHangar(), _botCreator.BotParrent);
        _bots.Add(bot);
    }

    public void SendBotToPicking(Bot bot)
    {
        Resource resource = GetNearestResource();

        if (resource)
        {
            bot.SetTargetResource(resource);
            bot.IsFree = false;

            _resourceAllocatore.AddToReserved(resource);
        }
    }

    private void HandleCameBackBot(Bot bot)
    {
        if (bot && bot.IsFree == false)
        {
            if (bot.DiscoveredResource)
            {
                bot.IsFree = true;

                Resource resource = bot.DiscoveredResource;
                _resourceStorage.AddResource(resource);

                if (_isMarkerSet == true)
                {
                    TryToBuildNewBase(bot);
                }
                else if (_freeBotHangars.Count != 0 && _resourceStorage.ResouresCount >= _countOfResourcesToCreateBot)
                {
                    _resourceStorage.TakeResources(_countOfResourcesToCreateBot);
                    CreateNewBot();
                }

                if (_resourceAllocatore.ContainsInReserved(resource))
                    _resourceAllocatore.RemoveFromReserved(resource);

                _userInterface.DrawResources(_resourceStorage.ResouresCount);
            }
        }
    }

    private void TryToBuildNewBase(Bot bot)
    {
        if (_resourceStorage.ResouresCount >= _countOfResourcesToCreateBase)
        {
            _resourceStorage.TakeResources(_countOfResourcesToCreateBase);
            _isMarkerSet = false;

            bot.IsFree = false;
            bot.ArrivedAtSpecifiedPosition += CreateNewBase;
            bot.SendToBuildNewBase(_newBasePosition);
        }
    }

    private void HandlePlacedMarker(Vector3 markerPosition)
    {
        _isMarkerSet = true;
        _newBasePosition = markerPosition;
        _newBasePosition.y = transform.position.y;
    }

    private void CreateNewBase(Bot bot)
    {
        _markerPlacer.RemoveMarker();

        bot.ArrivedAtSpecifiedPosition -= CreateNewBase;
        Base newBase = _baseSpawner.Spawn(_newBasePosition);
        _freeBotHangars.Add(bot.Hangar);
        _bots.Remove(bot);
        Destroy(bot.gameObject);
        newBase.CreateNewBot();
    }

    private BotHangar GetFreeBotHangar()
    {
        BotHangar hangar = _freeBotHangars[0];

        _freeBotHangars.RemoveAt(0);

        return hangar;
    }

    private Resource GetNearestResource()
    {
        List<Resource> resources = _scanner.Scan();
        resources = _resourceAllocatore.GetFree(_scanner.Scan());

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