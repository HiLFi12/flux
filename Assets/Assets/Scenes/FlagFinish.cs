using UnityEngine;
using UnityEngine.SceneManagement; // importante

public class FlagFinish : MonoBehaviour
{
    [Header("Nombre de la escena de victoria")]
    public string winSceneName = "WinScreen";

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // Buscar un LevelChanger en la escena
            LevelChanger changer = FindObjectOfType<LevelChanger>();
            if (changer != null)
            {
                changer.LoadSceneByName(winSceneName); // llamamos al método correcto
            }
            else
            {
                // Si no hay LevelChanger, usamos SceneManager directamente
                Debug.LogWarning("No se encontró LevelChanger, usando SceneManager.");
                SceneManager.LoadScene(winSceneName);
            }
        }
    }
}
