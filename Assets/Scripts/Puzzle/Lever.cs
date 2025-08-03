using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle
{
    public class Lever: MonoBehaviour, Interactable
    {
        [SerializeField] private GameObject _leverOn;
        [SerializeField] private GameObject _leverOff;
        [SerializeField] private GameObject _UI;
        
        private Door _door;
        public Door Door{get{return _door;} set{_door = value;}}
        
        private bool _isActive;

        public void SetToDefaultState()
        {
            _isActive = false;
            SwitchSprite();
            _door.ManageDoor(_isActive);
            _UI.SetActive(false);
        }

        public int Interact(Player.PlayerState state)
        {
            _isActive = true;
            SwitchSprite();
            _door.ManageDoor(_isActive);
            _UI.SetActive(false);
            return 1;
        }

        private void SwitchSprite()
        {
            _leverOn.SetActive(_isActive);
            _leverOff.SetActive(!_isActive);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && !_isActive)
                _UI.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
                _UI.SetActive(false);
        }
    }
}