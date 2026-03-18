using UnityEngine;

namespace EventBus
{
    public class UIManager : MonoBehaviour
    {
        public UIState CurrentUIState { get; private set; } = UIState.StartScreen;
        
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;

        private void OnEnable()
        {
            EventBus.SubscribeToEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
            EventBus.SubscribeToEvent<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.SubscribeToEvent<OnClickEvent>(OnButtonClicked);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeFromEvent<VehicleDestroyedEvent>(OnVehicleDestroyed);
            EventBus.UnsubscribeFromEvent<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.UnsubscribeFromEvent<OnClickEvent>(OnButtonClicked);
        }

        private void OnButtonClicked(OnClickEvent e)
        {
            switch (e.Data)
            {
                case ButtonEvent.StartGame:
                    SetState(UIState.Ingame);
                    break;
            }
        }

        private void OnVehicleDestroyed(VehicleDestroyedEvent e) 
        { 
            SetState(UIState.LostScreen);
        }

        private void OnLevelCompleted(LevelCompletedEvent e)     
        {
            SetState(UIState.WinScreen);
        }

        private void SetState(UIState state)
        {
            if (CurrentUIState == state) return;
            CurrentUIState = state;

            _startScreen.SetActive(state == UIState.StartScreen);
            _winScreen.SetActive(state == UIState.WinScreen);
            _loseScreen.SetActive(state == UIState.LostScreen);
        }
    }

    public enum UIState
    {
        StartScreen,
        Ingame,
        LostScreen,
        WinScreen,
    }
}