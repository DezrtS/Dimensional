namespace Systems.Events
{
    public abstract class DialogueEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Game;
        public string DialogueSequenceId;
    }
}