using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Managers
{
    [Serializable]
    public class ScreenShakeEvent
    {
        private ScreenShakeEventData _screenShakeEventData;
        private readonly float _startTime;
        
        public ScreenShakeEvent(ScreenShakeEventData screenShakeEventData)
        {
            _screenShakeEventData = screenShakeEventData;
            _startTime = Time.time;
        }
        
        public bool IsFinished()
        {
            return _screenShakeEventData.HasDuration && _screenShakeEventData.Duration < Time.time - _startTime;
        }

        private float GetElapsedTimeRatio()
        {
            return (Time.time - _startTime) / _screenShakeEventData.Duration;
        }
        
        public float GetAmplitude()
        {
            return _screenShakeEventData.Amplitude * _screenShakeEventData.AmplitudeCurve.Evaluate(GetElapsedTimeRatio());
        }

        public float GetFrequency()
        {
            return _screenShakeEventData.Frequency * _screenShakeEventData.FrequencyCurve.Evaluate(GetElapsedTimeRatio());
        }
    }
    
    [Serializable]
    public struct ScreenShakeEventData
    {
        [SerializeField] private bool hasDuration;
        [SerializeField] private float duration;
        
        [SerializeField] private float amplitude;
        [SerializeField] private AnimationCurve amplitudeCurve;
        [SerializeField] private float frequency;
        [SerializeField] private AnimationCurve frequencyCurve;
        
        public bool HasDuration => hasDuration;
        public float Duration => duration;
        public float Amplitude => amplitude;
        public AnimationCurve AmplitudeCurve => amplitudeCurve;
        public float Frequency => frequency;
        public AnimationCurve FrequencyCurve => frequencyCurve;
    }
    
    public class CameraManager : Singleton<CameraManager>
    {
        public delegate void CameraActiveEventHandler(Camera camera, bool active);
        public event CameraActiveEventHandler ActiveStateChanged;
        public delegate void CameraTransitionEventHandler(CinemachineCamera from, CinemachineCamera to);
        public event CameraTransitionEventHandler TransitionStarted;
        public event CameraTransitionEventHandler TransitionFinished;
        public delegate void InvokeCameraTransitionEventHandler(int cameraId, float duration, CinemachineBlendDefinition.Styles style);
        public event InvokeCameraTransitionEventHandler TransitionInvoked;
        public delegate void InvokeCameraEventHandler(int cameraId);
        public event InvokeCameraEventHandler CinemachineCameraChanged;
        
        [SerializeField] private bool lockAndHideCursor;
        [SerializeField] private Vector3 thirdPersonDamping;
        [SerializeField] private float yOffset2D;
        [Space]
        [SerializeField] private float defaultFOV;
        [SerializeField] private float fovInterpolationSpeed;
        [Space] 
        [SerializeField] private float maxGamepadMotorAmplitude;

        private CinemachineBrain _cinemachineBrain;
        private CinemachineCamera _cinemachineCamera;
        private CinemachineThirdPersonFollow _thirdPersonFollow;
        private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
        
        private Coroutine _transitionCoroutine;

        private float _currentFOV;
        private float _targetFOV;
        
        private List<ScreenShakeEvent> _screenShakeEvents;
        
        public Camera Camera { get; private set; }
        public CinemachineCamera TargetCinemachineCamera { get; private set; }

        public override void InitializeSingleton()
        {
            Camera = GetComponentInChildren<Camera>();
            _cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
            _cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
            _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            TargetCinemachineCamera = _cinemachineCamera;
            _thirdPersonFollow = _cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
            _thirdPersonFollow.Damping = thirdPersonDamping;
            _cinemachineBasicMultiChannelPerlin = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            SetFOV(defaultFOV);
            _screenShakeEvents = new List<ScreenShakeEvent>();
            StopAllScreenShake();
            base.InitializeSingleton();
        }

        private void Awake()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            if (lockAndHideCursor) LockAndHideCursor();
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Playing) return;
            _cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            _currentFOV = Mathf.Lerp(_currentFOV, _targetFOV, fixedDeltaTime * fovInterpolationSpeed);
            _cinemachineCamera.Lens.FieldOfView = _currentFOV;

            HandleScreenShake();
        }

        public void SetIsActive(bool isActive)
        {
            ActiveStateChanged?.Invoke(Camera, isActive);
        }

        public void SetCinemachineCamera(CinemachineCamera cinemachineCamera)
        {
            _cinemachineCamera = cinemachineCamera;
        }

        public void InvokeSetCinemachineCamera(int cameraId)
        {
            CinemachineCameraChanged?.Invoke(cameraId);
        }

        public void SetFollow(Transform follow) => _cinemachineCamera.Follow = follow;
        public void SetLookAt(Transform lookAt) => _cinemachineCamera.LookAt = lookAt;

        public void SetFOV(float fov) => _currentFOV = fov;
        public void SetTargetFOV(float fov) => _targetFOV = fov;
        
        [ContextMenu("Lock and Hide Cursor")]
        public void LockAndHideCursor() => SetCursorState(CursorLockMode.Locked, false);
        [ContextMenu("Unlock and Show Cursor")]
        public void UnlockAndShowCursor() => SetCursorState(CursorLockMode.None, true);

        private static void SetCursorState(CursorLockMode cursorLockMode, bool cursorVisible)
        {
            Cursor.lockState = cursorLockMode;
            Cursor.visible = cursorVisible;
        }
        
        public void EnableDamping() => _thirdPersonFollow.Damping = thirdPersonDamping;
        public void DisableDamping() => _thirdPersonFollow.Damping = Vector3.zero;

        private void HandleScreenShake()
        {
            if (_screenShakeEvents.Count <= 0) return;
            ScreenShakeEvent currentScreenShakeEvent = null;
            for (var i = 0; i < _screenShakeEvents.Count; i++)
            {
                if (_screenShakeEvents[i].IsFinished())
                {
                    _screenShakeEvents.RemoveAt(i);
                    i--;
                    continue;
                }

                if (currentScreenShakeEvent == null)
                {
                    currentScreenShakeEvent = _screenShakeEvents[i];
                    continue;
                }
                
                if (_screenShakeEvents[i].GetAmplitude() > currentScreenShakeEvent.GetAmplitude()) currentScreenShakeEvent = _screenShakeEvents[i];
            }

            if (_screenShakeEvents.Count > 0 && currentScreenShakeEvent != null)
            {
                var amplitude = currentScreenShakeEvent.GetAmplitude();
                _cinemachineBasicMultiChannelPerlin.AmplitudeGain = amplitude;
                _cinemachineBasicMultiChannelPerlin.FrequencyGain = currentScreenShakeEvent.GetFrequency();
                if (Gamepad.current != null)
                {
                    Gamepad.current.SetMotorSpeeds(amplitude / maxGamepadMotorAmplitude, amplitude / maxGamepadMotorAmplitude);
                }
            }
            else
            {
                StopAllScreenShake();
            }
        }

        public ScreenShakeEvent TriggerScreenShake(ScreenShakeEventData screenShakeEventData)
        {
            var screenShakeEvent = new ScreenShakeEvent(screenShakeEventData);
            _screenShakeEvents.Add(screenShakeEvent);
            HandleScreenShake();
            return screenShakeEvent;
        }

        public void RemoveScreenShakeEvent(ScreenShakeEvent screenShakeEvent)
        {
            _screenShakeEvents.Remove(screenShakeEvent);
            if (_screenShakeEvents.Count <= 0) StopAllScreenShake();
        }

        public void StopAllScreenShake()
        {
            _screenShakeEvents.Clear();
            _cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
            _cinemachineBasicMultiChannelPerlin.FrequencyGain = 0;
            if (Gamepad.current != null) Gamepad.current.ResetHaptics();
        }

        public void Transition(CinemachineCamera from, CinemachineCamera to, float duration, CinemachineBlendDefinition.Styles style = CinemachineBlendDefinition.Styles.EaseInOut)
        {
            if (to == TargetCinemachineCamera || from == to) return;
            if (!from) from = TargetCinemachineCamera;
            if (!to) to = TargetCinemachineCamera;
            if (duration <= 0) style = CinemachineBlendDefinition.Styles.Cut;
            
            if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionRoutine(from, to, duration, style));
        }

        public void InvokeTransition(int cameraId, float duration, CinemachineBlendDefinition.Styles style)
        {
            TransitionInvoked?.Invoke(cameraId, duration, style);
        }

        public void TransitionToDefault(bool waitForEndOfFrame, float duration, CinemachineBlendDefinition.Styles style = CinemachineBlendDefinition.Styles.EaseInOut)
        {
            if (waitForEndOfFrame) StartCoroutine(DefaultTransitionRoutine(duration, style));
            else Transition(TargetCinemachineCamera, _cinemachineCamera, duration, style);
        }

        private IEnumerator DefaultTransitionRoutine(float duration, CinemachineBlendDefinition.Styles style)
        {
            yield return new WaitForEndOfFrame();
            Transition(TargetCinemachineCamera, _cinemachineCamera, duration, style);
        }
        
        private IEnumerator TransitionRoutine(CinemachineCamera from, CinemachineCamera to, float duration, CinemachineBlendDefinition.Styles style)
        {
            SetIsActive(false);
            if (TargetCinemachineCamera && TargetCinemachineCamera != from)
            {
                _cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
                from.Priority = 1;
                TargetCinemachineCamera.Priority = -1;
                yield return new WaitForEndOfFrame();
            }
            
            TransitionStarted?.Invoke(from, to);
            TargetCinemachineCamera = to;
            _cinemachineBrain.DefaultBlend.Style = style;
            _cinemachineBrain.DefaultBlend.Time = duration;
            to.Priority = 1;
            from.Priority = -1;
            yield return new WaitForSeconds(duration);
            yield return new WaitForEndOfFrame();
            TransitionFinished?.Invoke(from, to);
            SetIsActive(true);
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
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            GameManager.WorldDimensionsChanged -= GameManagerOnWorldDimensionsChanged;
            StopAllScreenShake();
        }
    }
}
