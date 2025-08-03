using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle
{
    public class RuneSlabs: MonoBehaviour
    {
        [SerializeField] private RuneCodeManager runeCodeManager;
        private int _count;

        public void SetCount(int value)
        {
            _count = value;
        }

        public void EnableSlab()
        {
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("OnTriggerEnter2D "+gameObject.name);
            if (collision.GetComponent<Player.PlayerManager>())
            {
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                runeCodeManager.RegisterInput(_count, this);
            }
        }
    }
}