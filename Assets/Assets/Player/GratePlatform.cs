using UnityEngine;

public class GratePlatform : MonoBehaviour
{
    private Collider2D grateCol;

    void Awake()
    {
        grateCol = GetComponent<Collider2D>();
        if (grateCol == null)
            Debug.LogError("La rejilla necesita un Collider2D.");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null || grateCol == null) return;

        // LIQUIDO → no colisiona
        if (player.currentState == PlayerMovement.PlayerState.Liquid)
        {
            Physics2D.IgnoreCollision(other, grateCol, true);
        }
        else
        {
            Physics2D.IgnoreCollision(other, grateCol, false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // cuando sale, restablecer colisión
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null && grateCol != null)
        {
            Physics2D.IgnoreCollision(other, grateCol, false);
        }
    }
}
