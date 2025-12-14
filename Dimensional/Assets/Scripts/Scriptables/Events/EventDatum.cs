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
        Player,
        Teleport,
    }
    
    public abstract class EventDatum : ScriptableObject
    {
        [SerializeField] private EventType eventType;
        [SerializeField] private string eventName;
        [SerializeField] private float duration;
        
        public EventType EventType => eventType;
        public string EventName => eventName;
        public float Duration => duration;
        
        public abstract void HandleEvent();
    }
}
