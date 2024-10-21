using UnityEngine;

namespace Assets.Scripts.Characters
{
    public class Chair : MonoBehaviour
    {
        public Transform tableTransform;
        public Transform chairPosition; // Position of the chair, used to move the customer
        public Customer customer; // Reference to the customer sitting on the chair

        private void Awake()
        {
            chairPosition = this.transform; // Assign the transform of the chair
            customer = null; // Initialize the customer reference
        }
    }
}