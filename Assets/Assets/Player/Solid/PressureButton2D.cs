using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PressureButton2D : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Collider físico del botón (no trigger)")]
    [SerializeField] private Collider2D baseCollider;
    [Tooltip("Zona trigger por encima del botón (isTrigger = true)")]
    [SerializeField] private Collider2D pressZoneTrigger; 
    [SerializeField] private Door2D targetDoor;

    [Header("Quién puede presionar")]
    [Tooltip("Capas consideradas 'sólidas' (cajas, bloques, etc.)")]
    [SerializeField] private LayerMask solidLayers;
    [Tooltip("Masa mínima opcional para objetos sólidos (0 = ignorar)")]
    [SerializeField] private float minMass = 0f;

    [Header("Restricciones")]
    [Tooltip("Tolerancia vertical para considerar que está por arriba")]
    [SerializeField] private float aboveEpsilon = 0.02f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private readonly HashSet<Rigidbody2D> inZone = new();
    private readonly HashSet<Rigidbody2D> inContact = new();
    private readonly Dictionary<Rigidbody2D, Collider2D> zoneColliderByRB = new();

    private void Reset()
    {
        AutoAssignColliders();
    }

    private void Awake()
    {
        if (baseCollider == null || pressZoneTrigger == null)
        {
            AutoAssignColliders();
        }

        if (pressZoneTrigger != null && !pressZoneTrigger.isTrigger)
            pressZoneTrigger.isTrigger = true;

        if (baseCollider != null && baseCollider.isTrigger)
            Debug.LogWarning("[PressureButton2D] baseCollider no debe ser trigger.");
    }

    private void Update()
    {
        EvaluatePressed();
    }

    private void AutoAssignColliders()
    {
        foreach (var c in GetComponents<Collider2D>())
        {
            if (c.enabled == false) continue;
            if (c.isTrigger)
            {
                if (pressZoneTrigger == null) pressZoneTrigger = c;
            }
            else
            {
                if (baseCollider == null) baseCollider = c;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        var rb = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody2D>();
        if (rb == null) return;

        inZone.Add(rb);
        zoneColliderByRB[rb] = other;
        if (debugLogs) Debug.Log($"[Button] Enter zone: {rb.name}", this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody2D>();
        if (rb == null) return;

        inZone.Remove(rb);
        zoneColliderByRB.Remove(rb);
        if (debugLogs) Debug.Log($"[Button] Exit zone: {rb.name}", this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var rb = collision.otherRigidbody ?? collision.collider.attachedRigidbody;
        if (rb == null) return;

        inContact.Add(rb);
        if (debugLogs) Debug.Log($"[Button] Contact enter: {rb.name}", this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var rb = collision.otherRigidbody ?? collision.collider.attachedRigidbody;
        if (rb == null) return;

        inContact.Add(rb); 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var rb = collision.otherRigidbody ?? collision.collider.attachedRigidbody;
        if (rb == null) return;

        inContact.Remove(rb);
        if (debugLogs) Debug.Log($"[Button] Contact exit: {rb.name}", this);
    }

    private void EvaluatePressed()
    {
        bool pressed = false;

        if (baseCollider == null || pressZoneTrigger == null)
        {
            SetDoor(false);
            return;
        }

        foreach (var rb in inZone)
        {
            if (rb == null) continue;
            if (!inContact.Contains(rb)) continue;           
            if (!IsAbove(rb)) continue;
            if (!IsValidPressor(rb)) continue;               

            pressed = true;
            break;
        }

        SetDoor(pressed);
    }

    private bool IsAbove(Rigidbody2D rb)
    {
        if (baseCollider == null) return false;

        Collider2D col = null;
        if (!zoneColliderByRB.TryGetValue(rb, out col) || col == null)
        {
            var cols = rb.GetComponentsInChildren<Collider2D>();
            if (cols.Length > 0) col = cols[0];
        }
        if (col == null) return false;

        float buttonTop = baseCollider.bounds.max.y;
        float otherBottom = col.bounds.min.y;
        return otherBottom >= buttonTop - aboveEpsilon;
    }

    private bool IsValidPressor(Rigidbody2D rb)
    {
        var pm = rb.GetComponentInParent<PlayerMovement>();
        if (pm != null)
            return pm.currentState == PlayerMovement.PlayerState.Solid;

        if (((1 << rb.gameObject.layer) & solidLayers) != 0)
        {
            if (minMass <= 0f) return true;
            return rb.mass >= minMass;
        }

        return false;
    }

    private void SetDoor(bool pressed)
    {
        if (targetDoor != null)
            targetDoor.SetOpen(pressed);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (baseCollider != null)
        {
            Gizmos.color = Color.yellow;
            var b = baseCollider.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
        if (pressZoneTrigger != null)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            var b = pressZoneTrigger.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
#endif
}