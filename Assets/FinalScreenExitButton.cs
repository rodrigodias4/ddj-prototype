using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScreenExitButton : MonoBehaviour
{
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
