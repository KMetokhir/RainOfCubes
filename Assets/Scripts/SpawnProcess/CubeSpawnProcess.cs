using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawnProcess : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private CubeSpawner _spawner;

    private Coroutine _spawnCoroutine;
    private bool _isSpawn = false;

    public event Action<Vector3> CubeDead;

    private void OnDisable()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
    }

    public void Execute()
    {
        if (_isSpawn)
        {
            return;
        }

        _isSpawn = true;
        _spawnCoroutine = StartCoroutine(SpawnCorutine(_spawnInterval));
    }

    private IEnumerator SpawnCorutine(float delay)
    {
        WaitForSeconds delayTime = new WaitForSeconds(delay);

        while (_isSpawn)
        {
            Cube cube = _spawner.Spawn(GetRandomSpawnPosition());
            cube.Dead += OnCubeDead;

            yield return delayTime;
        }
    }

    private void OnCubeDead(IPoolable cube)
    {
        CubeDead?.Invoke(cube.Position);
        cube.Dead -= OnCubeDead;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int minIndex = 0;
        int maxIndex = _spawnPoints.Count;
        Vector3 position = _spawnPoints[UnityEngine.Random.Range(minIndex, maxIndex)].position;

        return position;
    }
}