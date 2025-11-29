using UnityEngine;

/// <summary>
/// Interfaz opcional para objetos poolables.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Llamado cuando el objeto se obtiene del pool.
    /// </summary>
    void OnAcquireFromPool();

    /// <summary>
    /// Llamado cuando el objeto se devuelve al pool.
    /// </summary>
    void OnReleaseToPool();
}
