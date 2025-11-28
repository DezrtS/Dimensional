using UnityEngine;

namespace Scriptables.Events
{
    public enum EventType
    {
        None,
        World,
        Dialogue,
        Camera,
        Cutscene,
    }
    
    public abstract class EventDatum : ScriptableObject
    {
        [SerializeField] private EventType eventType;
        [SerializeField] private string eventName;
        
        public EventType EventType => eventType;
        public string EventName => eventName;
        
        public abstract void HandleEvent();
    }
}
