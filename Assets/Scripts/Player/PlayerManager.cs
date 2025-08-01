using System;
using Game;
using Puzzle;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerType
{
    None,
    Engineer
}

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        private Interactable _currentInteractable;
        private PlayerTypeSelector _playerTypeSelector;
        private PlayerState _currentState;

        private void Start()
        {
            _currentState = new PlayerState();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_currentInteractable != null)
                {
                    if (_currentInteractable.TryInteract(_currentState))
                    {
                        _currentState.IsInteracting = true;
                        GhostManager.Instance.ForceRecord();
                        GameManager.Instance.StopLoop();
                    }
                }
                else if (_playerTypeSelector != null)
                {
                    _currentState.Type = _playerTypeSelector.PlayerTypeToApply;
                    Debug.Log("Type applied: "+_currentState.Type);
                }
            }
        }

        public PlayerState GetPlayerState()
        {
            _currentState.Position = transform.position;
            return new PlayerState(_currentState); // passage par copie
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Interactable interactable;
            if (other.TryGetComponent(out interactable))
            {
                _currentInteractable = interactable;
                return;
            }

            PlayerTypeSelector selector;
            if (other.TryGetComponent(out selector))
            {
                _playerTypeSelector = selector;
                return;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Interactable interactable;
            if (other.TryGetComponent(out interactable))
            {
                _currentInteractable = null;
            }

            PlayerTypeSelector selector;
            if(other.TryGetComponent(out selector))
                _playerTypeSelector = null;
        }
    }
}