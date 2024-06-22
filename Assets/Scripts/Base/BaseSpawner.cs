using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;
    [SerializeField] private Vector3 _firstBasePosition;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private ResourceAllocatore _resourceAllocatore;
    [SerializeField] private Camera _camera;
    [SerializeField] private List<Bot> _bots;

    private void Awake()
    {
        Base newBase = Spawn(_firstBasePosition);

        foreach (Bot bot in _bots)
            newBase.AddBot(bot);
    }

    public Base Spawn(Vector3 position)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.GetComponent<MarkerSetter>().Init(_camera);
        newBase.Init(this, _resourcesSpawner, _navMeshSurface, _resourceAllocatore);

        return newBase;
    }
}