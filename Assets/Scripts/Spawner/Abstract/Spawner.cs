using System;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner<T> : MonoBehaviour
   where T : MonoBehaviour, IPoolable
{
    [Header("Pool Settings")]
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 20;

    [Header("Poolable Prefab")]
    [SerializeField] private T _prefab;

    private uint _createdObjects;
    private uint _spawnedObjects;

    private ObjectPool<T> _pool;

    public event Action<uint> ObjectCreated;
    public event Action<uint> ObjectSpawned;
    public event Action<uint> ObjectActiveStatusChanged;

    private void Awake()
    {
        _pool = new ObjectPool<T>(Create, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
    }

    private void Start()
    {
        ObjectCreated?.Invoke(_createdObjects);
        ObjectSpawned?.Invoke(_spawnedObjects);
        ObjectActiveStatusChanged(0);
    }

    public T Spawn(Vector3 position)
    {
        T poolableObject = _pool.Get();
        poolableObject.SetPosition(position);
        _spawnedObjects++;
        ObjectSpawned?.Invoke(_spawnedObjects);

        return poolableObject;
    }

    protected virtual void OnReleaseToPool(T poolableObject)
    {
        poolableObject.Dead -= OnObjectDead;
        poolableObject.Deactivate();

        ObjectActiveStatusChanged?.Invoke((uint)_pool.CountActive);
    }

    protected virtual void OnGetFromPool(T poolableObject)
    {
        poolableObject.Dead += OnObjectDead;
        poolableObject.Activate();

        uint activeObjects = (uint)_pool.CountActive;
        ObjectActiveStatusChanged?.Invoke(activeObjects);
    }

    protected virtual void OnDestroyPooledObject(T poolableObject)
    {
        poolableObject.Dead -= OnObjectDead;
        poolableObject.Destroy();
    }

    private T Create()
    {
        MonoBehaviour entity = Instantiate(_prefab as MonoBehaviour);
        T itemPool = entity as T;

        _createdObjects++;
        ObjectCreated?.Invoke(_createdObjects);

        return itemPool;
    }

    private void OnObjectDead(IPoolable poolableObject)
    {
        T genericObject = poolableObject as T;
        _pool.Release(genericObject);
    }
}