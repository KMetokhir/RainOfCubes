using UnityEngine;

public class CubeFactory : MonoBehaviour, IPoolableObjectFactory
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Color _defaultColor;

    public IPoolable Create()
    {
        Cube cube = Instantiate(_cubePrefab);
        cube.Init(_defaultColor);
        return cube;
    }
}
