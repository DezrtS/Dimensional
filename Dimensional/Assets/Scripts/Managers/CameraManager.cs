using System;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private bool lockAndHideCursor;
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
            
            if (!lockAndHideCursor) return;
            LockAndHideCursor();
        }
        
        [ContextMenu("Lock and Hide Cursor")]
        private void LockAndHideCursor() => SetCursorState(CursorLockMode.Locked, false);
        [ContextMenu("Unlock and Show Cursor")]
        private void UnlockAndShowCursor() => SetCursorState(CursorLockMode.None, true);

        private static void SetCursorState(CursorLockMode cursorLockMode, bool cursorVisible)
        {
            Cursor.lockState = cursorLockMode;
            Cursor.visible = cursorVisible;
        }

        private void GameManagerOnWorldDimensionsChanged(Dimensions oldValue, Dimensions newValue)
        {
            switch (newValue)
            {
                case Dimensions.Two:
                    _thirdPersonFollow.VerticalArmLength = yOffset2D;
                    _thirdPersonFollow.AvoidObstacles.Enabled = false;
                    _camera.orthographic = true;
                    break;
                case Dimensions.Three:
                    _thirdPersonFollow.VerticalArmLength = 0;
                    _thirdPersonFollow.AvoidObstacles.Enabled = true;
                    _camera.orthographic = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
            }
        }
    }
}
