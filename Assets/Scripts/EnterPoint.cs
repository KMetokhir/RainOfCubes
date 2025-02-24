using UnityEngine;

public class EnterPoint : MonoBehaviour
{
    [SerializeField] private CubeSpawnProcess _spawnProcess;

    private void Start()
    {
        _spawnProcess.Execute();
    }
}