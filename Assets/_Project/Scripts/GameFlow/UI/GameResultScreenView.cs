using System;
using _Project.Scripts.GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.GameFlow.UI
{
    public class GameResultScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _damageDealtText;
        [SerializeField] private TMP_Text _damageTakenText;
        [SerializeField] private TMP_Text _elapsedTimeText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

        public event Action RestartClicked = delegate { };
        public event Action ExitClicked = delegate { };

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(OnRestartClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(OnRestartClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        public void Show(string result, MatchStatsSnapshot stats)
        {
            _root.SetActive(true);
            _titleText.text = result;
            SetText(_damageDealtText, stats.DamageDealt.ToString());
            SetText(_damageTakenText, stats.DamageTaken.ToString());
            SetText(_elapsedTimeText, FormatTime(stats.ElapsedTime));
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        private void OnRestartClicked()
        {
            RestartClicked.Invoke();
        }

        private void OnExitClicked()
        {
            ExitClicked.Invoke();
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
                text.text = value;
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.FloorToInt(seconds);
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            return $"{minutes:00}:{remainingSeconds:00}";
        }
    }
}
