using UnityEngine;

namespace EventBus
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.StartScreen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<OnClickEvent>(OnButtonClicked);
            EventBus.SubscribeToEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
            EventBus.SubscribeToEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<OnClickEvent>(OnButtonClicked);
            EventBus.UnsubscribeFromEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
            EventBus.UnsubscribeFromEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnButtonClicked(OnClickEvent e)
        {
            switch (e.Data)
            {
                case ButtonEvent.StartGame:
                    SetState(GameState.Playing);
                    break;
            }
        }

        private void OnVehicleDestroyed(VehicleDestroyedEvent e)
        {
            SetState(GameState.Lost);
        }

        private void OnLevelCompleted(LevelCompletedEvent e)
        {
            SetState(GameState.Won);
        }

        private void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            EventBus.Trigger(new GameStateChangedEvent(newState));
        }
    }
    
    public enum GameState
    {
        StartScreen,
        Playing,
        Lost,
        Won,
    }
}