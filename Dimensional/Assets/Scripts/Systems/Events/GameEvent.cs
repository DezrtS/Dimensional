using System;

namespace Systems.Events
{
    public enum EventBusType
    {
        Game,
        World,
        Quest,
        UI
    }
    
    [Serializable]
    public abstract class GameEvent
    {
        public abstract EventBusType BusType { get; }
        public float Duration;
    }
}
