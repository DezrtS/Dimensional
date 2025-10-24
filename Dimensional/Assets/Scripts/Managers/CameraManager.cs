using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        public delegate void CameraActiveEventHandler(Camera camera, bool active);
        public event CameraActiveEventHandler ActiveStateChanged;
        public delegate void CameraTransitionEventHandler(CinemachineCamera from, CinemachineCamera to);
        public event CameraTransitionEventHandler TransitionStarted;
        public event CameraTransitionEventHandler TransitionFinished;
        
        [SerializeField] private bool lockAndHideCursor;
        [SerializeField] private Vector3 thirdPersonDamping;
        [SerializeField] private float yOffset2D;

        private CinemachineBrain _cinemachineBrain;
        private CinemachineCamera _cinemachineCamera;
        private CinemachineThirdPersonFollow _thirdPersonFollow;
        private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
        
        private Coroutine _transitionCoroutine;
        
        private float _shakeTimer;
        
        private float _shakeDuration;
        private float _shakeAmplitude;
        private AnimationCurve _shakeAmplitudeCurve;
        private float _shakeFrequency;
        private AnimationCurve _shakeFrequencyCurve;
        
        public Camera Camera { get; private set; }
        public CinemachineCamera TargetCinemachineCamera { get; private set; }

        public override void InitializeSingleton()
        {
            Camera = GetComponentInChildren<Camera>();
            _cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
            _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            TargetCinemachineCamera = _cinemachineCamera;
            _thirdPersonFollow = _cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
            _thirdPersonFollow.Damping = thirdPersonDamping;
            _cinemachineBasicMultiChannelPerlin = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            StopScreenShake();
            base.InitializeSingleton();
        }

        private void Awake()
        {
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            if (lockAndHideCursor) LockAndHideCursor();
        }

        private void FixedUpdate()
        {
            if (_shakeTimer <= 0) return;
            var ratioTime = 1 - _shakeTimer / _shakeDuration;
            _cinemachineBasicMultiChannelPerlin.AmplitudeGain = _shakeAmplitude * _shakeAmplitudeCurve.Evaluate(ratioTime);
            _cinemachineBasicMultiChannelPerlin.FrequencyGain = _shakeFrequency * _shakeFrequencyCurve.Evaluate(ratioTime);
            _shakeTimer -= Time.fixedDeltaTime;
            if (_shakeTimer > 0) return;
            StopScreenShake();
        }

        public void SetIsActive(bool isActive)
        {
            ActiveStateChanged?.Invoke(Camera, isActive);
        }

        public void SetFollow(Transform follow) => _cinemachineCamera.Follow = follow;
        public void SetLookAt(Transform lookAt) => _cinemachineCamera.LookAt = lookAt;
        
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

        public void TriggerScreenShake(float duration, float amplitude, AnimationCurve amplitudeCurve, float frequency, AnimationCurve frequencyCurve)
        {
            _shakeTimer = duration;
            _shakeDuration = duration;
            _shakeAmplitude = amplitude;
            _shakeAmplitudeCurve = amplitudeCurve;
            _shakeFrequency = frequency;
            _shakeFrequencyCurve = frequencyCurve;
        }

        public void StopScreenShake()
        {
            _shakeTimer = 0;
            _cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
            _cinemachineBasicMultiChannelPerlin.FrequencyGain = 0;
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
            GameManager.WorldDimensionsChanged -= GameManagerOnWorldDimensionsChanged;
        }
    }
}
