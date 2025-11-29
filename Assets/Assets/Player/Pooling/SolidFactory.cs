using UnityEngine;

/// <summary>
/// Factory para instanciar objetos sólidos desde un SolidObjectPool.
/// </summary>
public class SolidFactory : MonoBehaviour
{
    public SolidObjectPool pool;

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        if (pool == null)
        {
            Debug.LogWarning("SolidFactory: pool no asignado.");
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
