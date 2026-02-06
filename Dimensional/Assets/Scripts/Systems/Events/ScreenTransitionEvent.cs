using System;
using Managers;
using UnityEngine;

namespace Systems.Events
{
    [Serializable]
    public class ScreenTransitionEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.UI;
        [SerializeField] private bool invert;
        [SerializeField] private bool reverse;
        [SerializeField] private float transitionDuration;

        public override void Handle()
        {
            UIManager.Instance.Transition(invert, reverse, transitionDuration);
        }
    }
}