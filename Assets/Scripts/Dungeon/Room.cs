using System;
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

        public bool IsStartingRoom
        {
            get{return _isStartingRoom;}
            set
            {
                _isStartingRoom = value;
                GetComponentInChildren<AutomaticLight>().EnableLight();
            }
        }
        
        private bool _isExitRoom;
        public bool IsExitRoom { get { return _isExitRoom; } set { _isExitRoom = value; } }

        private int _numberOfAdjacentRoom = 0;
        public int NumberOfAdjacentRoom{get{return _numberOfAdjacentRoom;}}
        
        [NonSerialized] public List<Hallway> ConnectedHallways = new List<Hallway>();

        public void OpenWall(Vector2Int direction)
        {
            _numberOfAdjacentRoom++;
            if(direction == Vector2Int.left)
                _leftWall.SetActive(false);
            else if (direction == Vector2Int.right)
                _rightWall.SetActive(false);
            else if (direction == Vector2Int.down)
                _downWall.SetActive(false);
            else if(direction == Vector2Int.up)
                _upWall.SetActive(false);
        }

        public void AddHallway(Hallway hallway)
        {
            if(ConnectedHallways == null)
                ConnectedHallways = new List<Hallway>();
            if(!ConnectedHallways.Contains(hallway))
                ConnectedHallways.Add(hallway);
        }
        
        public void SetAsExitRoom()
        {
            Puzzle.Door doorScript = null;
            // Place door on one of the closed door
            if (!_upWall.activeSelf)
            {
                
                Vector3 position = transform.position;
                position.y += 3.5f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                doorScript = _exitDoor.GetComponent<Puzzle.Door>();
            }
            else if (!_downWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.y -= 2.5f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                doorScript = _exitDoor.GetComponent<Puzzle.Door>();
            }
            else if (!_leftWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.x -= 3f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                doorScript = _exitDoor.GetComponent<Puzzle.Door>();
                doorScript.SetSideRenderer(false);
            }
            else if (!_rightWall.activeSelf)
            {
                Vector3 position = transform.position;
                position.x += 3f;
                _exitDoor = Instantiate(_doorPrefab, position, Quaternion.identity);
                doorScript = _exitDoor.GetComponent<Puzzle.Door>();
                doorScript.SetSideRenderer(true);
            }
            _isExitRoom = true;
            _exitDoor.transform.SetParent(transform);
        }

        public void ContainLever(Puzzle.Door door)
        {
            _containPuzzleElement = true;
            GameObject lever = Instantiate(_leverPrefab, transform.position, Quaternion.identity);
            lever.transform.parent = transform;
            lever.GetComponent<Puzzle.Lever>().Door = door;
            if(GameManager.Instance != null)
                GameManager.Instance.RegisterInteractable(lever.GetComponent<Puzzle.Lever>());
        }
    }
}