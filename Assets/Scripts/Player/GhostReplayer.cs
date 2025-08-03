using System.Collections;
using System.Collections.Generic;
using Game;
using Puzzle;
using UnityEngine;

namespace Player
{
    public class GhostReplayer: MonoBehaviour
    {
        // Time between 2 state, need to be the same as ghostmanager record interval (a faire, le recuperer via l'instance du ghost manager dans un start)
        [SerializeField] private float _playbackInterval = 0.02f;
        // List of player state to apply
        private List<PlayerState> _states;
        // Save interactable to interact with it if necessary
        private Interactable _currentInteractable;

        [SerializeField] private GameObject _bombPrefab;
        private GameObject _bombPlaced = null;

        Animator _animator;
        [SerializeField]
        GameObject _rendererGO;

        public bool _isFacingRight;
        public void Init(List<PlayerState> states)
        {
            _states = states;
            _animator = GetComponentInChildren<Animator>();
            _isFacingRight = states[0].IsFacingRight;
            _animator.SetBool("isFacingRight", _isFacingRight);
        }

        public void StopReplay()
        {
            // Stop the main coroutine
            StopAllCoroutines();
            // Reset position
            transform.position = _states[0].Position;
            _currentInteractable = null;
            if(_bombPlaced != null)
                Destroy(_bombPlaced);
        }

        public void StartReplay()
        {
            StartCoroutine(ReplayStates());
        }

        private IEnumerator ReplayStates()
        {
            // Start apply each state to the ghost
            foreach (PlayerState state in _states)
            {
                if (GameManager.Instance.IsEndLevel)
                    break;
                
                // apply position
                transform.position = state.Position;
                FlipSprite(state);

                // if interacting and near to interactable object, interact
                if (state.IsInteracting && _currentInteractable != null)
                {
                    _currentInteractable.Interact(state);
                }
                
                if (state.IsPlacingBomb)
                {
                    _bombPlaced = Instantiate(_bombPrefab, transform.position, Quaternion.identity);
                    _bombPlaced.GetComponent<Bomb>()._isExploding = true;
                }
                // Wait for the next state
                yield return new WaitForSeconds(_playbackInterval);
            }
        }

        // try to save the current interactable
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

        private void FlipSprite(PlayerState state)
        {
            if (_isFacingRight != state.IsFacingRight)
            {
                _isFacingRight = state.IsFacingRight;
                _animator.SetBool("isFacingRight", _isFacingRight);
            }
        }
    }
}