using TMPro;
using UnityEngine;

public class SpawnerView<T> : MonoBehaviour
    where T : MonoBehaviour, IPoolable
{
    [SerializeField] private Spawner<T> _spawner;

    [SerializeField] private TMP_Text _header;
    [SerializeField] private TMP_Text _objectsCreatedPlate;
    [SerializeField] private TMP_Text _objectsSpawnedPlate;
    [SerializeField] private TMP_Text _objectsActivePlate;

    private void OnEnable()
    {
        _header.text = _spawner.name;

        _spawner.ObjectCreated += OnObjectCreated;
        _spawner.ObjectSpawned += OnObjectSpawned;
        _spawner.ObjectActiveStatusChanged += OnObjectChangedActiveStatus;
    }

    private void OnDisable()
    {
        _spawner.ObjectCreated -= OnObjectCreated;
        _spawner.ObjectSpawned -= OnObjectSpawned;
        _spawner.ObjectActiveStatusChanged -= OnObjectChangedActiveStatus;
    }

    private void OnObjectCreated(uint count)
    {
        string text = "Created Objects ";

        ChangeText(_objectsCreatedPlate, text, count);
    }

    private void OnObjectSpawned(uint count)
    {
        string text = "Spawned Objects ";

        ChangeText(_objectsSpawnedPlate, text, count);
    }

    private void OnObjectChangedActiveStatus(uint count)
    {
        string text = "Active Objects ";

        ChangeText(_objectsActivePlate, text, count);
    }

    private void ChangeText(TMP_Text textPlate, string text, uint count)
    {
        text += count;
        textPlate.text = text;
    }
}