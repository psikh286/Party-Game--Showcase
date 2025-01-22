using Party.Minigames.Hockey;
using TMPro;
using UnityEngine;

namespace Party.GUI.Minigames
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        private readonly int[] _scores = new int[2];

        private void Start() => _scoreText.text = "0:0";

        private void OnEnable()
        {
            HockeyGate.GoalScoredEvent += OnGoalScored;
        }

        private void OnDisable()
        {
            HockeyGate.GoalScoredEvent -= OnGoalScored;
        }

        private void OnGoalScored(int winner)
        {
            _scores[winner]++;

            _scoreText.text = $"{_scores[0]}:{_scores[1]}";
        }
    }
}