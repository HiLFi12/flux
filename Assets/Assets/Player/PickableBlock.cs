using UnityEngine;

public class PickableBlock : MonoBehaviour
{
    [Header("Opciones")]
    public float pickDistance = 1f;
    public LayerMask playerMask;
    private Transform holdPoint;

    private bool isHeld = false;
    private PlayerMovement holder;
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Coste de levantar (solo en estado sólido)")]
    public float pickupCost = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!isHeld)
        {
            CheckPickUp();
        }
        else
        {
            FollowHolder();

            if (Input.GetKeyDown(KeyCode.E))
                Drop();
        }
    }

    void CheckPickUp()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, pickDistance, playerMask);
        if (hit == null) return;

        PlayerMovement player = hit.GetComponent<PlayerMovement>();
        if (player == null) return;

        // SOLO si está en estado sólido
        if (player.currentState != PlayerMovement.PlayerState.Solid) return;

        // GASTAR ENERGÍA
        player.SolidStrength -= pickupCost;

        Transform hp = player.transform.Find("HoldPoint");
        if (hp == null)
        {
            Debug.LogError("Falta el HoldPoint en el jugador.");
            return;
        }

        holder = player;
        holdPoint = hp;

        isHeld = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
    }

    void FollowHolder()
    {
        if (holdPoint != null)
            transform.position = holdPoint.position;
    }

    void Drop()
    {
        isHeld = false;
        holder = null;
        holdPoint = null;

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (player.currentState != PlayerMovement.PlayerState.Solid)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickDistance);
    }
}
