using UnityEngine;

namespace Dungeon
{
    public class Hallway: MonoBehaviour
    {
        public Room _room1;
        public Room _room2;

        public void SetRooms(Room room1, Room room2)
        {
            _room1 = room1;
            _room2 = room2;
        }
    }
}