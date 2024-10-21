using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private float pauseOverlayAlpha = 0.9f;
    private bool gamePaused = false;

    void Start()
    {
        Application.targetFrameRate = 60;

        pauseOverlayAlpha = Mathf.Clamp(pauseOverlayAlpha, 0, 1);
    }

    // Check for the Escape key press to reload the scene
    void Update()
    {
        // Game running
        if (!gamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(PauseGame());
            }
        }
        // Game paused
        else
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ReloadScene();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(UnpauseGame());
            }
        }
    }

    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public IEnumerator PauseGame()
    {
        Time.timeScale = 0;

        Image image = pauseOverlay.GetComponentInChildren<Image>();

        // Fade in
        pauseOverlay.SetActive(true);
        Color color;
        while (image.color.a < pauseOverlayAlpha - 0.001)
        {
            color = image.color;
            color.a = Mathf.Lerp(color.a, pauseOverlayAlpha, Time.unscaledDeltaTime * 10f);
            image.color = color;
            yield return null;
        }
        
        color = image.color;
        color.a = pauseOverlayAlpha;
        image.color = color;

        Debug.Log("Game Paused");
        gamePaused = true;
    }

    public IEnumerator UnpauseGame()
    {
        Image image = pauseOverlay.GetComponentInChildren<Image>();

        // Fade out
        while (image.color.a > 0.001)
        {
            var color = image.color;
            color.a = Mathf.Lerp(color.a, 0, Time.unscaledDeltaTime * 10f);
            image.color = color;
            Debug.Log(image.color.a);
            yield return null;
        }

        var imageColor = image.color;
        imageColor.a = 0f;
        image.color = imageColor;
        
        Time.timeScale = 1;
        pauseOverlay.SetActive(false);
        Debug.Log("Game Unpaused");
        gamePaused = false;
    }
}
