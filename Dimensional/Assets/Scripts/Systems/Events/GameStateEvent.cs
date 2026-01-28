using System;
using Managers;

namespace Systems.Events
{
    [Serializable]
    public class GameStateEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Gameplay;
        public GameState GameState;

        public override void Handle()
        {
            GameManager.Instance.ChangeGameState(GameState);
        }
    }
}