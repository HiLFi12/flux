using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plataforma peligrosa:
///  - Si el jugador la toca en estado Líquido: pierde vida por segundo según <see cref="liquidDrainPerSecondOnContact"/>.
///  - Si la toca en estado Sólido o Gaseoso: muerte instantánea (reinicia la escena).
/// Úsalo en un GameObject con Collider2D (trigger o colisión física). 
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HazardPlatform : MonoBehaviour
{
    [Header("Daño para estado Líquido")]
    [Tooltip("Cantidad de vida por segundo que pierde el jugador al tocar esta plataforma en estado Líquido.")]
    public float liquidDrainPerSecondOnContact = 25f;

    [Header("Ajustes")]
    [Tooltip("Si true, este script también funcionará con colisiones físicas (OnCollisionStay2D).")]
    public bool allowCollision = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyHazard(other);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!allowCollision) return;
        TryApplyHazard(collision.collider);
    }

    private void TryApplyHazard(Collider2D col)
    {
        var player = col.GetComponent<PlayerMovement>();
        if (player == null)
        {
            // En caso de colisionar con un child collider del jugador
            if (col.attachedRigidbody != null)
                player = col.attachedRigidbody.GetComponent<PlayerMovement>();
        }
        if (player == null) return;

        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Liquid:
                ApplyLiquidDrain(player);
                break;
            case PlayerMovement.PlayerState.Solid:
            case PlayerMovement.PlayerState.Gas:
                KillPlayer();
                break;
        }
    }

    private void ApplyLiquidDrain(PlayerMovement player)
    {
        if (player.liquidCurrentHealth <= 0f)
        {
            KillPlayer();
            return;
        }

        float delta = liquidDrainPerSecondOnContact * Time.deltaTime;
        player.liquidCurrentHealth = Mathf.Max(0f, player.liquidCurrentHealth - delta);

        if (player.liquidCurrentHealth <= 0f)
        {
            KillPlayer();
            return;
        }
        // Nota: El evento OnLiquidHealthChanged es privado de PlayerMovement (no accesible desde aquí).
        // La escala visual del líquido se actualiza cada frame en PlayerMovement.Move() usando liquidCurrentHealth.
    }

    private void KillPlayer()
    {
        // Asegura que el tiempo esté normal por si el juego está en pausa
        Time.timeScale = 1f;
        // Reinicia la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.35f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Vector3 size = new Vector3(box.size.x * transform.lossyScale.x, box.size.y * transform.lossyScale.y, 0.05f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
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
