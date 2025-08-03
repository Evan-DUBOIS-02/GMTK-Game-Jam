using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI: MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuElement;
        [SerializeField] private GameObject _creditsMenuElement;
        [SerializeField] private string _nextLevelName;
        [SerializeField] private Image _fader;
        private float _fadeDuration = 0.31f;

        public void OnReturnCreditClicked()
        {
            Debug.Log("OnReturnCreditClicked");
            _creditsMenuElement.SetActive(false);
            _mainMenuElement.SetActive(true);
        }
        
        public void OnCreditClicked()
        {
            Debug.Log("OnCreditClicked");
            _creditsMenuElement.SetActive(true);
            _mainMenuElement.SetActive(false);
        }

        public void OnPlayClicked()
        {
            Debug.Log("Play");
            StartCoroutine(FadeInAndPlay());
        }

        public IEnumerator FadeInAndPlay()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            float elapsed = 0f;
            Color c = _fader.color;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsed / _fadeDuration);
                audioSource.volume = Mathf.Lerp(0.5f, 0.0f, elapsed / _fadeDuration);;
                _fader.color = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }

            _fader.color = new Color(c.r, c.g, c.b, 1);
            SceneManager.LoadScene(_nextLevelName);
        }

        public void OnExitClicked()
        {
            Debug.Log("Exit");
            Application.Quit();
        }
    }
}