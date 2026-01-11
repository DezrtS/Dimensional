using Managers;

namespace Systems.Events
{
    public class GameStateEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Game;
        public GameState GameState;
    }
}