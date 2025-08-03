using UnityEngine;

namespace Puzzle
{

    public class BreakableWall : MonoBehaviour
    {
        [SerializeField] private GameObject _breakableWallVertical;
        [SerializeField] private GameObject _breakableWallHorizontal;

        private bool _isHorizontal = false;

        public void IsDisabled()
        {
            if (!_isHorizontal)
            {
                _breakableWallVertical.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                _breakableWallHorizontal.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public void SetSideRenderer()
        {
            _isHorizontal = true;
            _breakableWallVertical.SetActive(false);
            _breakableWallHorizontal.SetActive(true);
        }
    }
}
