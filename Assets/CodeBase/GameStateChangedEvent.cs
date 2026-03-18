using UnityEngine;

namespace EventBus
{
    public class GameStateChangedEvent : EventType
    {
        public GameState Data { get; private set; }

        public GameStateChangedEvent(GameState data)
        {
            Data = data;
        }
    }
}
