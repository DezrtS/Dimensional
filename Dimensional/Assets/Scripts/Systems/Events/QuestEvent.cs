namespace Systems.Events
{
    public abstract class QuestEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public string QuestId;
    }
}