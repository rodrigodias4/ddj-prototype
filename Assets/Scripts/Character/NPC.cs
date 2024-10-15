using UnityEngine;

namespace Assets.Scripts.Characters {
    public class NPC : Character
    {
        // Additional properties for NPCs
        public string[] dialogues;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start(); // Call the base start function
        }

        // NPC-specific behavior for dialogue
        public void Speak()
        {
            if (dialogues.Length > 0)
            {
                foreach (string dialogue in dialogues)
                {
                    Debug.Log($"{characterName} says: {dialogue}");
                }
            }
            else
            {
                Debug.Log($"{characterName} has nothing to say.");
            }
        }

        // Override the Move method if NPCs don't need to move or have different movement logic
        public override void Move(Vector3 direction)
        {
            // NPCs might have different movement patterns or may not move at all
            Debug.Log($"{characterName} is stationary and doesn't move like a player.");
            // Alternatively, call base.Move(direction) if you want NPCs to move like characters
        }

        // NPCs may have unique death behavior
        protected override void Die()
        {
            base.Die();
            Debug.Log($"{characterName} was an NPC and has now disappeared.");
            // Add additional NPC death behavior if needed
        }
    }
}
