using UnityEngine;
using UnityEngine.UI;

namespace EventBus
{
    public class LevelProgressBar : MonoBehaviour
    {
        [SerializeField] private LevelController _levelController;
        [SerializeField] private Slider _slider;

        private bool _isPlaying;

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.SubscribeToEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.UnsubscribeFromEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _isPlaying = e.Data == GameState.Playing;
            if (_isPlaying)
                _slider.value = 0f;
        }

        private void OnLevelCompleted(LevelCompletedEvent e)
        {
            _isPlaying = false;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            _slider.value = _levelController.Progress;
        }
    }
}