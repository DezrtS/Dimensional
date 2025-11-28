using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ActivateCameraEventDatum", menuName = "Scriptable Objects/ActivateCameraEventDatum")]
    public class ActivateCameraEventDatum : EventDatum
    {
        [SerializeField] private bool lockAndHideCursor;
        [SerializeField] private bool activateCamera;
        
        public override void HandleEvent()
        {
            var cameraManager = CameraManager.Instance;
            if (lockAndHideCursor) cameraManager.LockAndHideCursor();
            else cameraManager.UnlockAndShowCursor();
            cameraManager.SetIsActive(activateCamera);
        }
    }
}
