using System;
using UnityEngine;

namespace Systems.Events
{
    [Serializable]
    public class HideTextEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.UI;
        public DisplayType DisplayType;
    }
}