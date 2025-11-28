using Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "CameraEventDatum", menuName = "Scriptable Objects/Events/CameraEventDatum")]
    public class CameraTransitionEventDatum : EventDatum
    {
        [SerializeField] private int cameraId;
        [SerializeField] private float duration;
        [SerializeField] private CinemachineBlendDefinition.Styles style;

        public override void HandleEvent()
        {
            CameraManager.Instance.InvokeTransition(cameraId, duration, style);
        }
    }
}
