using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    [Header("Tecla para reiniciar")]
    public KeyCode restartKey = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartCurrentLevel();
        }
    }

    public void RestartCurrentLevel()
    {
        // Asegurarse de reanudar el tiempo por si está en pausa
        Time.timeScale = 1f;

        // Recarga la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
