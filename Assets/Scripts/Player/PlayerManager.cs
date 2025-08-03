using Game;
using Puzzle;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        private Interactable _currentInteractable;
        public PlayerState _currentState;
        [SerializeField] private GameObject _bombPrefab;
        private bool _isHoldinge;
        private bool _isHoldingBomb;
        public bool _isBeingBombed; //If true => Can go through void
        
        private void Start()
        {
            _currentState = new PlayerState();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isHoldinge)
                    return;

                _isHoldinge = true;

                if (_currentInteractable != null)
                {
                    int interactableId = _currentInteractable.Interact(_currentState);
                    if (interactableId == 1)
                    {
                        _currentState.IsInteracting = true;
                        GhostManager.Instance.ForceRecord();
                        GameManager.Instance.StopLoop();
                    }
                    else if (_isHoldingBomb == false && interactableId == 2)
                    {
                        _currentState.IsInteracting = true;
                        _isHoldingBomb = true;
                    }
                }
                else if(_isHoldingBomb)
                {
                    _currentState.IsPlacingBomb = true;
                    _isHoldingBomb = false;
                    GhostManager.Instance.ForceRecord();
                    GameManager.Instance.StopLoop();
                }
            }

            else
                _isHoldinge = false;
        }

        IEnumerator PauseThenRestart()
        {
            yield return new WaitForSecondsRealtime(2f);
        }

        public void ResetState()
        {
            _isHoldingBomb = false;
            GetComponent<PlayerMovement>().InitializePosition();
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