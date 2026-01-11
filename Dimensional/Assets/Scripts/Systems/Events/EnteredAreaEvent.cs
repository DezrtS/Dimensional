namespace Systems.Events
{
    public class EnteredAreaEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public string AreaId;
    }
}