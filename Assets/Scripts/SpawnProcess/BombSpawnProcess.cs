using UnityEngine;

public class BombSpawnProcess : MonoBehaviour
{
    [SerializeField] private BombSpawner _spawner;
    [SerializeField] private CubeSpawnProcess _spawnProcess;

    private void OnEnable()
    {
        _spawnProcess.CubeDead += OnCubeDead;
    }

    private void OnDisable()
    {
        _spawnProcess.CubeDead -= OnCubeDead;
    }

    private void OnCubeDead(Vector3 position)
    {
        _spawner.Spawn(position);
    }
}