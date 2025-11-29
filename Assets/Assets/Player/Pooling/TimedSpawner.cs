using UnityEngine;
using System.Collections;

/// <summary>
/// Spawner con temporizador: cada X segundos hace spawn usando una Factory (Sólida o Gaseosa).
/// - Asigna una de las factories: SolidFactory o GasFactory.
/// - Configura el intervalo y los puntos de spawn.
/// - Opcional: auto-despawn tras un lifetime (usando la misma factory para devolver al pool).
/// </summary>
public class TimedSpawner : MonoBehaviour
{
    [Header("Factory a usar (elige una)")]
    public SolidFactory solidFactory;
    public GasFactory gasFactory;

    [Tooltip("Si ambas factories están asignadas y esto es true, spawnea uno de cada por intervalo.")]
    public bool spawnBothIfAvailable = false;

    [Header("Temporizador")]
    public float intervalSeconds = 2f;
    public bool startOnAwake = true;
    public bool loop = true;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // si está vacío, usa el Transform del spawner
    public bool randomizePoints = false;

    [Header("Auto-Despawn (opcional)")]
    public bool enableAutoDespawn = false;
    public float lifetimeSeconds = 5f;

    private Coroutine _routine;
    private int _nextIndex = 0;

    void Awake()
    {
        if (startOnAwake)
            StartSpawning();
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        if (_routine != null) return;
        _routine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        if (intervalSeconds <= 0f) intervalSeconds = 0.01f;
        do
        {
            SpawnOne();
            yield return new WaitForSeconds(intervalSeconds);
        } while (loop);
        _routine = null;
    }

    private void SpawnOne()
    {
        var (pos, rot) = GetNextSpawnTransform();

        if (spawnBothIfAvailable && solidFactory != null && gasFactory != null)
        {
            var goSolid = solidFactory.Spawn(pos, rot);
            var goGas = gasFactory.Spawn(pos + Vector3.right * 0.5f, rot);
            if (enableAutoDespawn)
            {
                if (goSolid) StartCoroutine(AutoDespawn(goSolid));
                if (goGas) StartCoroutine(AutoDespawn(goGas));
            }
            return;
        }

        GameObject go = null;
        if (solidFactory != null)
            go = solidFactory.Spawn(pos, rot);
        else if (gasFactory != null)
            go = gasFactory.Spawn(pos, rot);
        else
        {
            Debug.LogWarning("TimedSpawner: No hay factory asignada.");
            return;
        }

        if (go != null && enableAutoDespawn)
            StartCoroutine(AutoDespawn(go));
    }

    private (Vector3, Quaternion) GetNextSpawnTransform()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            if (randomizePoints)
            {
                var p = spawnPoints[Random.Range(0, spawnPoints.Length)];
                return (p.position, p.rotation);
            }
            else
            {
                var p = spawnPoints[_nextIndex % spawnPoints.Length];
                _nextIndex++;
                return (p.position, p.rotation);
            }
        }
        return (transform.position, transform.rotation);
    }

    private IEnumerator AutoDespawn(GameObject go)
    {
        yield return new WaitForSeconds(lifetimeSeconds);
        if (go == null) yield break;
        if (solidFactory != null)
            solidFactory.Despawn(go);
        else if (gasFactory != null)
            gasFactory.Despawn(go);
    }
}
