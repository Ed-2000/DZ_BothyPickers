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
    [SerializeField] private int _startCountBots = 3;

    private void Awake()
    {
        Base newBase = Spawn(_firstBasePosition);

        for (int i = 0; i < _startCountBots; i++)
            newBase.CreateNewBot();
    }

    public Base Spawn(Vector3 position)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.GetComponent<MarkerPlacer>().Init(_camera);
        newBase.Init(this, _resourcesSpawner, _navMeshSurface, _resourceAllocatore);

        return newBase;
    }
}