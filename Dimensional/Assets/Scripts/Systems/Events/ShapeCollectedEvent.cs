using Scriptables.Shapes;

namespace Systems.Events
{
    public class ShapeCollectedEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Game;
        public ShapeType ShapeType;
    }
}