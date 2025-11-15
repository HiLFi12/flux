using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform currentTarget;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("[MovingPlatform] Faltan puntos de destino.");
            enabled = false;
            return;
        }

        currentTarget = pointB; // arranca yendo hacia B
    }

    void Update()
    {
        if (currentTarget == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Cuando llega al destino (distancia menor a 0.05)
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    // Para que el jugador se mueva con la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
