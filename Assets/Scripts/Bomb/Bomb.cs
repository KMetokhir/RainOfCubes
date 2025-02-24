using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class Bomb : MonoBehaviour, IPoolable, IExploisionTarget
{
    [SerializeField] private Color _defaultColor;

    [SerializeField] private float _exploisionRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private uint _lowerDetonationLimit = 2;
    [SerializeField] private uint _upperDetonationLimit = 6;

    private int _detonationTime;
    private Coroutine _alphaCanalCoroutine;

    private Material _material;
    private MeshRenderer _renderer;

    public event System.Action<IPoolable> Dead;
    private event System.Action _corutineComplete;

    public Vector3 Position => transform.position;
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
        Rigidbody = GetComponent<Rigidbody>();

        float tranparentBlendMode = 3;
        ChangeRenderMode(tranparentBlendMode);
    }

    private void OnDisable()
    {
        _corutineComplete -= OnCorutineComplete;

        if (_alphaCanalCoroutine != null)
        {
            StopCoroutine(_alphaCanalCoroutine);
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        SetRandomDetonationTime((int)_lowerDetonationLimit, (int)_upperDetonationLimit);

        _corutineComplete += OnCorutineComplete;

        if (_alphaCanalCoroutine != null)
        {
            StopCoroutine(_alphaCanalCoroutine);
        }

        ChangeColor(_defaultColor);
        float targetAlphaCanal = 0;
        _alphaCanalCoroutine = StartCoroutine(ChangeAlphaCanal(targetAlphaCanal));
    }

    public void Deactivate()
    {
        _corutineComplete -= OnCorutineComplete;
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ChangeColor(Color color)
    {
        _material.color = color;
    }

    private void OnCorutineComplete()
    {
        float alphaInvisibeValue = 0;

        if (Mathf.Approximately(_material.color.a, alphaInvisibeValue))
        {
            Explode();
            Dead?.Invoke(this);
        }
    }

    private void ChangeRenderMode(float value)
    {
        _material.SetFloat("_Mode", value);
        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _material.SetInt("_ZWrite", 0);
        _material.DisableKeyword("_ALPHATEST_ON");
        _material.EnableKeyword("_ALPHABLEND_ON");
        _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        _material.renderQueue = 3000;
    }

    private void SetRandomDetonationTime(int lowerLimit, int upperLimit)
    {
        _detonationTime = Random.Range(lowerLimit, upperLimit);
    }

    private void Explode()
    {
        List<Rigidbody> explodableRigidbodies = GetExplodableRigidbodies(transform.position, _exploisionRadius);

        foreach (Rigidbody explodableRigidbody in explodableRigidbodies)
        {
            explodableRigidbody.AddExplosionForce(_explosionForce, transform.position, _exploisionRadius);
        }
    }

    private List<Rigidbody> GetExplodableRigidbodies(Vector3 position, float explosionRadius)
    {
        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        Collider[] hitColliders = Physics.OverlapSphere(position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out IExploisionTarget target))
            {
                rigidbodies.Add(target.Rigidbody);
            }
        }

        return rigidbodies;
    }

    private IEnumerator ChangeAlphaCanal(float alphaValue)
    {
        float time = 0;

        Color startColor = _material.color;
        Color targetColor = new Vector4(_material.color.r, _material.color.b, _material.color.g, alphaValue);

        while (Mathf.Approximately(_material.color.a, alphaValue) == false)
        {
            time += Time.deltaTime / _detonationTime;

            _material.color = Vector4.MoveTowards(startColor, targetColor, time);

            yield return null;
        }

        _corutineComplete?.Invoke();
    }
}