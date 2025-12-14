using System;
using Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace Systems.Cameras
{
    public class CameraTransition : MonoBehaviour
    {
        public event Action TransitionToFinished;
        public event Action TransitionFromFinished;
        
        [SerializeField] private float enterDuration;
        [SerializeField] private float exitDuration;
        [SerializeField] private CinemachineCamera cinemachineCamera;

        private bool _isTransitioningTo;
        private bool _isTransitioningFrom;

        private void Start()
        {
            CameraManager.Instance.TransitionFinished += CameraManagerOnTransitionFinished;
        }

        private void CameraManagerOnTransitionFinished(CinemachineCamera from, CinemachineCamera to)
        {
            if (_isTransitioningTo) TransitionToFinished?.Invoke();
            if (_isTransitioningFrom) TransitionFromFinished?.Invoke();
            _isTransitioningTo = false;
            _isTransitioningFrom = false;
        }

        public void TransitionTo()
        {
            _isTransitioningTo = true;
            CameraManager.Instance.Transition(null, cinemachineCamera, enterDuration);
        }

        public void TransitionFrom()
        {
            CameraManager.Instance.Transition(null, cinemachineCamera, 0);
            _isTransitioningFrom = true;
            CameraManager.Instance.TransitionToDefault(true, exitDuration);
        }
    }
}
