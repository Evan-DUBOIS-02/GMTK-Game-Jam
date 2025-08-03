using UnityEngine;

namespace Player
{
    public class PlayerState
    {
        // All necessary stuff for a ghost to make same actions
        public Vector3 Position;
        public bool IsInteracting;
        public bool IsPlacingBomb;
        // Animation state
        // Looking right or left
        // ...

        // Normal contructor
        public PlayerState()
        {
            this.Position = Vector3.zero;
            this.IsInteracting = false; 
            this.IsPlacingBomb = false;
        }
        
        // Copie constructor
        public PlayerState(PlayerState other)
        {
            if (other != null)
            {
                this.Position = other.Position;
                this.IsInteracting = other.IsInteracting;
                this.IsPlacingBomb = other.IsPlacingBomb;
            }
        }
    }
}