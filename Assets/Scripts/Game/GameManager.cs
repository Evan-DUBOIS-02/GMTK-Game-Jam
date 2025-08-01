using System;
using System.Collections.Generic;
using Player;
using Puzzle;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        // Instance
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        // Manage loop
        private bool _isLoopStarted = false;

        // Player ref (a faire: trouve via un tag que par SerializeField)
        [SerializeField] private GameObject _player;
        
        // Puzzle elements to reset at each loop
        List<Interactable> _interactables;
        
        private void Awake()
        {
            // Generate unique instance
            if(_instance == null) 
                _instance = this;
            else 
                Destroy(this);
        }

        private void Start()
        {
            // Init
            _interactables =  new List<Interactable>();
        }

        private void Update()
        {
            // if the loop not start yet
            if (!_isLoopStarted)
            {
                // Waiting player movement (a faire: après le choix de classe ?)
                if (_player.GetComponent<PlayerMovement>().IsMoving())
                {
                    // Start state recording
                    GhostManager.Instance.StartRecording();
                    // Start the loop
                    _isLoopStarted = true;
                }
            } 
            // if the player press R (a faire: quand temps ecoule ?)
            else if (Input.GetKeyDown(KeyCode.R))
            {
                // Start new loop
                StartNewLoop();
            }
        }
        
        public void StartNewLoop()
        {
            // Stop the state recording
            GhostManager.Instance.StopRecording();
            // Reset player position
            _player.GetComponent<PlayerMovement>().InitializePosition();
            // Reset all interactble/puzzle states
            foreach(Interactable interactable in _interactables)
                interactable.SetToDefaultState();
            // Stop the current loop
            _isLoopStarted = false;
        }
        
        // Called by the puzzle generator to register the interactable/puzzle
        public void RegisterInteractable(Interactable interactable)
        {
            _interactables.Add(interactable);
        }
    }
}