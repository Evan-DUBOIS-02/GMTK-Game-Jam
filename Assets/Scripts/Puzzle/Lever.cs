using System;
using UnityEngine;

namespace Puzzle
{
    public class Lever: MonoBehaviour, Interactable
    {
        [SerializeField] private GameObject _leverOn;
        [SerializeField] private GameObject _leverOff;
        
        private Door _door;
        public Door Door{get{return _door;} set{_door = value;}}
        
        private bool _isActive;

        public void SetToDefaultState()
        {
            _isActive = false;
            SwitchSprite();
            _door.ManageDoor(_isActive);
        }

        public bool TryInteract(Player.PlayerState state)
        {
            _isActive = true; 
            SwitchSprite();
            _door.ManageDoor(_isActive);
            return true;
        }

        private void SwitchSprite()
        {
            _leverOn.SetActive(_isActive);
            _leverOff.SetActive(!_isActive);
        }
    }
}