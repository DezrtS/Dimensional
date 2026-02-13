using System;

namespace Systems.Events.Busses
{
    public static class UIEventBus
    {
        public static event Action<GameEvent> EventFired;
        public static void Fire(GameEvent gameEvent) => EventFired?.Invoke(gameEvent);
    }
}