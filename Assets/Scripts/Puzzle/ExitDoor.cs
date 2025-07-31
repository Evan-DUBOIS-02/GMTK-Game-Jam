using UnityEngine;

namespace Puzzle
{

    public class ExitDoor: MonoBehaviour
    {
        private bool _isSideDoor = false;
        
        public void ManageDoor(bool open)
        {
            if(_isSideDoor)
                transform.GetChild(1).gameObject.SetActive(!open);
            else
                transform.GetChild(0).gameObject.SetActive(!open);
        }

        public void SetSideRenderer(bool isLookingRight)
        {
            _isSideDoor = true;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = isLookingRight;
        }
    }
}