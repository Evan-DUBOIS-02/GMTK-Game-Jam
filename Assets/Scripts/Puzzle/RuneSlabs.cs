using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle
{
    public class RuneSlabs: MonoBehaviour
    {
        [SerializeField] private RuneCodeManager runeCodeManager;
        private int _count = 0;

        public void SetCount(int value)
        {
            _count = value;
        }

        public void EnableSlab()
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.PlayerManager>())
            {
                GetComponent<BoxCollider2D>().enabled = false;
                runeCodeManager.RegisterInput(_count);
            }
        }
    }
}