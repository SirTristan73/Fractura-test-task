using UnityEngine;

namespace EventBus
{
    public class OnClickEvent : EventType
    {
        public ButtonEvent Data {get; private set;}
        
        public OnClickEvent (ButtonEvent data)
        {
            Data = data;
        }
    }
}