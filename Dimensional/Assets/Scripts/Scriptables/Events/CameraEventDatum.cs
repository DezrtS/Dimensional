using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "CameraEventDatum", menuName = "Scriptable Objects/CameraEventDatum")]
    public class CameraEventDatum : EventDatum
    {
        [SerializeField] private int cameraId;
        
        public override void HandleEvent()
        {
            CameraManager.Instance.InvokeSetCinemachineCamera(cameraId);
        }
    }
}
