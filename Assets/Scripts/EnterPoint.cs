using UnityEngine;

public class EnterPoint : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;

    private void Start()
    {
        _spawner.StartSpawn();
    }
}
