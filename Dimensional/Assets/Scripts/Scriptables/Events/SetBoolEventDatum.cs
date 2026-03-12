using Scriptables.Save;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "SetBoolEventDatum", menuName = "Scriptable Objects/Events/SetBoolEventDatum")]
    public class SetBoolEventDatum : EventDatum
    {
        [SerializeField] private BoolVariable boolVariable;
        [SerializeField] private bool setTo;
        
        public override void HandleEvent()
        {
            boolVariable.Value = setTo;
        }
    }
}
