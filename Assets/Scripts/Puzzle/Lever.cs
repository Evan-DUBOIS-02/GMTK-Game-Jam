using UnityEngine;

namespace Puzzle
{
    public class Lever: MonoBehaviour
    {
        private ExitDoor _door;
        public ExitDoor Door{get{return _door;} set{_door = value;}}
        
        public void OpenDoor()
        {
            _door.ManageDoor(true);
        }
    }
}