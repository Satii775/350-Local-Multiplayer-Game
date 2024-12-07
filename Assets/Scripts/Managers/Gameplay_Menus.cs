using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay_Menus : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restarting the game...");
        // Reload the current scene
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
        // Quit the application
        Application.Quit();
    }

    public void LoadNewScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
