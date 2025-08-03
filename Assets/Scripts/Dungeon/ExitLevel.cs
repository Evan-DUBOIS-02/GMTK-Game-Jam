using Game;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon
{
    public class ExitLevel: MonoBehaviour
    {
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (other.tag == "Player")
            {
                GameManager.Instance.EndLevel();
            }
        }
    }
}