using System;
using UnityEngine;

public interface IPoolable
{
    public event Action<IPoolable> Dead;

    public Vector3 Position { get; }

    public void Activate();

    public void Deactivate();

    public void Destroy();

    public void SetPosition(Vector3 position);
}