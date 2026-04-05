using System;
using UnityEngine;

namespace Systems.Events
{
    public enum DisplayType
    {
        None,
        Tutorial,
        Area,
        Boss,
        Objective,
    }
    
    [Serializable]
    public class DisplayTextEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.UI;
        public DisplayType DisplayType;
        public string Text;
        public bool HasDisplayDuration;
        public float DisplayDuration;
    }
}