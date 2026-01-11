namespace Systems.Events
{
    public class CollectablesChangedEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public int Collectables;
    }
}