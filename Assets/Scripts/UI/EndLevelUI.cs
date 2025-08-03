using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class EndLevelUI : MonoBehaviour
    {
        [SerializeField] private string _mainMenuLevelName;
        [SerializeField] private string _nextLevelName;
        [SerializeField] private TMP_Text _endLevelUIText;
        [SerializeField] private TMP_Text _totalTimeText;
        public void UpdateUI(int numberOfLoop, float totalTime)
        {
            _endLevelUIText.text = numberOfLoop.ToString();
            _totalTimeText.text = ((int)totalTime)+".s";
        }
        
        public void OnRetryClicked()
        {
            Debug.Log("RetryClicked");
        }

        public void OnNextClicked()
        {
            SceneManager.LoadScene(_nextLevelName);
        }

        public void OnMenuClicked()
        {
            SceneManager.LoadScene(_mainMenuLevelName);
        }
    }
}