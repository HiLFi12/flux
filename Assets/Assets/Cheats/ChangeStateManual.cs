using UnityEngine;

public class ChangeStateManual : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public PlayerMovement player;

    void Update()
    {
        if (player == null) return; // evita errores si no asignaste el jugador

        // Cambiar estado con teclas 1, 2, 3 (números de arriba del teclado)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.SetState(PlayerMovement.PlayerState.Solid);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            player.SetState(PlayerMovement.PlayerState.Liquid);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            player.SetState(PlayerMovement.PlayerState.Gas);
    }
}
