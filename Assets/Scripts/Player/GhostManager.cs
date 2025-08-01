using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GhostManager: MonoBehaviour
    {
        // Instance
        private static GhostManager _instance;
        public static GhostManager Instance => _instance;
        
        // Player ref (a faire: trouver par tag)
        [SerializeField] private PlayerManager _player;
        
        // ghost prefab to instantiate
        [SerializeField] private GameObject _ghostPrefab;
        // record frame rate (here = 50)
        [SerializeField] private float _recordInterval = 0.02f;
        
        // Timer to record according to the frame rate
        private float timer;
        private List<PlayerState> _recordedStates;
        private bool isRecording;

        // List of all current ghost => remove oldest to limit the number
        private List<GhostReplayer> _ghosts;

        // Initialize unique instance
        private void Awake()
        {
            if(_instance == null) 
                _instance = this;
            else 
                Destroy(this);
        }

        private void Start()
        {
            _ghosts = new List<GhostReplayer>();
        }
        
        private void Update()
        {
            // Wait for recording
            if (!isRecording) 
                return;
            
            // register player state according to record frame rate
            timer += Time.deltaTime;
            if (timer >= _recordInterval)
            {
                _recordedStates.Add(_player.GetPlayerState());
                timer = 0;
            }
        }

        /// <summary>
        /// Called when player interact with an object to ensure we catch it.
        /// Potential issue: if we record when timer is near to 0 and not to record interval we will see a little
        /// interference in the ghost execution because it will apply the state at record interval and not at 0.
        /// </summary>
        public void ForceRecord()
        {
            _recordedStates.Add(_player.GetPlayerState());
            // Reset timer to jump the next potential frame
            timer = 0;
        }

        /// <summary>
        /// Reset record state and ask to all ghost to start replaying there states
        /// </summary>
        public void StartRecording()
        {
            _recordedStates = new List<PlayerState>();
            timer = 0;
            isRecording = true;
            
            // Start all ghosts
            foreach(GhostReplayer g in _ghosts)
                g.StartReplay();
        }

        /// <summary>
        /// Called at the end of a loop
        /// Stop the record, create new ghost according to last record and stop all ghost executing there states
        /// </summary>
        public void StopRecording()
        {
            isRecording = false;
            
            // Initialize new ghost
            GameObject ghost = Instantiate(_ghostPrefab);
            ghost.GetComponent<GhostReplayer>().Init(_recordedStates);
            _ghosts.Add(ghost.GetComponent<GhostReplayer>());
            
            // Stop all ghosts
            foreach(GhostReplayer g in _ghosts)
                g.StopReplay();
        }
    }
}