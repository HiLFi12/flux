using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        // Asegura que el tiempo esté normal por si el juego está en pausa
        Time.timeScale = 1f;

        // Reinicia la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
