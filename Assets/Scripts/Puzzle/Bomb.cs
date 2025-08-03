using System;
using UnityEngine;

namespace Puzzle
{

    public class Bomb : MonoBehaviour, Interactable
    {
        private float _explosionForce = 2000f;
        [NonSerialized] public bool _isExploding;
        [NonSerialized] public BreakableWall Wall;

        public void SetToDefaultState()
        {
            gameObject.SetActive(true);
            Wall.ManageWall(false);
        }

        public int Interact(Player.PlayerState state)
        {
            gameObject.SetActive(false);
            return 2;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isExploding)
            {
                bool canExplode = false;
                if (collision.CompareTag("BreakableWall"))
                {
                    collision.gameObject.GetComponentInParent<BreakableWall>().ManageWall(true);
                    canExplode = true;
                }

                if (collision.CompareTag("Player"))
                {
                    Debug.Log("Collision player");
                    Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        // Direction from bomb to player
                        Vector2 direction = (rb.position - (Vector2)transform.position).normalized;

                        // Apply force
                        Debug.Log("add force");
                        rb.AddForce(direction * _explosionForce, ForceMode2D.Impulse);
                    }

                    canExplode = true;
                }
                
                if(canExplode)
                    Destroy(gameObject);
            }
        }
    }
}
