using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField] private SpawnedZone _spawnedZone;
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField] private float _repeatRate = 1.0f;
    [SerializeField] private int _defaultCapacity = 20;

    private bool _isSpawning = true;
    private ObjectPool<Resource> _pool;
    private Transform _parent;

    public Resource ResourcePrefab { get => _resourcePrefab; private set => _resourcePrefab = value; }
    public int DefaultCapacity
    {
        get => _defaultCapacity;

        private set
        {
            if (value > _spawnedZone.MaxCountOfSpawnedObjects)
                _defaultCapacity = _spawnedZone.MaxCountOfSpawnedObjects;
        }
    }

    private void Awake()
    {
        _parent = this.transform;

        _pool = new ObjectPool<Resource>
            (
            createFunc: () => CreateResource(),
            actionOnGet: (obj) => GetResource(obj),
            actionOnRelease: (obj) => ReleaseResource(obj),
            actionOnDestroy: (obj) => DestroyResource(obj),
            defaultCapacity: DefaultCapacity
            );
    }

    private void Start()
    {
        for (int i = 0; i < DefaultCapacity; i++)
            RandomSpawn();

        StartCoroutine(Spawn());
    }

    public Resource Get()
    {
        return _pool.Get();
    }

    public void Release(Resource resource)
    {
        if (resource != null)
            _pool.Release(resource);
    }

    public void RecalculateSpawnPositions()
    {
        _spawnedZone.CalculateSpawnPositions();
    }

    private Resource CreateResource()
    {
        Resource poolObject = Instantiate(ResourcePrefab);
        poolObject.transform.SetParent(_parent);

        return poolObject;
    }

    private void GetResource(Resource resource)
    {
        if (resource.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.isKinematic = false;

        resource.gameObject.SetActive(true);
    }

    private void ReleaseResource(Resource resource)
    {
        resource.transform.SetParent(_parent);
        resource.gameObject.SetActive(false);
    }

    private void DestroyResource(Resource resource)
    {
        Destroy(resource.gameObject);
    }

    private void RandomSpawn()
    {
        Resource resource = _pool.Get();
        int _variableForHalving = 2;

        Vector3 position = _spawnedZone.GetPosition();
        position.y += resource.transform.localScale.y / _variableForHalving;

        resource.transform.position = position;
    }

    private IEnumerator Spawn()
    {
        var wait = new WaitForSeconds(_repeatRate);

        while (_isSpawning)
        {
            if (_pool.CountActive < DefaultCapacity)
                RandomSpawn();

            yield return wait;
        }
    }
}