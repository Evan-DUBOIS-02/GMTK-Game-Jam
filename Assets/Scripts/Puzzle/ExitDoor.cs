using UnityEngine;

namespace Puzzle
{

    public class ExitDoor: MonoBehaviour
    {
        public void ManageDoor(bool open)
        {
            GetComponent<SpriteRenderer>().enabled = !open;
        }
    }
}