using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool genérico de GameObjects basados en un prefab.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Configuración del Pool")]
    public GameObject prefab;
    public int initialSize = 10;
    public bool expandable = true;
    public Transform container; // opcional para organizar jerarquía

    [Header("Depuración")]
    [Tooltip("Imprimir logs cuando se instancian o adquieren objetos.")]
    public bool verboseLogs = false;

    private readonly Queue<GameObject> _available = new Queue<GameObject>();
    private readonly HashSet<GameObject> _inUse = new HashSet<GameObject>();

    void Awake()
    {
        if (container == null) container = this.transform;
        Prewarm();
    }

    [ContextMenu("Prewarm")]
    public void Prewarm()
    {
        if (prefab == null || initialSize <= 0) return;
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(prefab, container);
            go.SetActive(false);
            _available.Enqueue(go);
            if (verboseLogs)
                Debug.Log($"[Pool {name}] Prewarm instancia #{i}: {go.name}");
        }
    }

    public GameObject Acquire(Vector3 position, Quaternion rotation)
    {
        GameObject go = null;
        if (_available.Count > 0)
        {
            go = _available.Dequeue();
        }
        else if (expandable && prefab != null)
        {
            go = Instantiate(prefab, container);
            if (verboseLogs)
                Debug.Log($"[Pool {name}] Expand instanciando nuevo objeto: {go.name}");
        }
        else
        {
            if (verboseLogs)
                Debug.Log($"[Pool {name}] Acquire falló: sin objetos y no expandable.");
            return null; // sin objetos y no expandable
        }

        _inUse.Add(go);
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        var poolable = go.GetComponent<IPoolable>();
        poolable?.OnAcquireFromPool();
        if (verboseLogs)
            Debug.Log($"[Pool {name}] Acquire -> {go.name} (Available={_available.Count}, InUse={_inUse.Count})");
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null) return;
        if (!_inUse.Contains(go)) return;

        var poolable = go.GetComponent<IPoolable>();
        poolable?.OnReleaseToPool();

        go.SetActive(false);
        go.transform.SetParent(container, false);
        _inUse.Remove(go);
        _available.Enqueue(go);
        if (verboseLogs)
            Debug.Log($"[Pool {name}] Release <- {go.name} (Available={_available.Count}, InUse={_inUse.Count})");
    }

    [ContextMenu("Release All In Use")]
    public void ReleaseAllInUse()
    {
        // Copia para evitar modificar mientras iteramos
        var list = new List<GameObject>(_inUse);
        foreach (var go in list)
            Release(go);
    }

    public int AvailableCount => _available.Count;
    public int InUseCount => _inUse.Count;
}
