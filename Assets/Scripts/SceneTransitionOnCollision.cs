using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnCollision : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        Debug.Log("other tag:" + other.tag);
        if (other.CompareTag("Player"))
        {
            // Load the next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
