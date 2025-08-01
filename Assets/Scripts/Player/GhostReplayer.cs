using System.Collections;
using System.Collections.Generic;
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
        
        public void Init(List<PlayerState> states)
        {
            _states = states;
        }

        public void StopReplay()
        {
            // Stop the main coroutine
            StopAllCoroutines();
            // Reset position
            transform.position = _states[0].Position;
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
                // apply position
                transform.position = state.Position;
                // if interacting and near to interactable object, interact
                if (state.IsInteracting && _currentInteractable != null)
                    _currentInteractable.TryInteract(state);
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
    }
}