using System;
using Systems.Player;

namespace Systems.Events
{
    [Serializable]
    public class SetPlayerIsActiveEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Gameplay;
        public bool IsPlayerActive;

        public override void Handle()
        {
            PlayerController.Instance.DebugDisable = !IsPlayerActive;
            PlayerController.Instance.PlayerMovementController.IsDisabled = !IsPlayerActive;
        }
    }
}