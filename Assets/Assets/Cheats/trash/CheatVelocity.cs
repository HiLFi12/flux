using UnityEngine;

public class CheatVelocity : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement player; // Arrastrar el objeto del jugador en inspector

    [Header("Incremento de velocidad")]
    public float step = 1f; // cuánto aumenta o disminuye

    void Update()
    {
        if (player == null) return;

        // Disminuir velocidad
        if (Input.GetKeyDown(KeyCode.K))
        {
            AdjustVelocity(-step);
        }

        // Aumentar velocidad
        if (Input.GetKeyDown(KeyCode.L))
        {
            AdjustVelocity(step);
        }
    }

    private void AdjustVelocity(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidMoveSpeed = Mathf.Max(0f, player.solidMoveSpeed + delta);
                player.SetState(PlayerMovement.PlayerState.Solid); // aplicar el cambio
                Debug.Log("Solid MoveSpeed: " + player.solidMoveSpeed);
                break;

            case PlayerMovement.PlayerState.Liquid:
                player.liquidMoveSpeed = Mathf.Max(0f, player.liquidMoveSpeed + delta);
                player.SetState(PlayerMovement.PlayerState.Liquid);
                Debug.Log("Liquid MoveSpeed: " + player.liquidMoveSpeed);
                break;

            case PlayerMovement.PlayerState.Gas:
                player.gasMoveSpeed = Mathf.Max(0f, player.gasMoveSpeed + delta);
                player.SetState(PlayerMovement.PlayerState.Gas);
                Debug.Log("Gas MoveSpeed: " + player.gasMoveSpeed);
                break;
        }
    }
}
