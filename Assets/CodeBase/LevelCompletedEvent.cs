using UnityEngine;

namespace EventBus
{
    public class LevelCompletedEvent : EventType
    {
        public bool Success { get; }
        public LevelCompletedEvent(bool t)
        {
            Success = t;
        }
    }
}