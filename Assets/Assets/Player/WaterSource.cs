using UnityEngine;

public class WaterSource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.RefillLiquid();
        }
    }
}
