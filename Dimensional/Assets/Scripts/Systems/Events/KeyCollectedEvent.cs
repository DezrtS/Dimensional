namespace Systems.Events
{
    public class KeyCollectedEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Game;
        public string KeyId;
    }
}