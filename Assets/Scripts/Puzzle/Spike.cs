using Game;
using UnityEngine;

namespace Puzzle
{
    public class Spike: MonoBehaviour, Interactable
    {
        [SerializeField] private GameObject _activeSpikes;

        public int Interact(Player.PlayerState sate)
        {
            return -1;
        }

        public void SetToDefaultState()
        {
            _activeSpikes.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && _activeSpikes.activeSelf)
            {
                GameManager.Instance.StopLoop();
            }
            else
            {
                _activeSpikes.SetActive(false);
            }
        }
    }
}