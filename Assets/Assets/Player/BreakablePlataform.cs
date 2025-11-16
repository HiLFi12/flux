using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Opciones")]
    public float breakDelay = 0.5f;
    public bool isBreaking = false;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void TriggerBreak()
    {
        if (!isBreaking)
            StartCoroutine(BreakRoutine());
    }

    private System.Collections.IEnumerator BreakRoutine()
    {
        isBreaking = true;

        yield return new WaitForSeconds(breakDelay);

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(3f);

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;

        isBreaking = false;
    }
}
