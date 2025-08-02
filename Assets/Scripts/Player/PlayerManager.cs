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
        private float _timeUntilExplosion;
        private bool _bombIsTriggered;

        private void Start()
        {
            _currentState = new PlayerState();
            _bombIsTriggered = false;
        }

        private void Update()
        {
            if (_currentState.IsHoldingBomb == false && Input.GetKeyDown(KeyCode.E))
            {
                if (_currentInteractable != null)
                {
                    if (_currentInteractable.Interact(_currentState) == 1)
                    {
                        _currentState.IsInteracting = true;
                        GhostManager.Instance.ForceRecord();
                        GameManager.Instance.StopLoop();
                    }
                    else if(_currentInteractable.Interact(_currentState) == 2)
                    {
                        _currentState.IsHoldingBomb = true;
                        _bomb = _currentInteractable as Bomb;
                        _bombIsTriggered = true;
                    }
                }
            }

            if(_bombIsTriggered)
            {
                if (_timeUntilExplosion >= 0)
                {
                    if( _currentState.IsHoldingBomb == true && Input.GetKeyDown(KeyCode.E))
                    {
                        _currentState.IsHoldingBomb = false;
                        _bomb.transform.position = this.transform.position;
                        _bomb._isActive = true;
                    }
                    _timeUntilExplosion -= Time.deltaTime;
                    Debug.Log("Decompte : " + _timeUntilExplosion);
                }
                else
                {
                    Debug.Log("boom");
                    _bomb._isExploding = true;
                    _currentState.IsHoldingBomb = false;
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