using UnityEngine;

public class SolidZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.SetState(PlayerMovement.PlayerState.Solid);

            // 🔥 Restaura la fuerza al máximo
            player.RestoreSolidStrength();
        }
    }
}
