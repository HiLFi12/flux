using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Opciones")]
    public float breakDelay = 0.5f;
    private bool isBreaking = false;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null && player.currentState == PlayerMovement.PlayerState.Solid && !isBreaking)
        {
            StartCoroutine(BreakPlatform());
        }
    }

    private System.Collections.IEnumerator BreakPlatform()
    {
        isBreaking = true;
        yield return new WaitForSeconds(breakDelay);

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(3f); // tiempo de respawn opcional
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
        isBreaking = false;
    }
}
