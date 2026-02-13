using Systems.Events;
using Systems.Events.Busses;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ScreenTransitionEventDatum", menuName = "Scriptable Objects/Events/ScreenTransitionEventDatum")]
    public class ScreenTransitionEventDatum : GameEventDatum
    {
        [SerializeField] private ScreenTransitionEvent screenTransitionEvent;
        public override GameEvent GameEvent => screenTransitionEvent;
    }
}
