using UnityEngine;

/// <summary>
/// GasPipe: "tubería" para el jugador en estado Gas.
/// Modo desplazamiento por camino (waypoints) en lugar de teletransporte.
/// Uso:
///  1) Añade un Collider2D (isTrigger) al objeto que tendrá este script (zona de entrada).
///  2) Crea una serie de Transforms ordenados en la escena y asígnalos al array <see cref="pathPoints"/>.
///     - Deben tener al menos 2 puntos. El primero es (aprox) la entrada y el último la salida.
///  3) Opcionalmente asigna un <see cref="exitPoint"/> para definir la dirección final de impulso (si null se usa el último punto).
///  4) El jugador (estado Gas) entra en el trigger, presiona la tecla (E por defecto) y recorre el camino a velocidad constante.
///  5) Al finalizar: se aplica un impulso configurado usando el eje Right del exitPoint (o del último pathPoint).
/// Notas:
///  - Se desactiva temporalmente el componente PlayerMovement para evitar inputs mientras se desplaza.
///  - Cooldown evita múltiples activaciones consecutivas.
///  - Para curvas suaves puedes colocar más puntos o usar objetos vacíos como control de trayectoria.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GasPipe : MonoBehaviour
{
    [Header("Waypoints del camino")]
    public Transform[] pathPoints; // Debe tener >= 2 para funcionar.
    [Tooltip("Punto opcional para definir la dirección final del impulso. Si null se usa el último del path.")] public Transform exitPoint;

    [Header("Interacción")]
    public KeyCode interactKey = KeyCode.E;
    public float cooldownSeconds = 0.5f;

    [Header("Movimiento dentro de la tubería")]
    [Tooltip("Velocidad lineal a lo largo del camino (unidades/segundo).")] public float travelSpeed = 10f;
    [Tooltip("True para bloquear nueva activación mientras se recorre el camino.")] public bool lockDuringTravel = true;

    [Header("Impulso al salir")]
    public float impulseForce = 12f; // Magnitud de la velocidad aplicada al final
    public float extraUpwardForce = 0f; // Permite añadir componente vertical adicional

    [Header("Feedback visual (opcional)")]
    public Color gizmoColor = Color.yellow;
    public float exitDirectionRay = 1.5f;

    private float _lastUseTime = -999f;
    private bool _playerInside;
    private PlayerMovement _cachedPlayer;
    private Collider2D _trigger;
    private bool _isTravelling;
    private Coroutine _travelRoutine;

    void Awake()
    {
        _trigger = GetComponent<Collider2D>();
        _trigger.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            _playerInside = true;
            _cachedPlayer = pm;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_cachedPlayer != null && other.GetComponent<PlayerMovement>() == _cachedPlayer)
        {
            _playerInside = false;
            _cachedPlayer = null;
        }
    }

    void Update()
    {
        if (_isTravelling) return; // No aceptar nueva interacción mientras viaja
        if (!_playerInside) return;
        if (_cachedPlayer == null) return;
        if (_cachedPlayer.currentState != PlayerMovement.PlayerState.Gas) return; // Solo Gas
        if (Time.time < _lastUseTime + cooldownSeconds) return;
        if (pathPoints == null || pathPoints.Length < 2) return; // Camino inválido

        if (Input.GetKeyDown(interactKey))
        {
            StartTravel(_cachedPlayer);
        }
    }

    private void StartTravel(PlayerMovement player)
    {
        if (_isTravelling) return;
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogWarning("GasPipe: pathPoints debe tener al menos 2 elementos.");
            return;
        }
        _lastUseTime = Time.time;
        _playerInside = false; // Evita reactivar mientras comienza
        _cachedPlayer = player;
        _isTravelling = true;

        // Desactivar el PlayerMovement para que no procese inputs durante el viaje
        player.enabled = false;
        _travelRoutine = StartCoroutine(TravelCoroutine(player));
    }

    private System.Collections.IEnumerator TravelCoroutine(PlayerMovement player)
    {
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("GasPipe: Player sin Rigidbody2D.");
            EndTravel(player);
            yield break;
        }

        // Colocar al jugador exactamente en el primer punto (si está lejos)
        rb.MovePosition(pathPoints[0].position);

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Transform a = pathPoints[i];
            Transform b = pathPoints[i + 1];
            if (a == null || b == null) continue;
            float dist = Vector3.Distance(a.position, b.position);
            if (dist < 0.0001f) continue;
            float t = 0f;
            while (t < 1f)
            {
                float step = (travelSpeed * Time.deltaTime) / dist; // Progreso normalizado
                t += step;
                if (t > 1f) t = 1f;
                Vector3 pos = Vector3.Lerp(a.position, b.position, t);
                rb.MovePosition(pos);
                yield return null;
            }
        }

        ApplyExitImpulse(player, rb);
        EndTravel(player);
    }

    private void ApplyExitImpulse(PlayerMovement player, Rigidbody2D rb)
    {
        Transform dirSource = exitPoint != null ? exitPoint : (pathPoints != null && pathPoints.Length > 0 ? pathPoints[pathPoints.Length - 1] : null);
        if (dirSource == null) return;
        Vector2 dir = dirSource.right.normalized;
        Vector2 vel = dir * impulseForce + Vector2.up * extraUpwardForce;
        rb.linearVelocity = vel;
    }

    private void EndTravel(PlayerMovement player)
    {
        player.enabled = true; // Reactivar control
        _isTravelling = false;
        _travelRoutine = null;
        _cachedPlayer = null;
    }

    // Gizmos para facilitar colocación en la escena
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        // Dibujar camino
        if (pathPoints != null && pathPoints.Length > 0)
        {
            for (int i = 0; i < pathPoints.Length; i++)
            {
                var p = pathPoints[i];
                if (p == null) continue;
                float r = (i == 0 || i == pathPoints.Length - 1) ? 0.3f : 0.2f;
                Gizmos.DrawWireSphere(p.position, r);
                if (i < pathPoints.Length - 1 && pathPoints[i + 1] != null)
                    Gizmos.DrawLine(p.position, pathPoints[i + 1].position);
            }
        }
        // Dirección de salida
        Transform dirSource = exitPoint != null ? exitPoint : (pathPoints != null && pathPoints.Length > 0 ? pathPoints[pathPoints.Length - 1] : null);
        if (dirSource != null)
        {
            Vector3 dir = dirSource.right.normalized * exitDirectionRay;
            Gizmos.DrawRay(dirSource.position, dir);
        }
    }
}
