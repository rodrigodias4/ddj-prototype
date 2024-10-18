using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Characters
{
    public class TestSitMethod : MonoBehaviour
    {
        private CustomerManager customerManager;

        void Start()
        {
            // Get reference to the CustomerManager
            customerManager = FindObjectOfType<CustomerManager>();

            // Start the coroutine to call the Sit method repeatedly
            StartCoroutine(CallSitMethodRepeatedly());
        }

        private IEnumerator CallSitMethodRepeatedly()
        {
            while (true) // Infinite loop for repeated calls
            {
                Debug.Log("Calling Sit method for testing.");
                customerManager.Sit();

                // Wait for a specified amount of time before the next call
                yield return new WaitForSeconds(5f); // Adjust the interval as needed
            }
        }
    }
}