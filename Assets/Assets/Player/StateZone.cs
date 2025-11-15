using UnityEngine;

public class StateZone : MonoBehaviour
{
    public PlayerMovement.PlayerState zoneState;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.SetState(zoneState);
        }
    }
}
