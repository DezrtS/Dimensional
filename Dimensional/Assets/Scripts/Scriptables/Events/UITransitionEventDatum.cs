using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "UITransitionEventDatum", menuName = "Scriptable Objects/Events/UITransitionEventDatum")]
    public class UITransitionEventDatum : EventDatum
    {
        [SerializeField] private bool invert;
        [SerializeField] private bool reverse;
        [SerializeField] private float transitionDuration;
    
        public override void HandleEvent()
        {
            UIManager.Instance.Transition(invert, reverse, transitionDuration);
        }
    }
}
