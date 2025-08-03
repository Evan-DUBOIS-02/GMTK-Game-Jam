using System;
using UnityEngine;

namespace Dungeon
{
    public class Hallway: MonoBehaviour
    {
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _breakableWallPrefab;
        [SerializeField] private GameObject _voidPrefab;

        private Room _room1;
        public Room Room1{get{return _room1;}}
        private Room _room2;
        public Room Room2{get{return _room2;}}

        [NonSerialized] public bool ContainObstacle = false;
        
        public void SetRooms(Room room1, Room room2)
        {
            _room1 = room1;
            _room1.AddHallway(this);
            _room2 = room2;
            _room2.AddHallway(this);
        }

        public Room GetOtherRoom(Room initialRoom)
        {
            if (_room1 != initialRoom)
                return _room1;
            return _room2;
        }

        public Puzzle.Door GenerateDoor()
        {
            GameObject doorGo = Instantiate(_doorPrefab, transform.position, Quaternion.identity);
            Puzzle.Door doorScript = doorGo.GetComponent<Puzzle.Door>();
            if(_room1.RoomIndex.x <  _room2.RoomIndex.x)
                doorScript.SetSideRenderer(false);
            else if(_room1.RoomIndex.x >  _room2.RoomIndex.x)
                doorScript.SetSideRenderer(true);
            doorGo.transform.SetParent(transform);
            doorScript.ManageDoor(false);
            ContainObstacle = true;
            return doorScript;
        }

        public Puzzle.BreakableWall GenerateBreakableWall()
        {
            GameObject BWGo = Instantiate(_breakableWallPrefab, transform.position, Quaternion.identity);
            Puzzle.BreakableWall bwScript = _breakableWallPrefab.GetComponent<Puzzle.BreakableWall>();
            if(_room1.RoomIndex.x != _room2.RoomIndex.x)
                bwScript.SetSideRenderer();
            BWGo.transform.SetParent(transform);
            ContainObstacle = true;
            return bwScript;
        }

        public Puzzle.Void GenerateVoid()
        {
            GameObject VoidGO = Instantiate(_voidPrefab, transform.position, Quaternion.identity);
            Puzzle.Void voidScript = _voidPrefab.GetComponent<Puzzle.Void>();
            if (_room1.RoomIndex.x != _room2.RoomIndex.x)
                voidScript.SetSideRenderer();
            VoidGO.transform.SetParent(transform);
            ContainObstacle = true;
            return voidScript;
        }
    }
}