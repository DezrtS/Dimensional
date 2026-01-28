using System;
using Managers;
using Systems.Events.Busses;

namespace Systems.Events
{
    public enum EventBusType
    {
        Gameplay,
        World,
        Quest,
        UI
    }
    
    [Serializable]
    public abstract class GameEvent
    {
        public abstract EventBusType BusType { get; }
        public float Duration;

        public virtual void Handle()
        {
            EventBus.Fire(this);
        }
    }
}
