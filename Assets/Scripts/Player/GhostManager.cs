using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GhostManager: MonoBehaviour
    {
        private static GhostManager _instance;
        public static GhostManager Instance => _instance;
        
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _ghostPrefab;
        [SerializeField] private float _recordInterval = 0.02f;
        
        private float timer;
        private List<PlayerState> _recordedStates;
        private bool isRecording = false;

        private List<GhostReplayer> _ghosts;

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
            if (!isRecording) 
                return;
            
            timer += Time.deltaTime;
            if (timer >= _recordInterval)
            {
                PlayerState ps =  new PlayerState();
                ps.Position = _player.transform.position; // faire une fonction GetPlayerState dans un PlayerManager attaché au player
                _recordedStates.Add(ps);
                timer = 0;
            }
        }

        public void StartRecording()
        {
            Debug.Log("Starting recording");
            _recordedStates = new List<PlayerState>();
            timer = 0;
            isRecording = true;
            
            // Start all ghosts
            foreach(GhostReplayer g in _ghosts)
                g.StartReplay();
        }

        public void StopRecording()
        {
            Debug.Log("Stopping recording");
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