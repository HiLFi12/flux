using UnityEngine;

public class GasSource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.RefillGas();
        }
    }
}