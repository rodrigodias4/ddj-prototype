using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(PressAnyKey());
    }

    private IEnumerator PressAnyKey()
    {
        yield return new WaitForSeconds(1f);

        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
