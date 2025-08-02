using Game;
using Puzzle;
using System.Linq.Expressions;
using UnityEngine;

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        private Interactable _currentInteractable;
        private PlayerState _currentState;
        private Bomb _bomb;
        [SerializeField]
        private bool _bombIsTriggered;
        private bool _isHoldinge;

        private void Start()
        {
            _currentState = new PlayerState();
            _bombIsTriggered = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isHoldinge)
                    return;
                else
                    _isHoldinge = true;

                if (_currentInteractable != null)
                {
                    if (_currentInteractable.Interact(_currentState) == 1)
                    {
                        _currentState.IsInteracting = true;
                        GhostManager.Instance.ForceRecord();
                        GameManager.Instance.StopLoop();
                    }

                    else if (_currentState.IsHoldingBomb == false && _currentInteractable.Interact(_currentState) == 2)
                    {
                        _currentState.IsHoldingBomb = true;
                        _bomb = _currentInteractable as Bomb;
                    }
                }
                else if(_currentState.IsHoldingBomb == true)
                {
                    _currentState.IsHoldingBomb = false;
                    _bomb.transform.position = this.transform.position;
                    _bomb.rendererBomb.SetActive(true);
                    GhostManager.Instance.ForceRecord();
                    GameManager.Instance.StopLoop();
                    _bomb._isExploding = true;
                }
            }

            else
                _isHoldinge = false;
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
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Interactable interactable;
            if (other.TryGetComponent(out interactable))
            {
                _currentInteractable = null;
            }
        }
    }
}