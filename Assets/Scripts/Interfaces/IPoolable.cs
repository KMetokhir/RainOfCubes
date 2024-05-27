using System;
using UnityEngine;

public interface IPoolable
{
    public event Action<IPoolable> Dead;

    public void Activate();

    public void Deactivate();

    public void Destroy();

    public void SetPosition(Vector3 position);
}
