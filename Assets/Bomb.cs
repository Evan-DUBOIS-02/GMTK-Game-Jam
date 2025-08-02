using System;
using UnityEngine;

namespace Puzzle
{

    public class Bomb : MonoBehaviour, Interactable
    {
        [SerializeField]
        //public float _timeUntilExplode;
        public bool _isExploding = false;
        private float _explosionForce = 200f;
        public GameObject rendererBomb;

        public void SetToDefaultState()
        {
            gameObject.SetActive(true);
        }

        public int Interact(Player.PlayerState state)
        {
            rendererBomb.SetActive(false);
            return 2;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isExploding && collision.GetComponent<BreakableWall>())
            {
                collision.gameObject.GetComponent<BreakableWall>().IsDestroyed();
                rendererBomb.SetActive(false);
            }
            if(_isExploding && collision.CompareTag("Player"))
            {
                rendererBomb.SetActive(false);
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Direction from bomb to player
                    Vector2 direction = (rb.position - (Vector2)transform.position).normalized;

                    // Apply force
                    rb.AddForce(direction * _explosionForce);
                }
            }
        }
    }
}
