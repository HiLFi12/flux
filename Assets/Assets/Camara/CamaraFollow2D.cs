using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Jugador a seguir")]
    public Transform target; // el jugador

    [Header("Suavizado")]
    public float smoothSpeed = 0.125f; // cuanto más bajo, más lento y suave
    public Vector3 offset = new Vector3(0f, 1f, -10f); // separación inicial cámara-jugador
    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
