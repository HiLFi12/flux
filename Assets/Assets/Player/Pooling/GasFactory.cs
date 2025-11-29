using UnityEngine;

/// <summary>
/// Factory para instanciar objetos gaseosos desde un GasObjectPool.
/// </summary>
public class GasFactory : MonoBehaviour
{
    public GasObjectPool pool;

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        if (pool == null)
        {
            Debug.LogWarning("GasFactory: pool no asignado.");
            return null;
        }
        return pool.Acquire(position, rotation);
    }

    public void Despawn(GameObject go)
    {
        if (pool == null || go == null) return;
        pool.Release(go);
    }
}
