using UnityEngine;

public class BreakLever : MonoBehaviour
{
    [Header("Bloque a romper")]
    public GameObject blockToBreak;

    [Header("Opciones")]
    public bool isActivated = false; // Evita activaciones múltiples

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Solo jugador en estado sólido
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null && player.currentState == PlayerMovement.PlayerState.Solid && !isActivated)
        {
            ActivateLever();
        }
    }

    private void ActivateLever()
    {
        isActivated = true;

        if (blockToBreak != null)
        {
            Destroy(blockToBreak); // Rompe el bloque asignado
        }

        // Cambio visual opcional para indicar activación
        if (sr != null)
            sr.color = Color.red;
    }
}
