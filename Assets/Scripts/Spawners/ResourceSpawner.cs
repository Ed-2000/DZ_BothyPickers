using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _spawnedObject;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _defaultCapacity = 4;

    private ObjectPool<Resource> _pool;

    private void Awake()
    {
        InitPool();
    }

    private void InitPool()
    {
        _pool = new ObjectPool<Resource>
            (
            createFunc: () => Create(),
            actionOnGet: (obj) => ActionOnGet(obj),
            actionOnRelease: (obj) => ActionOnRelease(obj),
            actionOnDestroy: (obj) => ActionOnDestroy(obj),
            defaultCapacity: _defaultCapacity
            );
    }

    private Resource Create()
    {
        Resource poolObject = Instantiate(_spawnedObject);
        poolObject.transform.SetParent(_parent);

        return poolObject;
    }

    private void ActionOnGet(Resource poolObject)
    {
        poolObject.gameObject.SetActive(true);
    }

    private void ActionOnRelease(Resource poolObject)
    {
        if (poolObject != null)
            poolObject.gameObject.SetActive(false);
    }

    private void ActionOnDestroy(Resource poolObject)
    {
        Destroy(poolObject.gameObject);
    }

    public int GetCountActive()
    {
        return _pool.CountActive;
    }

    public Resource Get()
    {
        return _pool.Get();
    }

    public void Release(Resource poolObject)
    {
        _pool.Release(poolObject);
    }
}