using System.Collections;
using UnityEngine;

public class ResourcesSpawnerController : MonoBehaviour
{
    [SerializeField] private ResourceSpawner[] _resourcesSpawners;
    [SerializeField] private SpawnedZone _spawnedZone;
    [SerializeField] private float _repeatRate;
    [SerializeField] private int _targetCapacity;

    private bool _isSpawning = true;

    private void OnValidate()
    {
        if (_targetCapacity > _spawnedZone.MaxCountOfSpawnedObjects)
            _targetCapacity = _spawnedZone.MaxCountOfSpawnedObjects;
    }

    private void Start()
    {
        for (int i = 0; i < _targetCapacity; i++)
            RandomSpawn();

        StartCoroutine(Spawn());
    }

    public void AcceptResource(Resource resource)
    {
        for (int i = 0; i < _resourcesSpawners.Length; i++)
        {
            if (_resourcesSpawners[i].ResourcePrefab.GetType() == resource.GetType())
                _resourcesSpawners[i].Release(resource);
        }
    }

    private void RandomSpawn()
    {
        ResourceSpawner resourcesSpawner = _resourcesSpawners[Random.Range(0, _resourcesSpawners.Length)];
        Resource resource = resourcesSpawner.Get();
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
            if (GetAllCountsActiveResources() < _targetCapacity)
                RandomSpawn();

            yield return wait;
        }
    }

    private int GetAllCountsActiveResources()
    {
        int allCountsActive = 0;

        foreach (ResourceSpawner spawner in _resourcesSpawners)
            allCountsActive += spawner.GetCountActive();

        return allCountsActive;
    }
}