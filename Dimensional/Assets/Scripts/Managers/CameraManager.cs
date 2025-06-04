using System;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private float yOffset2D;
        
        private Camera _camera;
        private CinemachineCamera _cinemachineCamera;
        private CinemachineThirdPersonFollow _thirdPersonFollow;

        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
            _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            _thirdPersonFollow = _cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
        }

        private void GameManagerOnWorldDimensionsChanged(Dimensions oldValue, Dimensions newValue)
        {
            switch (newValue)
            {
                case Dimensions.Two:
                    _thirdPersonFollow.VerticalArmLength = yOffset2D;
                    _camera.orthographic = true;
                    break;
                case Dimensions.Three:
                    _thirdPersonFollow.VerticalArmLength = 0;
                    _camera.orthographic = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
            }
        }
    }
}
