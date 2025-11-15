using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
   public void LoadSceneByName(string sceneName)
   {
       SceneManager.LoadScene(sceneName);
   } 
}
