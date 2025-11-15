using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Asignar un panel de UI en el Canvas (puede estar vacío)
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Cambiamos a la tecla "P" para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        // SceneManager.LoadScene("MainMenu"); // cuando tengas un menú principal
    }
}
