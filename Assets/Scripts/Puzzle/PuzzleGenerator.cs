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
        public void GeneratePuzzle(List<Room> generatedRoom, int[,] roomGrid)
        {
            List<PuzzleType> availablePuzzles = new List<PuzzleType>();
            foreach (PuzzleType puzzleType in Enum.GetValues(typeof(PuzzleType)))
                availablePuzzles.Add(puzzleType);

            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                if (availablePuzzles.Count == 0) break;

                int randomPuzzleIndex = Random.Range(0, availablePuzzles.Count);
                bool isGenerated = TryGeneratePuzzle(availablePuzzles[randomPuzzleIndex], generatedRoom, roomGrid);
                availablePuzzles.RemoveAt(randomPuzzleIndex);

                if (!isGenerated)
                    i--;
            }
        }

        public bool TryGeneratePuzzle(PuzzleType type, List<Room> generatedRoom, int[,] roomGrid)
        {
            bool success = false;

            switch (type)
            {
                case PuzzleType.Lever:
                    success = TryGenerateLever(generatedRoom, roomGrid);
                    break;
                default:
                    success = false;
                    break;
            }

            return success;
        }

        private bool TryGenerateLever(List<Room> generatedRoom, int[,] roomGrid)
        {
            float maxDistance = float.MinValue;
            Room selectedRoom = null;
            Room[] startAndExitRoom = GetStartingAndExitRoom(generatedRoom);
            foreach (Room room in generatedRoom)
            {
                if (Vector2.Distance(room.RoomIndex, startAndExitRoom[1].RoomIndex) > maxDistance && !room.ContainPuzzleElement && !room.IsStartingRoom)
                {
                    maxDistance = Vector2.Distance(room.RoomIndex, new Vector2Int(roomGrid.GetLength(1) / 2, roomGrid.GetLength(0) / 2));
                    selectedRoom = room;
                }
            }

            if (selectedRoom != null)
            {
                selectedRoom.ContainLever(startAndExitRoom[1].GetExitDoor());
                startAndExitRoom[1].GetExitDoor().ManageDoor(false);
                return true;
            }

            return false;
        }

        private Room[] GetStartingAndExitRoom(List<Room> generatedRoom)
        {
            Room[] startAndExitRoomList = new Room[2];

            foreach (Room room in generatedRoom)
            {
                if (room.IsStartingRoom)
                    startAndExitRoomList[0] = room;
                else if(room.IsExitRoom)
                    startAndExitRoomList[1] = room;
            }

            return startAndExitRoomList;
        }
    }
}
