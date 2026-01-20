namespace Systems.Events
{
    public class FlagEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public string FlagId;
    }
}
