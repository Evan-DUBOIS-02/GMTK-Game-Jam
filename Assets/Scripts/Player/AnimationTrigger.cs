using UnityEngine;

namespace Player
{
    public class AnimationTrigger: MonoBehaviour
    {
        public void EndDisapear()
        {
            GetComponentInParent<PlayerManager>().ReturnToStartPosition();
        }
    }
}