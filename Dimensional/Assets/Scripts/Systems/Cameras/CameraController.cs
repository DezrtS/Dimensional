using System;
using Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace Systems.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private int id;
        private CinemachineCamera _cinemachineCamera;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            CameraManager.Instance.TransitionInvoked += CameraManagerOnTransitionInvoked;
            CameraManager.Instance.CinemachineCameraChanged += CameraManagerOnCinemachineCameraChanged;
        }

        private void CameraManagerOnCinemachineCameraChanged(int cameraId)
        {
            if (cameraId != id) return;
            CameraManager.Instance.SetCinemachineCamera(_cinemachineCamera);
        }

        private void CameraManagerOnTransitionInvoked(int cameraId, float duration, CinemachineBlendDefinition.Styles style)
        {
            if (cameraId != id) return;
            CameraManager.Instance.Transition(null, _cinemachineCamera, duration, style);
        }
    }
}
