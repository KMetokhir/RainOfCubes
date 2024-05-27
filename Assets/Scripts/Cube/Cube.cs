using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class Cube : MonoBehaviour, IHittable, IPoolable
{
    private MeshRenderer _mesh;
    private bool isHit = false;
    private Coroutine _deadCoroutine;

    private Color _defaultColor;
    private bool _isInited = false;

    public event Action<IPoolable> Dead;

    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }

    public void Init(Color defaultColor)
    {
        if (_isInited)
        {
            return;
        }

        _isInited = true;
        _defaultColor = defaultColor;
        ChangeColor(_defaultColor);
    }

    public void Hit(Color color, int deathTime)
    {
        if (isHit)
        {
            return;
        }

        if (_isInited == false)
        {
            Debug.LogError("Cube doesn't inited");
        }

        isHit = true;
        ChangeColor(color);

        _deadCoroutine = StartCoroutine(DelayedDeath(deathTime));
    }

    public void ChangeColor(Color color)
    {
        _mesh.material.color = color;
    }

    public void Activate()
    {
        if (_isInited == false)
        {
            Debug.LogError("Cube doesn't inited");
        }

        gameObject.SetActive(true);

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
        isHit = false;
        StopCoroutine(_deadCoroutine);

        gameObject.SetActive(false);
    }

    private IEnumerator DelayedDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        Dead?.Invoke(this);
    }
}
