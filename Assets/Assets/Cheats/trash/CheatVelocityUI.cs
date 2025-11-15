using UnityEngine;
using TMPro;

public class CheatVelocityUI : MonoBehaviour
{
    public PlayerMovement player; 
    public TextMeshProUGUI textDisplay; // Asignar en el inspector
    private bool showUI = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            showUI = !showUI;

        if (textDisplay != null)
            textDisplay.gameObject.SetActive(showUI);

        if (!showUI || player == null || textDisplay == null) return;

        // Mostrar estado
        string state = player.currentState.ToString();

        // Mostrar velocidad y salto según el estado
        float speed = 0f;
        float jump = 0f;

        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                speed = player.solidMoveSpeed;
                jump = player.solidJumpForce;
                break;
            case PlayerMovement.PlayerState.Liquid:
                speed = player.liquidMoveSpeed;
                jump = 0f; // líquido no salta
                break;
            case PlayerMovement.PlayerState.Gas:
                speed = player.gasMoveSpeed;
                jump = player.gasFlapForce;
                break;
        }

        textDisplay.text = $"Estado: {state}\nVelocidad: {speed:F1}\nSalto: {jump:F1}";

        // Cambiar velocidad con K y L
        if (Input.GetKeyDown(KeyCode.L)) ChangeSpeed(1f);
        if (Input.GetKeyDown(KeyCode.K)) ChangeSpeed(-1f);

        // Cambiar salto con O y P
        if (Input.GetKeyDown(KeyCode.O)) ChangeJump(1f);
        if (Input.GetKeyDown(KeyCode.P)) ChangeJump(-1f);
    }

    void ChangeSpeed(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Solid); // refrescar
                break;
            case PlayerMovement.PlayerState.Liquid:
                player.liquidMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Liquid); // refrescar
                break;
            case PlayerMovement.PlayerState.Gas:
                player.gasMoveSpeed += delta;
                player.SetState(PlayerMovement.PlayerState.Gas); // refrescar
                break;
        }
    }

    void ChangeJump(float delta)
    {
        switch (player.currentState)
        {
            case PlayerMovement.PlayerState.Solid:
                player.solidJumpForce += delta;
                player.SetState(PlayerMovement.PlayerState.Solid); // refrescar
                break;
            case PlayerMovement.PlayerState.Gas:
                player.gasFlapForce += delta;
                player.SetState(PlayerMovement.PlayerState.Gas); // refrescar
                break;
        }
    }
}
