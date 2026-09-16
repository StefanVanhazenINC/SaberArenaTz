using TMPro;
using UnityEngine;

namespace _Project.Scripts.GameFlow.UI
{
    public sealed class GameTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        public void SetTime(float seconds)
        {
            int totalSeconds = Mathf.FloorToInt(seconds);
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            _timeText.text = $"{minutes:00}:{remainingSeconds:00}";
        }
    }
}
