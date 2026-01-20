using System;

namespace Systems.Events
{
    [Serializable]
    public class StartQuestEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Quest;
        public string QuestId;
        public bool ForceStart;
    }
}