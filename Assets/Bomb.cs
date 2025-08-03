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
        public Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            Debug.Log("exploding status : " + _isExploding);
        }

        public void SetToDefaultState()
        {
            gameObject.SetActive(true);
            transform.position = initialPosition;
            _isExploding = false;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        public int Interact(Player.PlayerState state)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            return 2;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isExploding && collision.GetComponent<BreakableWall>())
            {
                Debug.Log("Collision breakable wall");
                collision.gameObject.GetComponent<BreakableWall>().IsDisabled();
                rendererBomb.SetActive(false);
            }
            if(_isExploding && collision.CompareTag("Player"))
            {
                Debug.Log("Collision player");
                rendererBomb.SetActive(false);
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Direction from bomb to player
                    Vector2 direction = (rb.position - (Vector2)transform.position).normalized;

                    // Apply force
                    rb.AddForce(direction * _explosionForce);
                }
                rendererBomb.SetActive(false);
            }
        }
    }
}
