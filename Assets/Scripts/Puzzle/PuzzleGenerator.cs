using Dungeon;
using System.Collections.Generic;
using System;
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
        private Room _exitRoom;
        private Room _startingRoom;
        
        public void GeneratePuzzle(List<Room> generatedRoom, int[,] roomGrid)
        {
            _generatedRooms = generatedRoom;
            _roomGrid = roomGrid;
            FindStartingAndExitRoom();
            
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
            float maxDistance = float.MinValue;
            Room selectedRoom = null;

            foreach (Room room in _generatedRooms)
            {
                if (Vector2.Distance(room.RoomIndex, _exitRoom.RoomIndex) > maxDistance && !room.ContainPuzzleElement && !room.IsStartingRoom)
                {
                    maxDistance = Vector2.Distance(room.RoomIndex, new Vector2Int(_roomGrid.GetLength(0) / 2, _roomGrid.GetLength(1) / 2));
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

        private void FindStartingAndExitRoom()
        {
            foreach (Room room in _generatedRooms)
            {
                if (room.IsStartingRoom)
                    _startingRoom = room;
                else if(room.IsExitRoom)
                    _exitRoom = room;
            }
        }
    }
}
