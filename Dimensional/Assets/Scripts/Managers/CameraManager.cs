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

        private CinemachineCamera _cinemachineCamera;
        private CinemachineThirdPersonFollow _thirdPersonFollow;
        
        public Camera Camera { get; private set; }

        private void Awake()
        {
            Camera = GetComponentInChildren<Camera>();
            _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            _thirdPersonFollow = _cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            
            if (!lockAndHideCursor) return;
            LockAndHideCursor();
        }
        
        [ContextMenu("Lock and Hide Cursor")]
        public void LockAndHideCursor() => SetCursorState(CursorLockMode.Locked, false);
        [ContextMenu("Unlock and Show Cursor")]
        public void UnlockAndShowCursor() => SetCursorState(CursorLockMode.None, true);

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
                    Camera.orthographic = true;
                    break;
                case Dimensions.Three:
                    _thirdPersonFollow.VerticalArmLength = 0;
                    _thirdPersonFollow.AvoidObstacles.Enabled = true;
                    Camera.orthographic = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
            }
        }

        private void OnDisable()
        {
            GameManager.WorldDimensionsChanged -= GameManagerOnWorldDimensionsChanged;
        }
    }
}
