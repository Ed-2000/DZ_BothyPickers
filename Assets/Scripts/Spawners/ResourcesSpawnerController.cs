using System.Collections;
using UnityEngine;

public class ResourcesSpawnerController : MonoBehaviour
{
    [SerializeField] private ResourceSpawner[] _resourcesSpawners;
    [SerializeField] private SpawnedZone _spawnedZone;
    [SerializeField] private float _repeatRate;
    [SerializeField] private int _maxCountOfSpawnedObjects;

    private void OnValidate()
    {
        if (_maxCountOfSpawnedObjects > _spawnedZone.MaxCountOfSpawnedObjects)
            _maxCountOfSpawnedObjects = _spawnedZone.MaxCountOfSpawnedObjects;
    }

    private void Start()
    {
        for (int i = 0; i < _maxCountOfSpawnedObjects; i++)
            RandomSpawn();

        StartCoroutine(Spawn());
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

        while (true)
        {
            if (GetAllCountsActive() < _maxCountOfSpawnedObjects)
                RandomSpawn();

            yield return wait;
        }
    }

    private int GetAllCountsActive()
    {
        int allCountsActive = 0;

        foreach (var _resource in _resourcesSpawners)
            allCountsActive += _resource.GetCountActive();

        return allCountsActive;
    }
}