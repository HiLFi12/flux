using UnityEngine;

/// <summary>
/// Plataforma que al ser tocada por objetos con tag "Gas" o "Solid" los devuelve a su Pool correspondiente.
/// Similar a HazardPlatform pero en lugar de aplicar daño/muerte, recicla.
///
/// Uso:
///  - Asigna referencias a las pools: solidPool y gasPool.
///  - Asegúrate de que los objetos instanciados desde esas pools tengan sus tags configuradas
///    correctamente a "Solid" o "Gas".
///  - Coloca un Collider2D (isTrigger o colisión física) en este GameObject.
///  - Cuando un objeto con el tag esperado entra o colisiona, se llama Release en la pool.
///    Si el objeto no está registrado en la pool (por ejemplo fue destruido manualmente) Release lo ignorará.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ReturnToPoolPlatform : MonoBehaviour
{
    [Header("Pools")]
    public SolidObjectPool solidPool;
    public GasObjectPool gasPool;

    [Header("Opciones")]
    [Tooltip("Aceptar OnCollision además de OnTrigger.")] public bool allowCollision = true;
    [Tooltip("Ocultar el objeto fallido (Destroy) si no está en la pool al intentar reciclar.")] public bool destroyIfNotInPool = false;
    [Tooltip("Loguear operaciones de retorno.")] public bool verbose = false;

    private void Awake()
    {
        // Forzamos trigger si se desea usar solo triggers
        // (No lo cambiamos automáticamente para no interferir si el usuario quiere colisión física)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryRecycle(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!allowCollision) return;
        TryRecycle(collision.gameObject);
    }

    private void TryRecycle(GameObject go)
    {
        if (go == null) return;
        string tag = go.tag;

        bool handled = false;
        if (tag == "Solid" && solidPool != null)
        {
            solidPool.Release(go); // Release solo actúa si el objeto está en uso
            handled = true;
            if (verbose) Debug.Log($"[ReturnToPoolPlatform] Recycle Solid -> {go.name}");
        }
        else if (tag == "Gas" && gasPool != null)
        {
            gasPool.Release(go);
            handled = true;
            if (verbose) Debug.Log($"[ReturnToPoolPlatform] Recycle Gas -> {go.name}");
        }

        if (!handled && destroyIfNotInPool)
        {
            // Caso: tag coincide pero pool no asignada o objeto no estuvo en pool (Release lo ignoró).
            if (tag == "Solid" || tag == "Gas")
            {
                if (verbose) Debug.LogWarning($"[ReturnToPoolPlatform] Objeto {go.name} no pudo reciclarse, destruyendo.");
                Destroy(go);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Vector3 size = new Vector3(box.size.x * transform.lossyScale.x, box.size.y * transform.lossyScale.y, 0.05f);
            Gizmos.DrawCube(box.offset, size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + circle.offset, circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y));
        }
        else if (col is CapsuleCollider2D cap)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + cap.offset, Mathf.Max(cap.size.x, cap.size.y) * 0.5f);
        }
        Gizmos.color = Color.white;
    }
#endif
}
