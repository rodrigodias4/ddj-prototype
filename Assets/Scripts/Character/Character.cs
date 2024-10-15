using UnityEngine;

namespace Assets.Scripts.Characters {
    public class Character : MonoBehaviour
    {
        // Common properties for all characters
        public string characterName;
        public int health;
        public float speed;

        // Start is called before the first frame update
        protected virtual void Start()
        {
        }

        // Method to move the character
        public virtual void Move(Vector3 direction)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            Debug.Log($"{characterName} is moving.");
        }

        // Method to damage the character
        public virtual void TakeDamage(int damage)
        {
            health -= damage;
            Debug.Log($"{characterName} took {damage} damage, health now: {health}");

            if (health <= 0)
            {
                Die();
            }
        }

        // Method to handle character death
        protected virtual void Die()
        {
            Debug.Log($"{characterName} has died.");
            // Add more functionality for when the character dies (like disabling the character)
        }
    }
}