using Managers;
using Systems.Player;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ActivatePlayerEventDatum", menuName = "Scriptable Objects/Events/ActivatePlayerEventDatum")]
    public class ActivatePlayerEventDatum : EventDatum
    {
        [SerializeField] private bool activatePlayerControls;
        [SerializeField] private bool activatePlayerMovement;
        
        public override void HandleEvent()
        {
            PlayerController.Instance.DebugDisable = !activatePlayerControls;
            PlayerController.Instance.PlayerMovementController.IsDisabled = !activatePlayerMovement;
        }
    }
}
