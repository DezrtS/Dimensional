using System;

namespace Systems.Events.Busses
{
    public static class EventBus
    {
        public static event Action<GameEvent> EventFired;

        public static void Fire(GameEvent gameEvent)
        {
            EventFired?.Invoke(gameEvent);
            switch (gameEvent.BusType)
            {
                case EventBusType.Gameplay:
                    break;
                case EventBusType.World:
                    break;
                case EventBusType.Quest:
                    QuestEventBus.Fire(gameEvent);
                    break;
                case EventBusType.UI:
                    UIEventBus.Fire(gameEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}