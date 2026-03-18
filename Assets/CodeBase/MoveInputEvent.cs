using UnityEngine;

namespace EventBus
{
    public class MoveInputEvent :EventType
    {
        public Vector2 Delta { get; }
        public MoveInputEvent(Vector2 delta) 
        { 
            Delta = delta;
        }
    }
}