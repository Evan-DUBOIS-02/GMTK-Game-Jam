using System;
using UnityEngine;

namespace Puzzle
{
    public class Lever: MonoBehaviour, Interactable
    {
        [SerializeField] private GameObject _leverOn;
        [SerializeField] private GameObject _leverOff;
        
        private ExitDoor _door;
        public ExitDoor Door{get{return _door;} set{_door = value;}}
        
        private bool _isActive;

        public void SetToDefaultState()
        {
            _isActive = false;
            SwitchSprite();
            _door.ManageDoor(_isActive);
        }

        public bool TryInteract(Player.PlayerState state)
        {
            if (state.Type == PlayerType.Engineer)
            {
                _isActive = !_isActive;
                SwitchSprite();
                _door.ManageDoor(_isActive);
                return true;
            }

            return false;
        }

        private void SwitchSprite()
        {
            _leverOn.SetActive(_isActive);
            _leverOff.SetActive(!_isActive);
        }
    }
}