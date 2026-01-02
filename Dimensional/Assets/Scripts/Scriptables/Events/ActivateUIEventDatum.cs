using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ActivateUIEventDatum", menuName = "Scriptable Objects/Events/ActivateUIEventDatum")]
    public class ActivateUIEventDatum : EventDatum
    {
        [SerializeField] private UserInterfaceType userInterfaceType;


        public override void HandleEvent()
        {
            UIManager.Instance.ActivateUI(userInterfaceType);
        }
    }
}
