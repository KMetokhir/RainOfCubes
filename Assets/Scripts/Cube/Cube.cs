using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class Cube : MonoBehaviour, IHittable, IPoolable, IExploisionTarget
{
    [SerializeField] private Color _defaultColor;

    private Material _material;
    private bool _isHit = false;
    private Coroutine _deadCoroutine;

    public event Action<IPoolable> Dead;

    public Vector3 Position => transform.position;
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        Rigidbody = GetComponent<Rigidbody>();

        ChangeColor(_defaultColor);
    }

    public void Hit(Color color, int deathTime)
    {
        if (_isHit)
        {
            return;
        }

        _isHit = true;
        ChangeColor(color);

        _deadCoroutine = StartCoroutine(DelayedDeath(deathTime));
    }

    public void ChangeColor(Color color)
    {
        _material.color = color;
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        if (_deadCoroutine != null)
        {
            StopCoroutine(_deadCoroutine);
        }

        ChangeColor(_defaultColor);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Deactivate()
    {
        _isHit = false;
        StopCoroutine(_deadCoroutine);

        gameObject.SetActive(false);
    }

    private IEnumerator DelayedDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        Dead?.Invoke(this);
    }
}