using System;
using System.Collections.Generic;
using Player;
using Puzzle;
using TMPro;
using UI;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        // Instance
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        // Manage loop
        private int _numberOfLoop = 1;
        [SerializeField] private TMP_Text _numberOfLoopUI;
        [SerializeField] private TMP_Text _timerUI;
        private bool _isLoopStarted;
        [SerializeField] private float _timeOfALoop;
        private float _timeUntilLoopEnd;
        private float _totalTime;

        // Player ref
        [SerializeField] private GameObject _player;
        
        // Puzzle elements to reset at each loop
        List<Interactable> _interactables;
        
        // Audio
        [Header("Audio")]
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _preRun;
        [SerializeField] private AudioClip _postRun;
        [SerializeField] private List<AudioClip> _musiques;
        
        // End level
        [NonSerialized] public bool IsEndLevel = false;
        [SerializeField] private GameObject _endLevelUI;
        
        // light
        [SerializeField] private GameObject _globalLight;
        
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
            _timeUntilLoopEnd = _timeOfALoop;
            _interactables =  new List<Interactable>();
            _audioSource = GetComponent<AudioSource>();
            _globalLight.SetActive(false);
        }

        private void Update()
        {
            if (IsEndLevel)
                return;
            
            // if the player press R (a faire: quand temps ecoule ?)
            if (Input.GetKeyDown(KeyCode.R) && _isLoopStarted)
            {
                // Start new loop
                StopLoop();
            }

            if (_isLoopStarted)
            {
                _totalTime += Time.deltaTime;
                if (_timeUntilLoopEnd <= 0)
                {
                    StopLoop();
                }
                _timeUntilLoopEnd -= Time.deltaTime;
                if(_timeUntilLoopEnd < 10)
                    _timerUI.text = "0"+(int)_timeUntilLoopEnd;
                else
                    _timerUI.text = ((int)_timeUntilLoopEnd).ToString();
            }
            else
                _timeUntilLoopEnd = _timeOfALoop;
        }

        public void StopLoop()
        {
            // Stop the state recording
            GhostManager.Instance.StopRecording();
            // Reset player position
            _player.GetComponent<PlayerManager>().ResetState();
            // Reset all interactble/puzzle states
            foreach(Interactable interactable in _interactables)
                interactable.SetToDefaultState();
            // Stop the current loop
            _isLoopStarted = false;
            // New loop
            _numberOfLoop++;
            if(_numberOfLoop < 10)
                _numberOfLoopUI.text = "0"+_numberOfLoop;
            else
                _numberOfLoopUI.text = _numberOfLoop.ToString();
            _timerUI.text = "20";
            // Audio
            _audioSource.clip = _preRun;
            _audioSource.Play();
        }
        
        // Called by the puzzle generator to register the interactable/puzzle
        public void RegisterInteractable(Interactable interactable)
        {
            _interactables.Add(interactable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_isLoopStarted && other.GetComponent<PlayerManager>() != null)
            {
                // Start state recording
                GhostManager.Instance.StartRecording();
                // Start the loop
                _isLoopStarted = true;
                // setup correct audio clip
                _audioSource.clip = _musiques[Mathf.Clamp(_numberOfLoop, 0, _musiques.Count - 1)];
                _audioSource.Play();
            }
        }

        public void EndLevel()
        {
            IsEndLevel = true;
            _audioSource.clip = _postRun;
            _audioSource.Play();
            _endLevelUI.SetActive(true);
            _endLevelUI.GetComponent<EndLevelUI>().UpdateUI(_numberOfLoop, _totalTime);
        }
    }
}