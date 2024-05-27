using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Color _newCollisionObjectColor;
    [SerializeField] private Vector2Int _deathTimeRange = new Vector2Int(2, 6);

    private void OnValidate()
    {
        _deathTimeRange.x = Mathf.Abs(_deathTimeRange.x);
        _deathTimeRange.y = Mathf.Abs(_deathTimeRange.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IHittable hittable))
        {
            int deathTime = Random.Range(_deathTimeRange.x, _deathTimeRange.y);
            hittable.Hit(_newCollisionObjectColor, deathTime);
        }
    }
}
