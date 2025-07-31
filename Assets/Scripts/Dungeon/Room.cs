using UnityEngine;

namespace Dungeon
{
    public class Room: MonoBehaviour
    {
        [Header("Door references")]
        [SerializeField] private GameObject _upDoor;
        [SerializeField] private GameObject _downDoor;
        [SerializeField] private GameObject _rightDoor;
        [SerializeField] private GameObject _leftDoor;
        
        [Header("Exit")]
        [SerializeField] private Material _exitDoorMaterial;

        private Vector2Int _roomIndex;
        public Vector2Int RoomIndex{get{return _roomIndex;} set{_roomIndex = value;}}

        public void OpenDoor(Vector2Int direction)
        {
            if(direction == Vector2Int.left)
                _leftDoor.SetActive(false);
            else if(direction == Vector2Int.right)
                _rightDoor.SetActive(false);
            else if(direction == Vector2Int.down)
                _downDoor.SetActive(false);
            else if(direction == Vector2Int.up)
                _upDoor.SetActive(false);
        }

        public void SetAsExitRoom()
        {
            // Place door on one of the closed door
            if (_upDoor.activeSelf)
                _upDoor.GetComponent<SpriteRenderer>().material = _exitDoorMaterial;
            else if(_rightDoor.activeSelf)
                _rightDoor.GetComponent<SpriteRenderer>().material = _exitDoorMaterial;
            else if(_downDoor.activeSelf)
                _downDoor.GetComponent<SpriteRenderer>().material = _exitDoorMaterial;
            else if(_leftDoor.activeSelf)
                _leftDoor.GetComponent<SpriteRenderer>().material = _exitDoorMaterial;
        }
    }
}