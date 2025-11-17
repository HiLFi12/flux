using UnityEngine;

public class GratePlatform : MonoBehaviour
{
    [Header("Pon aquí el collider sólido (el que bloquea)")]
    public Collider2D solidCollider;  // NO trigger

    private void Reset()
    {
        // Autoasigna el primer collider como sólido
        var cols = GetComponents<Collider2D>();
        if (cols.Length > 0)
            solidCollider = cols[0];
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (solidCollider == null) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;

        bool isLiquid = (player.currentState == PlayerMovement.PlayerState.Liquid);

        // ← Solo esto determina si lo atraviesa
        Physics2D.IgnoreCollision(other, solidCollider, isLiquid);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (solidCollider == null) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Physics2D.IgnoreCollision(other, solidCollider, false);
        }
    }
}
