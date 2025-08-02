using Dungeon;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor.Experimental.GraphView;

public enum PuzzleType
{
    Lever,
    Void,
    PressurePlate,
    BreakableWall
}

namespace Puzzle
{
    public class PuzzleGenerator : MonoBehaviour
    {
        private int[,] _roomGrid;  
        private List<Room> _generatedRooms;
        private List<Room> _isolatedRoom;
        private List<Hallway> _hallways;
        private Room _exitRoom;
        private Room _startingRoom;
        
        [SerializeField] private List<Sprite> _availableRuneSprites;
        [SerializeField] private Sprite _blankRune;
        
        public void GeneratePuzzle(List<Room> generatedRoom, List<Hallway> hallways, int[,] roomGrid)
        {
            // Initialize variables
            _generatedRooms = generatedRoom;
            _hallways = hallways;
            _roomGrid = roomGrid;
            _isolatedRoom = new List<Room>();
            FindStartingAndExitRoom();
            
            // Generate main puzzle
            GenerateMainPuzzle();
            
            // Generate random puzzles
            List<PuzzleType> availablePuzzles = new List<PuzzleType>();
            foreach (PuzzleType puzzleType in Enum.GetValues(typeof(PuzzleType)))
                availablePuzzles.Add(puzzleType);

            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                if (availablePuzzles.Count == 0) break;

                int randomPuzzleIndex = Random.Range(0, availablePuzzles.Count);
                bool isGenerated = TryGeneratePuzzle(availablePuzzles[randomPuzzleIndex]);
                availablePuzzles.RemoveAt(randomPuzzleIndex);

                if (!isGenerated)
                    i--;
            }
        }

        private void GenerateMainPuzzle()
        {
            // Select 4 random runes
            List<Sprite> _selectedRuneSprites = new List<Sprite>();
            for (int i = 0; i < 4; i++)
            {
                Sprite randomSprite = _availableRuneSprites[Random.Range(0, _availableRuneSprites.Count)];
                while (_selectedRuneSprites.Contains(randomSprite))
                {
                    randomSprite = _availableRuneSprites[Random.Range(0, _availableRuneSprites.Count)];
                }
                _selectedRuneSprites.Add(randomSprite);
            }
            
            // Find correct repartition
            List<int> repartition = FindRuneRepartition();
            List<Sprite> totalRuneBag = new List<Sprite>();
            for (int i = 0; i < repartition.Count; i++)
            {
                for (int j = 0; j < repartition[i]; j++)
                {
                    totalRuneBag.Add(_selectedRuneSprites[i]);
                }
            }

            foreach (var rune in totalRuneBag)
            {
                Debug.Log(rune);
            }
        }

        private List<int> FindRuneRepartition()
        {
            List<int> randomNumbersOfRune =  new List<int>(){1, 2, 3, 4, 5, 6, 7, 8, 9};
            List<int> repartition = new List<int>();
            int totalNumberOfRunes = 0;
            for (int i = 0; i < 4; i++)
            {
                int randomIndex = Random.Range(0, randomNumbersOfRune.Count);
                int randomValue = randomNumbersOfRune[randomIndex];
                randomNumbersOfRune.RemoveAt(randomIndex);
                repartition.Add(randomValue);
                totalNumberOfRunes += randomValue;
            }

            if (totalNumberOfRunes > 16)        // max 16 runes for 4 rooms
                return FindRuneRepartition();
            
            repartition.Add(0); // blank rune
            
            while (totalNumberOfRunes % 4 != 0)
            {
                repartition[repartition.Count-1]++;
                totalNumberOfRunes++;
            }
            
            return repartition;
        }

        public bool TryGeneratePuzzle(PuzzleType type)
        {
            bool success;

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
            // Place door in random hallway
            bool isDoorPlaced = false;
            Door generatedDoor = null;
            do
            {
                Hallway hallway = _hallways[Random.Range(0, _hallways.Count)];
                if (!hallway.ContainObstacle)
                {
                    generatedDoor = hallway.GenerateDoor();
                    isDoorPlaced = generatedDoor != null;
                }
            }while(!isDoorPlaced);
            
            // Place lever in random available room
            bool isLeverPlaced = false;
            do
            {
                Room room = _generatedRooms[Random.Range(0, _generatedRooms.Count)];
                Queue<Room> rooms = new Queue<Room>();
                rooms.Enqueue(_startingRoom);
                if (room != _exitRoom && room!= _startingRoom && ExistPath(rooms, new List<Room>(), room))
                {
                    room.ContainLever(generatedDoor);
                    isLeverPlaced = true;
                }
            } while (!isLeverPlaced);
            return true;
        }

        private void FindStartingAndExitRoom()
        {
            foreach (Room room in _generatedRooms)
            {
                if (room.IsStartingRoom)
                    _startingRoom = room;
                else if(room.IsExitRoom)
                    _exitRoom = room;
                
                if(room.NumberOfAdjacentRoom == 1)
                    _isolatedRoom.Add(room);
            }
        }

        private bool ExistPath(Queue<Room> startingRooms, List<Room> marquedRoom, Room end)
        {
            if (startingRooms.Contains(end))
                return true;
            if (startingRooms.Count == 0)
                return false;
            
            Room roomToAnalyse =  startingRooms.Dequeue();
            marquedRoom.Add(roomToAnalyse);
            
            foreach (var hallway in roomToAnalyse.ConnectedHallways)
            {
                if (!hallway.ContainObstacle)
                {
                    Room other = hallway.GetOtherRoom(roomToAnalyse);
                    if(!marquedRoom.Contains(other))
                        startingRooms.Enqueue(other);
                }
            }
            
            return ExistPath(startingRooms, marquedRoom, end);
        }
    }
}
