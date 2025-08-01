using UnityEngine;

namespace Puzzle
{

    public class ExitDoor: MonoBehaviour
    {
        [SerializeField] private GameObject _frontDoorClosed;
        [SerializeField] private GameObject _frontDoorOpen;
        [SerializeField] private GameObject _sideDoorClosed;
        [SerializeField] private GameObject _sideDoorOpen;
        
        private bool _isSideDoor = false;
        
        public void ManageDoor(bool open)
        {
            if (_isSideDoor)
            {
                _sideDoorClosed.SetActive(!open);
                _sideDoorOpen.SetActive(open);
            }
            else
            {
                _frontDoorClosed.SetActive(!open);
                _frontDoorOpen.SetActive(open);
            }
        }

        public void SetSideRenderer(bool isLookingRight)
        {
            _isSideDoor = true;
            _frontDoorClosed.SetActive(false);
            _sideDoorClosed.SetActive(true);
            _sideDoorClosed.GetComponent<SpriteRenderer>().flipX = isLookingRight;
        }
    }
}