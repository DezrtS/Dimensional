namespace Systems.Events
{
    public class TicketCollectedEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Game;
        public string TicketId;
    }
}
