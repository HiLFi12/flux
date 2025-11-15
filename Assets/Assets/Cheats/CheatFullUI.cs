using UnityEngine;
using TMPro;

public class CheatFullUI : MonoBehaviour
{
    public PlayerMovement player;
    public TextMeshProUGUI textDisplay; // Asignar en inspector

    void Update()
    {
        if (player == null || textDisplay == null) return;

        // Siempre activo
        textDisplay.gameObject.SetActive(true);

        // Mostrar estado y stats
        string state = player.currentState.ToString();
        float speed = 0f, jump = 0f, gravity = 0f;

        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                speed = player.solidMoveSpeed;
                jump = player.solidJumpForce;
                gravity = player.solidGravity;
                break;
            case PlayerMovement.PlayerState.Liquid:
                speed = player.liquidMoveSpeed;
                jump = 0f;
                gravity = player.liquidGravity;
                break;
            case PlayerMovement.PlayerState.Gas:
                speed = player.gasMoveSpeed;
                jump = player.gasFlapForce;
                gravity = player.gasGravity;
                break;
        }

        textDisplay.text =
            $"Estado: {state}\n" +
            $"Velocidad: {speed:F1}  (K/L)\n" +
            $"Salto/Flap: {jump:F1}  (O/P)\n" +
            $"Gravedad: {gravity:F1}  (G/H)"+
            $"Fuerza: {player.SolidStrength:F1}/{player.solidMaxStrength}\n";


        // Cambiar velocidad
        if (Input.GetKeyDown(KeyCode.L)) ChangeSpeed(1f);
        if (Input.GetKeyDown(KeyCode.K)) ChangeSpeed(-1f);

        // Cambiar salto
        if (Input.GetKeyDown(KeyCode.P)) ChangeJump(1f);
        if (Input.GetKeyDown(KeyCode.O)) ChangeJump(-1f);

        // Cambiar gravedad
        if (Input.GetKeyDown(KeyCode.H)) ChangeGravity(1f);
        if (Input.GetKeyDown(KeyCode.G)) ChangeGravity(-1f);
    }

    void ChangeSpeed(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Solid);
                break;
            case PlayerMovement.PlayerState.Liquid:
                player.liquidMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Liquid);
                break;
            case PlayerMovement.PlayerState.Gas:
                player.gasMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Gas);
                break;
        }
    }

    void ChangeJump(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidJumpForce += delta;
                player.SetState(PlayerMovement.PlayerState.Solid);
                break;
            case PlayerMovement.PlayerState.Gas:
                player.gasFlapForce += delta;
                player.SetState(PlayerMovement.PlayerState.Gas);
                break;
        }
    }

    void ChangeGravity(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidGravity += delta;
                player.SetState(PlayerMovement.PlayerState.Solid);
                break;
            case PlayerMovement.PlayerState.Liquid:
                player.liquidGravity += delta;
                player.SetState(PlayerMovement.PlayerState.Liquid);
                break;
            case PlayerMovement.PlayerState.Gas:
                player.gasGravity += delta;
                player.SetState(PlayerMovement.PlayerState.Gas);
                break;
        }
    }
}
