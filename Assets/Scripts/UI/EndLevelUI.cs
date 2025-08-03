using System;
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
        [SerializeField] private TMP_Text _seed;
        public void UpdateUI(int numberOfLoop, float totalTime)
        {
            _endLevelUIText.text = numberOfLoop.ToString();
            _totalTimeText.text = ((int)totalTime)+".s";
            _seed.text = "Seed : " + SeedManager.Instance._seed.ToString();
        }
        
        public void OnRetryClicked()
        {
            SeedManager.Instance.GenerateRandomizer(SeedManager.Instance._seed);
            SceneManager.LoadScene(_nextLevelName);
        }

        public void OnNextClicked()
        {
            SeedManager.Instance.GenerateRandomizer((int)DateTime.Now.Ticks);
            SceneManager.LoadScene(_nextLevelName);
        }

        public void OnMenuClicked()
        {
            SceneManager.LoadScene(_mainMenuLevelName);
        }
    }
}