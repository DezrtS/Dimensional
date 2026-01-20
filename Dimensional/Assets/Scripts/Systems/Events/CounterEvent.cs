namespace Systems.Events
{
    public class CounterEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public string CounterId;
        public int CounterValue;
    }
}
