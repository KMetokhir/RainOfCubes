using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 20;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    private IPoolableObjectFactory _factory;
    private ObjectPool<IPoolable> _cubesPool;

    private Coroutine _spawnCoroutine;
    private bool _isSpawn = false;

    private void Awake()
    {
        _factory = GetComponent<IPoolableObjectFactory>();
        _cubesPool = new ObjectPool<IPoolable>(Create, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
    }

    public void StartSpawn()
    {
        if (_isSpawn)
        {
            return;
        }

        _isSpawn = true;
        _spawnCoroutine = StartCoroutine(SpawnCorutine(_spawnInterval));
    }

    public void StopSpawn()
    {
        if (_isSpawn == false)
        {
            return;
        }

        _isSpawn = false;
        StopCoroutine(_spawnCoroutine);
    }

    private IEnumerator SpawnCorutine(float delay)
    {
        WaitForSeconds delayTime = new WaitForSeconds(delay);

        while (_isSpawn)
        {
            Spawn();
            yield return delayTime;
        }
    }

    private void Spawn()
    {
        IPoolable poolableObject = _cubesPool.Get();
        poolableObject.SetPosition(GetRandomSpawnPosition());

        if (poolableObject == null)
        {
            return;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int minIndex = 0;
        int maxIndex = _spawnPoints.Count;
        Vector3 position = _spawnPoints[Random.Range(minIndex, maxIndex)].position;

        return position;
    }

    private void OnObjectDead(IPoolable poolableObject)
    {
        _cubesPool.Release(poolableObject);
    }

    private IPoolable Create()
    {
        IPoolable poolableObject = _factory.Create();

        return poolableObject;
    }

    private void OnReleaseToPool(IPoolable poolableObject)
    {
        poolableObject.Dead -= OnObjectDead;
        poolableObject.Deactivate();
    }

    private void OnGetFromPool(IPoolable poolableObject)
    {
        poolableObject.Dead += OnObjectDead;
        poolableObject.Activate();
    }

    private void OnDestroyPooledObject(IPoolable poolableObject)
    {
        poolableObject.Dead -= OnObjectDead;
        poolableObject.Destroy();
    }
}
