using UnityEngine;

namespace Puzzle
{

    public class BreakableWall : MonoBehaviour
    {
        [SerializeField] private GameObject _breakableWallVertical;
        [SerializeField] private GameObject _breakableWallHorizontal;

        private bool _isHorizontal = false;

        public void ManageWall(bool open)
        {
            if (_isHorizontal)
            {
                _breakableWallHorizontal.SetActive(!open);
            }
            else
            {
                _breakableWallVertical.SetActive(!open);
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
