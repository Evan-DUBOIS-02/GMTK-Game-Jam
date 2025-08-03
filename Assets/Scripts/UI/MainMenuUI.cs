using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI: MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuElement;
        [SerializeField] private GameObject _creditsMenuElement;
        [SerializeField] private string _nextLevelName;

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
            SceneManager.LoadScene(_nextLevelName);
        }

        public void OnExitClicked()
        {
            Debug.Log("Exit");
            Application.Quit();
        }
    }
}