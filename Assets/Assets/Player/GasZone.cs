using UnityEngine;

public class GasZone : MonoBehaviour
{
    public PlayerMovement.PlayerState zoneState = PlayerMovement.PlayerState.Gas;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.SetState(zoneState);
        }
    }
}
