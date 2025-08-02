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
                    Debug.Log(_currentInteractable.Interact(_currentState));
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
                    Debug.Log("Depose la bombe");
                    _currentState.IsHoldingBomb = false;
                    _bomb.transform.position = this.transform.position;
                    _bomb.rendererBomb.SetActive(true);
                    _timeUntilExplosion -= Time.deltaTime;
                }
            }

            else
                _isHoldinge = false;
            if(_currentState.IsHoldingBomb)
            {
                Debug.Log("Possede la bombe + temps restants : " + _timeUntilExplosion);
                _timeUntilExplosion -= Time.deltaTime;
                if (_timeUntilExplosion <= 0)
                {
                    _currentState.IsHoldingBomb = false;
                    _bomb._isExploding = true;
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