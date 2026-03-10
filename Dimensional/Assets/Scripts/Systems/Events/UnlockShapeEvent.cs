using System;
using Scriptables.Shapes;
using Systems.Player;

namespace Systems.Events
{
    [Serializable]
    public class UnlockShapeEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Gameplay;
        public ShapeType ShapeType;

        public override void Handle()
        {
            if (!PlayerController.Instance.UnlockedShapes.Contains(ShapeType)) PlayerController.Instance.UnlockedShapes.Add(ShapeType);
        }
    }
}