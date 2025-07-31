using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum PuzzleType
{
    Lever,
    Void,
    PressurePlate,
    BreakableWall
}

namespace Dungeon
{
    public class Generator: MonoBehaviour
    {
        [Header("Rooms parameters")]
        [SerializeField] private GameObject _roomPrefab;    // The room prfab to instantiate
        [SerializeField] private int _roomWidth = 3;        // Save this in room ?
        [SerializeField] private int _roomHeight = 3;       // Save this in room ?
        
        [Header("Number of rooms")]
        [SerializeField] private int _minRooms = 10;        // Minimum number of room to generate
        [SerializeField] private int _maxRooms = 15;        // Maximum number of toom to generate
        
        [Header("Grid size")]
        [SerializeField] private int _gridSizeX = 11;       // the overall X amplitude of the dungeon
        [SerializeField] private int _gridSizeY = 11;       // the overall Y amplitude of the dungeon
        
        private Queue<Vector2Int> _roomQueue;               // Queue of room to try expension
        private int[,] _roomGrid;                           // In the grid, 0: no room, 1: room at the indicated position
        private int _roomCount;                             // Room counter
        private bool _generationComplete;                   // Catch the end of the generation
        private List<Room> _generatedRooms;
        private Room _exitRoom;
        
        private void Start()
        {
            // Generate the dungeon
            Generate();
        }
        
        /// <summary>
        /// Used to generate the dungeon
        /// </summary>
        public void Generate()
        {
            // Clear existing rooms
            foreach (var go in GameObject.FindGameObjectsWithTag("DungeonElement"))
            {
                DestroyImmediate(go);
            }
            
            if(_generatedRooms == null)
                _generatedRooms = new List<Room>();
            else
                _generatedRooms.Clear();
            
            // STEP 0: Initialize
            _roomGrid = new int[_gridSizeX, _gridSizeY];
            _roomQueue =  new Queue<Vector2Int>();
            _roomCount = 0;
            _generationComplete = false;
            
            // STEP 1: Place the first room at the middle of the grid
            Vector2Int initialRoomIndex = new Vector2Int(_gridSizeX/2, _gridSizeY/2);
            GenerateRoom(initialRoomIndex);
            
            // STEP 2: Iterate
            while (!_generationComplete)
            {
                // If we generate a new room at the previous iteration
                // and we don't reach the maximum of room
                // and the generation is not complete
                if (_roomQueue.Count > 0 && _roomCount < _maxRooms && !_generationComplete)
                {
                    // Get the last generated room coord
                    Vector2Int roomIndex = _roomQueue.Dequeue();
                    int gridX = roomIndex.x;
                    int gridY = roomIndex.y;
                    
                    // Try to generate a room at the right
                    if(gridX > 0)
                        TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
                    // Try to generate a room at the left 
                    if(gridX < _gridSizeX - 1)
                        TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
                    // Try to generate a room at the bottom
                    if(gridY > 0)
                        TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
                    // Try to generate a room at the top
                    if(gridY < _gridSizeY - 1)
                        TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
                }
                else if(_roomCount < _minRooms) // if we don't have generated room and we don't reach the minimum number of room to generate
                {
                    Generate(); // Generate again
                    return;
                }
                else if (!_generationComplete)  // if we don't have generated room and we reach the minimum number of room or if we reach the maximum
                    _generationComplete = true; // Stop generation
            }
            
            // STEP 3: Instantiate the rooms
            InstantiateRooms();
            
            // STEP 4: Place exit
            GenerateExit();
            
            // STEP 5: Setup random puzzle
            List<PuzzleType> availablePuzzles = new List<PuzzleType>();
            foreach(PuzzleType puzzleType in Enum.GetValues(typeof(PuzzleType)))
                availablePuzzles.Add(puzzleType);
            
            Debug.Log("==== Puzzle generation ====");
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                if (availablePuzzles.Count == 0) break;
                
                int randomPuzzleIndex = Random.Range(0, availablePuzzles.Count);
                bool isGenerated = TryGeneratePuzzle(availablePuzzles[randomPuzzleIndex]);
                Debug.Log(availablePuzzles[randomPuzzleIndex].ToString());
                availablePuzzles.RemoveAt(randomPuzzleIndex);

                if (!isGenerated)
                    i--;
            }
        }
        
        #region STEP 1 - 3 (room generation)
        /// <summary>
        /// Used to generate a room at the given position
        /// </summary>
        /// <param name="roomIndex">The coord of the room to generate</param>
        private void GenerateRoom(Vector2Int roomIndex)
        {
            // Add the room in the new generated room list
            _roomQueue.Enqueue(roomIndex);
            // Save it in the grid
            _roomGrid[roomIndex.x, roomIndex.y] = 1;
            // Increase the number of room
            _roomCount++;
        }
        
        /// <summary>
        /// Used to try a generation of a room at the given position (adjacent to, at least, another one)
        /// </summary>
        /// <param name="roomIndex">The coord of the room to try the generation</param>
        private void TryGenerateRoom(Vector2Int roomIndex)
        {
            // If we reach the maximum number of room, exit
            if (_roomCount >= _maxRooms)
                return;
            // Count the number of adjacent room to avoid big squares (maximum 2 adjacent rooms)
            if (CountAdjacentRooms(roomIndex) > 1)
                return;
            // 50% to generate the room
            if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
                return;
            // Generate the room
            GenerateRoom(roomIndex);
        }

        /// <summary>
        /// Used to instantiate all the room from the grid info
        /// </summary>
        private void InstantiateRooms()
        {
            for(int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _gridSizeY; y++)
            {
                if (_roomGrid[x, y] != 0)
                {
                    // Instantiate the room prefab at the world position
                    var room = Instantiate(_roomPrefab, GetPositionFromGridIndex(new Vector2Int(x, y)), Quaternion.identity);
                    room.name = $"Room-{x}-{y}";
                    room.transform.SetParent(transform);
                    
                    // Check the adjacents room to open the doors if necessary
                    Room structure = room.GetComponent<Room>();
                    structure.RoomIndex = new Vector2Int(x, y);
                    // Check if room exist at the left and open the door if necessary
                    if (x > 0 && _roomGrid[x - 1, y] != 0) 
                        structure.OpenDoor(Vector2Int.left);
                    // Check if room exist at the right and open the door if necessary
                    if (x < _gridSizeX - 1 && _roomGrid[x + 1, y] != 0) 
                        structure.OpenDoor(Vector2Int.right);
                    // Check if room exist at the bottom and open the door if necessary
                    if (y > 0 && _roomGrid[x, y - 1] != 0) 
                        structure.OpenDoor(Vector2Int.down);
                    // Check if room exist at the top and open the door if necessary
                    if (y < _gridSizeY - 1 && _roomGrid[x, y + 1] != 0) 
                        structure.OpenDoor(Vector2Int.up);
                    
                    _generatedRooms.Add(structure);
                    if (x == _gridSizeX / 2 && y == _gridSizeY / 2)
                        structure.IsStartingRoom = true;
                }
            }
        }
        #endregion
        
        #region STEP 4 (puzzle placement)
        private void GenerateExit()
        {
            float maxDistance = float.MinValue;
            _exitRoom = null;
            
            foreach (var room in _generatedRooms)
            {
                if (Vector2.Distance(room.RoomIndex, new Vector2Int(_gridSizeX/2, _gridSizeY/2)) > maxDistance)
                {
                    maxDistance = Vector2.Distance(room.RoomIndex, new Vector2Int(_gridSizeX/2, _gridSizeY/2));
                    _exitRoom = room;
                }
            }
            _exitRoom.SetAsExitRoom();
        }

        private bool TryGeneratePuzzle(PuzzleType type)
        {
            bool success = false;
            
            switch (type)
            {
                case PuzzleType.Lever:
                    success = TryGenerateLever();
                    break;
                default:
                    success = false;
                    break;
            }
            
            return success;
        }

        private bool TryGenerateLever()
        {
            float maxDistance = float.MinValue;
            Room selectedRoom = null;
            
            foreach (Room room in _generatedRooms)
            {
                if (Vector2.Distance(room.RoomIndex, _exitRoom.RoomIndex) > maxDistance && !room.ContainPuzzleElement && !room.IsStartingRoom)
                {
                    maxDistance = Vector2.Distance(room.RoomIndex, new Vector2Int(_gridSizeX/2, _gridSizeY/2));
                    selectedRoom = room;
                }
            }

            if (selectedRoom != null)
            {
                selectedRoom.ContainLever(_exitRoom.GetExitDoor());
                _exitRoom.GetExitDoor().ManageDoor(false);
                return true;
            }
            
            return false;
        }
        
        #endregion

        #region TOOLS
        
        /// <summary>
        /// Used to return the current number of existing rooms adjacent at the given positions
        /// </summary>
        /// <param name="roomIndex">The position to check adjacent rooms</param>
        /// <returns>The number of adjacent rooms</returns>
        private int CountAdjacentRooms(Vector2Int roomIndex)
        {
            int x =   roomIndex.x;
            int y =   roomIndex.y;
            int count = 0;
            
            // Check left
            if (x > 0 && _roomGrid[x - 1, y] != 0) count++;
            // Check right
            if (x < _gridSizeX - 1 && _roomGrid[x + 1, y] != 0) count++;
            // Check bottom
            if (y > 0 && _roomGrid[x, y - 1] != 0) count++;
            // Check top
            if (y < _gridSizeY - 1 && _roomGrid[x, y + 1] != 0) count++;

            return count;
        }

        /// <summary>
        /// Convert the given grid position to world position according to the rooms and the grid sizes
        /// </summary>
        /// <param name="gridIndex">The grid position to convert</param>
        /// <returns>The grid position converted to world position</returns>
        private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
        {
            int gridX = gridIndex.x;
            int gridY = gridIndex.y;

            return new Vector3(_roomWidth * (gridX - _gridSizeX / 2), _roomHeight * (gridY - _gridSizeY / 2)); 
        }
    
        /// <summary>
        /// Display the grid to visualize it
        /// </summary>
        private void OnDrawGizmos()
        {
            Color gizmoColor = new Color(1.0f, 1.0f, 0.5f);
            Gizmos.color = gizmoColor;

            for (int i = 0; i < _gridSizeX; i++)
            for (int j = 0; j < _gridSizeY; j++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(i, j));
                Gizmos.DrawWireCube(position, new Vector3(_roomWidth, _roomHeight, 0.1f));
            }
        }
        
        #endregion
        
    }
}