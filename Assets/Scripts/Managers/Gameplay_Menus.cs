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
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the game...");
        // Quit the application
        Application.Quit();
    }
}
