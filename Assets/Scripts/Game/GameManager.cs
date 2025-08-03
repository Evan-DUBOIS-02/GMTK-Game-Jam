using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Puzzle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;

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
        [Header("UI")]
        [NonSerialized] public bool IsEndLevel = false;
        [SerializeField] private GameObject _endLevelUI;
        [SerializeField] private GameObject _inGameUI;
        
        // light
        [SerializeField] private GameObject _globalLight;
        
        // Fade animation
        private float _fadeDuration = 0.31f;
        [SerializeField] private Image _fader;
        
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
            // Fade out
            Color c = _fader.color;
            _fader.color = new Color(c.r, c.g, c.b, 1);
            StartCoroutine(FadeOut());
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
                else
                {
                    _timeUntilLoopEnd -= Time.deltaTime;
                    if (_timeUntilLoopEnd < 10)
                        _timerUI.text = "0" + (int)_timeUntilLoopEnd;
                    else
                        _timerUI.text = ((int)_timeUntilLoopEnd).ToString();
                }
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
            // Stop the current loop
            _isLoopStarted = false;
            StartCoroutine(LoopTransition());
        }

        public IEnumerator LoopTransition()
        {
            // Fade in
            float elapsed = 0f;
            Color c = _fader.color;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsed / _fadeDuration);
                _audioSource.volume = Mathf.Lerp(0.5f, 0, elapsed / _fadeDuration);
                _fader.color = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }

            _fader.color = new Color(c.r, c.g, c.b, 1);
            
            // Reinitialize
            ReinitializeForNextLoop();
            yield return new WaitForSeconds(0.1f);
            
            // Fade out
            StartCoroutine(FadeOut());
        }

        public IEnumerator FadeOut()
        {
            _audioSource.clip = _preRun;
            _audioSource.volume = 0;
            _audioSource.loop = true;
            _audioSource.Play();
            float elapsed = 0f;
            Color c = _fader.color;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsed / _fadeDuration);
                _audioSource.volume = Mathf.Lerp(0, 0.5f, elapsed / _fadeDuration);;
                _fader.color = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }

            _fader.color = new Color(c.r, c.g, c.b, 0);
        }

        public void ReinitializeForNextLoop()
        {
            // Reset all interactble/puzzle states
            foreach(Interactable interactable in _interactables)
                interactable.SetToDefaultState();
            // New loop
            _numberOfLoop++;
            if(_numberOfLoop < 10)
                _numberOfLoopUI.text = "0"+_numberOfLoop;
            else
                _numberOfLoopUI.text = _numberOfLoop.ToString();
            _timerUI.text = "20";
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
                _audioSource.loop = false;
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
            _inGameUI.SetActive(false);
        }
    }
}