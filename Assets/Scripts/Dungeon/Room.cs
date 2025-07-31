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
        private GameObject _exitDoor;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _exitDoorPrefab;
        [SerializeField] private GameObject _leverPrefab;

        private Vector2Int _roomIndex;
        public Vector2Int RoomIndex{get{return _roomIndex;} set{_roomIndex = value;}}

        private bool _containPuzzleElement = false;
        public bool ContainPuzzleElement{get{return _containPuzzleElement;}}

        private bool _isStartingRoom = false;
        public bool IsStartingRoom{get{return _isStartingRoom;} set{_isStartingRoom = value;}}

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
    
        #region Exit door
        public void SetAsExitRoom()
        {
            // Place door on one of the closed door
            if (_upDoor.activeSelf)
                _exitDoor = _upDoor;
            else if(_rightDoor.activeSelf)
                _exitDoor = _rightDoor;
            else if(_downDoor.activeSelf)
                _exitDoor = _downDoor;
            else if(_leftDoor.activeSelf)
                _exitDoor = _leftDoor;
            
            _exitDoor.GetComponent<SpriteRenderer>().material = _exitDoorMaterial;
            Puzzle.ExitDoor exitDoorScript = _exitDoor.AddComponent<Puzzle.ExitDoor>();
            exitDoorScript.ManageDoor(true);
        }

        public Puzzle.ExitDoor GetExitDoor()
        {
            if(_exitDoor != null)
                return _exitDoor.GetComponent<Puzzle.ExitDoor>();
            return null;
        }
        
        #endregion

        public void ContainLever(Puzzle.ExitDoor exitDoor)
        {
            _containPuzzleElement = true;
            GameObject lever = Instantiate(_leverPrefab, transform.position, Quaternion.identity);
            lever.transform.parent = transform;
            lever.GetComponent<Puzzle.Lever>().Door = exitDoor;
        }
    }
}