using UnityEngine;

namespace Puzzle
{

    public class BreakableWall : MonoBehaviour
    {
        [SerializeField] private GameObject _breakableWall;

        public void IsDestroyed()
        {
            _breakableWall.SetActive(false);
        }
    }
}
