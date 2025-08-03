using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Puzzle;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public Door ExitDoor{get{return _exitDoor.GetComponent<Door>();}}
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _leverPrefab;
        
        [Header("Solution room slabs")]
        [SerializeField] private List<GameObject> _solutionRoomSlabs;
        
        [Header("Hint room tiles")]
        [SerializeField] private List<GameObject> _topHintRoomTiles;
        [SerializeField] private List<GameObject> _rightHintRoomTiles;
        [SerializeField] private List<GameObject> _botHintRoomTiles;
        [SerializeField] private List<GameObject> _leftHintRoomTiles;
        [SerializeField] private GameObject _bombPrefab;
        
        [Header("Level exit")]
        [SerializeField] private GameObject _exitLevel;

        private Vector2Int _roomIndex;
        public Vector2Int RoomIndex{get{return _roomIndex;} set{_roomIndex = value;}}

        private bool _containPuzzleElement;
        public bool ContainPuzzleElement{get{return _containPuzzleElement;}}
        
        [NonSerialized] public bool IsStartingRoom;
        [NonSerialized] public bool IsExitRoom;
        [NonSerialized] public bool IsSolutionRoom;

        private int _numberOfAdjacentRoom = 0;
        public int NumberOfAdjacentRoom{get{return _numberOfAdjacentRoom;}}
        
        [NonSerialized] public List<Hallway> ConnectedHallways;
        [SerializeField] private GameObject _frontTorch;
        [SerializeField] private GameObject _sideTorch;
        private GameObject _generatedTopTorchLight;
        private GameObject _generatedLeftTorchLight;
        private GameObject _generatedRightTorchLight;

        private void Start()
        {
            
            if (Random.value <= 0.5f) // need to generate torch litgh ?
            {
                List<int> _availablePositions = new List<int>();
                if(_upWall.activeSelf)
                    _availablePositions.Add(0);
                if(_rightWall.activeSelf)
                    _availablePositions.Add(1);
                if(_leftWall.activeSelf)
                    _availablePositions.Add(2);
                if (_availablePositions.Count == 0)
                    return;
                int finalPosition = _availablePositions[Random.Range(0, _availablePositions.Count)];
                switch (finalPosition)
                {
                    case 0:
                        _generatedTopTorchLight = Instantiate(_frontTorch, transform.position, Quaternion.identity);
                        _generatedTopTorchLight.transform.parent = transform;
                        _generatedTopTorchLight.transform.position = new Vector3(
                            transform.position.x + Random.Range(-1.0f, 1.0f),
                            transform.position.y + 2.5f,
                            0);
                        break;
                    case 1:
                        _generatedRightTorchLight = Instantiate(_sideTorch, transform.position, Quaternion.identity);
                        _generatedRightTorchLight.transform.parent = transform;
                        _generatedRightTorchLight.transform.position = new Vector3(
                            transform.position.x + 1.5f,
                            transform.position.y + Random.Range(-1.0f, 1.0f),
                            0);
                        _generatedRightTorchLight.GetComponent<Animator>().SetBool("IsRight", true);
                        break;
                    case 2:
                        _generatedLeftTorchLight = Instantiate(_sideTorch, transform.position, Quaternion.identity);
                        _generatedLeftTorchLight.transform.parent = transform;
                        _generatedLeftTorchLight.transform.position = new Vector3(
                            transform.position.x - 1.5f,
                            transform.position.y + Random.Range(-1.0f, 1.0f),
                            0);
                        break;
                }
            }
            
        }

        public void OpenWall(Vector2Int direction)
        {
            _numberOfAdjacentRoom++;
            if(direction == Vector2Int.left)
                _leftWall.SetActive(false);
            else if (direction == Vector2Int.right)
                _rightWall.SetActive(false);
            else if (direction == Vector2Int.down)
                _downWall.SetActive(false);
            else if (direction == Vector2Int.up)
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
            IsExitRoom = true;
            _exitLevel.SetActive(true);
            _exitDoor.transform.SetParent(transform);
        }

        public void SetAsSolutionRoom(Dictionary<Sprite, int> sprites, Door door)
        {
            for (int i = 0; i < _solutionRoomSlabs.Count; i++)
            {
                _solutionRoomSlabs[i].GetComponent<SpriteRenderer>().sprite = sprites.ElementAt(i).Key;
                _solutionRoomSlabs[i].GetComponent<RuneSlabs>().SetCount(sprites.ElementAt(i).Value);
            }
            _solutionRoomSlabs[0].transform.parent.gameObject.SetActive(true);
            _solutionRoomSlabs[0]?.transform.parent.GetComponent<RuneCodeManager>().SetExitDoor(door);
            IsSolutionRoom = true;
        }

        public void SetAsHintRoom(List<Sprite> sprites)
        {
            // Can place runes at the top ?
            if(_upWall.activeSelf)
                ApplySpriteOnGameObject(sprites, _topHintRoomTiles);
            else if(_downWall.activeSelf)
                ApplySpriteOnGameObject(sprites, _botHintRoomTiles);
            else if(_rightWall.activeSelf)
                ApplySpriteOnGameObject(sprites, _rightHintRoomTiles);
            else if(_leftWall.activeSelf)
                ApplySpriteOnGameObject(sprites, _leftHintRoomTiles);
        }

        private void ApplySpriteOnGameObject(List<Sprite> sprites, List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].GetComponent<SpriteRenderer>().sprite = sprites[i];
            }
            gameObjects[0].transform.parent.gameObject.SetActive(true);
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

        public void ContainBomb(BreakableWall wall)
        {
            _containPuzzleElement = true;
            GameObject bomb = Instantiate(_bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.parent = transform;
            bomb.GetComponent<Bomb>().Wall = wall;
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterInteractable(bomb.GetComponent<Puzzle.Bomb>());
        }
    }
}