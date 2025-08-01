using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Dungeon
{
    public class Room: MonoBehaviour
    {
        [Header("Walls")] 
        [SerializeField] private GameObject _upWall;
        [SerializeField] private GameObject _downWall;
        [SerializeField] private GameObject _rightWall;
        [SerializeField] private GameObject _leftWall;
        
        private GameObject _exitDoor;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _leverPrefab;

        private Vector2Int _roomIndex;
        public Vector2Int RoomIndex{get{return _roomIndex;} set{_roomIndex = value;}}

        private bool _containPuzzleElement;
        public bool ContainPuzzleElement{get{return _containPuzzleElement;}}

        private bool _isStartingRoom;
        public bool IsStartingRoom{get{return _isStartingRoom;} set{_isStartingRoom = value;}}
        
        private bool _isExitRoom;
        public bool IsExitRoom { get { return _isExitRoom; } set { _isExitRoom = value; } }

        public void OpenWall(Vector2Int direction)
        {
            if(direction == Vector2Int.left)
                _leftWall.SetActive(false);
            else if (direction == Vector2Int.right)
                _rightWall.SetActive(false);
            else if (direction == Vector2Int.down)
                _downWall.SetActive(false);
            else if(direction == Vector2Int.up)
                _upWall.SetActive(false);
        }
    
        #region Exit door
        public void SetAsExitRoom()
        {
            Puzzle.ExitDoor exitDoorScript = null;
            // Place door on one of the closed door
            if (!_upWall.activeSelf)
            {
                
                Vector3 position = transform.position;
                position.y += 3.5f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                exitDoorScript = _exitDoor.AddComponent<Puzzle.ExitDoor>();
            }
            else if (!_downWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.y -= 2.5f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                exitDoorScript = _exitDoor.AddComponent<Puzzle.ExitDoor>();
            }
            else if (!_leftWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.x -= 3f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                exitDoorScript = _exitDoor.AddComponent<Puzzle.ExitDoor>();
                exitDoorScript.SetSideRenderer(false);
            }
            else if (!_rightWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.x += 3f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                exitDoorScript = _exitDoor.AddComponent<Puzzle.ExitDoor>();
                exitDoorScript.SetSideRenderer(true);
            }
            _isExitRoom = true;
            _exitDoor.transform.SetParent(transform);
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
            if(GameManager.Instance != null)
                GameManager.Instance.RegisterInteractable(lever.GetComponent<Puzzle.Lever>());
        }
    }
}