using System;
using Game;
using Puzzle;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerType
{
    Engineer
}

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        private Interactable _currentInteractable = null;
        private PlayerState _currentState;

        private void Start()
        {
            _currentState = new PlayerState();
        }

        private void Update()
        {
            if (_currentInteractable != null && Input.GetKeyDown(KeyCode.E))
            {
                if (_currentInteractable.TryInteract(_currentState))
                {
                    _currentState.IsInteracting = true;
                    GhostManager.Instance.ForceRecord();
                    GameManager.Instance.StartNewLoop();
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
            }
        }
    }
}