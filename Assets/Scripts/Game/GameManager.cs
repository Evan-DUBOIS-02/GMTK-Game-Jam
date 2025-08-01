using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        private bool _isLoopStarted = false;

        [SerializeField] private GameObject _player;
        
        private void Awake()
        {
            if(_instance == null) 
                _instance = this;
            else 
                Destroy(this);
        }
        private void Update()
        {
            if (!_isLoopStarted)
            {
                if (_player.GetComponent<PlayerMovement>().IsMoving())
                {
                    Player.GhostManager.Instance.StartRecording();
                    _isLoopStarted = true;
                }
            } 
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Player.GhostManager.Instance.StopRecording();
                _player.GetComponent<PlayerMovement>().InitializePosition();
                _isLoopStarted = false;
            }
        }
    }
}