using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{

    // Check for the Escape key press to reload the scene
    void Update()
    {
        // Press the "Escape" key to reload the scene
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ReloadScene();
        }
    }
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
