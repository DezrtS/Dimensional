using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ResetUIEventDatum", menuName = "Scriptable Objects/Events/ResetUIEventDatum")]
    public class ResetUIEventDatum : EventDatum
    {
        public override void HandleEvent()
        {
            UIManager.Instance.DeactivateUI();
        }
    }
}
