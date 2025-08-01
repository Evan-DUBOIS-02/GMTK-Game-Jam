using System;
using UnityEngine;

namespace Puzzle
{
    public class Lever: MonoBehaviour, Interactable
    {
        private ExitDoor _door;
        public ExitDoor Door{get{return _door;} set{_door = value;}}
        
        private bool _isActive = false;

        public void SetToDefaultState()
        {
            _isActive = false;
            _door.ManageDoor(_isActive);
        }

        public bool TryInteract(Player.PlayerState state)
        {
            if (state.Type == PlayerType.Engineer)
            {
                _isActive = !_isActive;
                _door.ManageDoor(_isActive);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Lever: OnTriggerEnter");
        }
    }
}