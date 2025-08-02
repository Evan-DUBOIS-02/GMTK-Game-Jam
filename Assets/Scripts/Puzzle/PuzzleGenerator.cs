using Dungeon;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering;

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
        private Dictionary<Room, Hallway> _solutionToObstacle;
        
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
            _solutionToObstacle = new Dictionary<Room, Hallway>();
            List<PuzzleType> availablePuzzles = new List<PuzzleType>();
            foreach (PuzzleType puzzleType in Enum.GetValues(typeof(PuzzleType)))
                availablePuzzles.Add(puzzleType);

            for (int i = 0; i < 3; i++)
            {
                if (availablePuzzles.Count == 0) break;
                
                int randomPuzzleIndex = Random.Range(0, availablePuzzles.Count);
                bool isGenerated = TryGeneratePuzzle(availablePuzzles[randomPuzzleIndex]);
                availablePuzzles.RemoveAt(randomPuzzleIndex);

                if (!isGenerated)
                    i--;
            }
        }
        
        #region Main puzzle generation
        private void GenerateMainPuzzle()
        {
            // Select 4 random runes sprites
            List<Sprite> selectedRuneSprites = new List<Sprite>();
            for (int i = 0; i < 4; i++)
            {
                Sprite randomSprite = _availableRuneSprites[Random.Range(0, _availableRuneSprites.Count)];
                while (selectedRuneSprites.Contains(randomSprite))
                {
                    randomSprite = _availableRuneSprites[Random.Range(0, _availableRuneSprites.Count)];
                }
                selectedRuneSprites.Add(randomSprite);
            }
            
            // Find correct repartition
            List<int> repartition = FindRuneRepartition();
            List<Sprite> totalRuneBag = new List<Sprite>();
            for (int i = 0; i < repartition.Count; i++)
            {
                for (int j = 0; j < repartition[i]; j++)
                {
                    if(i != 4)
                        totalRuneBag.Add(selectedRuneSprites[i]);
                    else
                        totalRuneBag.Add(_blankRune);
                }
            }

            Dictionary<Sprite, int> spriteToQuantity = new Dictionary<Sprite, int>()
            {
                { selectedRuneSprites[0], repartition[0] },
                { selectedRuneSprites[1], repartition[1] },
                { selectedRuneSprites[2], repartition[2] },
                { selectedRuneSprites[3], repartition[3] }
            };

            // Find solution room
            List<Room> isolatedRoomTmp = new List<Room>(_isolatedRoom);
            isolatedRoomTmp.Remove(_exitRoom);
            isolatedRoomTmp.Remove(_startingRoom);
            
            Room solutionRoom =  null;
            float minDistance = float.MaxValue;
            
            for (int i = 0; i < isolatedRoomTmp.Count; i++)
            {
                float currentDistance = Vector2.Distance(isolatedRoomTmp[i].RoomIndex, _exitRoom.RoomIndex);
                if (currentDistance <= minDistance)
                {
                    minDistance = currentDistance;
                    solutionRoom = isolatedRoomTmp[i];
                }
            }

            isolatedRoomTmp.Remove(solutionRoom);
            solutionRoom.SetAsSolutionRoom(spriteToQuantity, _exitRoom.ExitDoor);
            
            // Find hint rooms
            int numberOfRoom = totalRuneBag.Count/4;
            
            // If to much rooms, remove random
            while (isolatedRoomTmp.Count > numberOfRoom)
            {
                isolatedRoomTmp.RemoveAt(Random.Range(0, isolatedRoomTmp.Count));
            }

            // If not enough rooms, add randoms
            while (isolatedRoomTmp.Count < numberOfRoom)
            {
                Room randomRoom = _generatedRooms[Random.Range(0, _generatedRooms.Count)];
                while (isolatedRoomTmp.Contains(randomRoom) || randomRoom.IsExitRoom || randomRoom.IsStartingRoom ||
                       randomRoom.IsSolutionRoom)
                {
                    randomRoom = _generatedRooms[Random.Range(0, _generatedRooms.Count)];
                }

                isolatedRoomTmp.Add(randomRoom);
            }

            foreach (Room room in isolatedRoomTmp)
            {
                List<Sprite> selectedSpriteForThisRoom = new List<Sprite>();
                
                // Get random 4 available runes
                for (int i = 0; i < 4; i++)
                {
                    int randomIndex = Random.Range(0, totalRuneBag.Count);
                    Sprite randomSprite = totalRuneBag[randomIndex];
                    totalRuneBag.RemoveAt(randomIndex);
                    selectedSpriteForThisRoom.Add(randomSprite);
                }

                room.SetAsHintRoom(selectedSpriteForThisRoom);
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
        #endregion

        public bool TryGeneratePuzzle(PuzzleType type)
        {
            // ===
            // Generate obstacle
            // ===
            Hallway hallwayObstacle = null;
            bool isValid = false;
            
            do
            {
                hallwayObstacle = _hallways[Random.Range(0, _hallways.Count)];
                isValid = IsValidObstaclePosition(hallwayObstacle);
            } while (!isValid);
            
            // ===
            // Generate solution
            // ===
            List<Room> generatedRoomsTmp = new List<Room>(_generatedRooms);
            generatedRoomsTmp.Remove(_exitRoom);
            generatedRoomsTmp.Remove(_startingRoom);
            // remove all solutions rooms
            foreach (var elem in _solutionToObstacle)
                generatedRoomsTmp.Remove(elem.Key);
            // remove solution room
            for(int i = 0; i < generatedRoomsTmp.Count; i++)
                if (generatedRoomsTmp[i].IsSolutionRoom)
                {
                    generatedRoomsTmp.Remove(generatedRoomsTmp[i]);
                    break;
                }
            
            Room solutionRoom = null;
            isValid = false;
            while (!isValid)
            {
                solutionRoom = generatedRoomsTmp[Random.Range(0, generatedRoomsTmp.Count)];
                
                // Start queue
                Queue<Room> startingRooms = new Queue<Room>();
                startingRooms.Enqueue(_startingRoom);
                // All obstacles list
                List<Hallway> obstacles = _solutionToObstacle.Values.ToList();
                obstacles.Add(hallwayObstacle);
                
                // Direct access between player and solution
                if(ExistPath(startingRooms, solutionRoom, obstacles, new List<Room>()))
                    isValid = true;
                // Other case
                else
                {
                    startingRooms = new Queue<Room>();
                    startingRooms.Enqueue(_startingRoom);
                    // Blocked by new generated obstacle
                    if (!ExistPath(startingRooms, solutionRoom, new List<Hallway>() { hallwayObstacle },
                            new List<Room>()))
                    {
                        generatedRoomsTmp.Remove(solutionRoom);
                        isValid = false;
                    }
                    // Cross puzzle case
                    else
                    {
                        foreach (var elem in _solutionToObstacle)
                        {
                            startingRooms = new Queue<Room>();
                            startingRooms.Enqueue(elem.Key);
                            if (!ExistPath(startingRooms, elem.Value.Room1, new List<Hallway>() { hallwayObstacle },
                                    new List<Room>()))
                            {
                                generatedRoomsTmp.Remove(solutionRoom);
                                isValid = false;
                            }
                        }

                        isValid = true;
                    }
                }
            }
            
            _solutionToObstacle.Add(solutionRoom, hallwayObstacle);

            switch (type)
            {
                case PuzzleType.Lever:
                    GenerateLeverPuzzle(hallwayObstacle, solutionRoom);
                    break;

                case PuzzleType.BreakableWall:
                    GenerateBreakableWallPuzzle(hallwayObstacle, solutionRoom);
                    break;
            }
            return true;
        }

        private bool IsValidObstaclePosition(Hallway hallwayObstacle)
        {
            if (hallwayObstacle.Room1.IsExitRoom || hallwayObstacle.Room2.IsExitRoom)
                return false;
            // If starting room is isolated, don't place door in the unique connected hallway
            if (hallwayObstacle.Room1.IsStartingRoom && hallwayObstacle.Room1.NumberOfAdjacentRoom == 1)
                return false;
            if (hallwayObstacle.Room2.IsStartingRoom && hallwayObstacle.Room2.NumberOfAdjacentRoom == 1)
                return false;
            
            return true;
        }

        private void GenerateLeverPuzzle(Hallway hallwayObstacle, Room room)
        {
            Door generatedDoor = hallwayObstacle.GenerateDoor();
            room.ContainLever(generatedDoor);
        }

        private void GenerateBreakableWallPuzzle(Hallway hallwayObstacle, Room room)
        {
            BreakableWall breakableWall = hallwayObstacle.GenerateBreakableWall();
            room.ContainBomb();
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

        private bool ExistPath(Queue<Room> startingRooms, Room end, List<Hallway> obstacles, List<Room> marquedRoom)
        {
            if (startingRooms.Contains(end))
                return true;
            if (startingRooms.Count == 0)
                return false;
            
            Room roomToAnalyse =  startingRooms.Dequeue();
            marquedRoom.Add(roomToAnalyse);
            
            foreach (var hallway in roomToAnalyse.ConnectedHallways)
            {
                if (!obstacles.Contains(hallway))
                {
                    Room other = hallway.GetOtherRoom(roomToAnalyse);
                    if(!marquedRoom.Contains(other))
                        startingRooms.Enqueue(other);
                }
            }
            
            return ExistPath(startingRooms, end, obstacles, marquedRoom);
        }
    }
}
