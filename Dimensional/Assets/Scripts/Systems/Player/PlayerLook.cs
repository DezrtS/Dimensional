using System;
using Interfaces;
using Managers;
using Scriptables.Player;
using Systems.Movement;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerLook : MonoBehaviour
    {
        private static readonly int Fade = Shader.PropertyToID("_Fade");
        [SerializeField] private PlayerLookDatum playerLookDatum;
        [SerializeField] private Dimensions lookDimensions;
        [Space] 
        [SerializeField] private bool isDisabled;
        [SerializeField] private Transform root;
        [Space]
        [SerializeField] private float minDitherDistance = 5f;
        [SerializeField] private float maxDitherDistance = 10f;
        [SerializeField] private Material playerMaterial;

        private Transform _cameraTransform;
        private IAim _aim;
        
        private CameraManager _cameraManager;
        private ForceController _forceController;

        // 2D
        private Vector3 _lookPosition;
        private float _resetTimer;
        
        // 3D
        private float _yRotation;

        private bool _cameraDisabled;
        
        public Transform Root => root;
        public float XRotation { get; private set; }

        private void Awake()
        {
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
            _forceController = GetComponent<ForceController>();
            var instance = CameraManager.Instance;
            if (instance)
            {
                CameraManagerOnInitialized(instance);
            }
            else
            {
                CameraManager.Initialized += CameraManagerOnInitialized;
            }
        }

        private void OnDestroy()
        {
            CameraManager.Initialized -= CameraManagerOnInitialized;
        }

        private void CameraManagerOnInitialized(CameraManager instance)
        {
            _cameraTransform = instance.Camera.transform;
            _cameraManager = instance;
            instance.ActiveStateChanged += CameraManagerOnActiveStateChanged;
        }

        private void CameraManagerOnActiveStateChanged(Camera currentCamera, bool active)
        {
            _cameraDisabled = !active;
        }

        private void FixedUpdate()
        {
            var distance = Vector3.Distance(_cameraTransform.position, root.position);
            var ratio = (distance - minDitherDistance) / maxDitherDistance;
            playerMaterial.SetFloat(Fade, Mathf.Clamp01(ratio));
            
            var velocity = _forceController.GetVelocity();
            if (!playerLookDatum.UseYVelocity) velocity.y = 0;
            var speed = velocity.magnitude;
            var speedRatio = (speed - playerLookDatum.MinSpeed) / playerLookDatum.MaxSpeed;
            var fovDifference = playerLookDatum.MaxFOV - playerLookDatum.MinFOV;
            _cameraManager.SetTargetFOV(playerLookDatum.MinFOV + playerLookDatum.FovCurve.Evaluate(Mathf.Clamp01(speedRatio)) * fovDifference);
        }

        public void Initialize(IAim aim)
        {
            _aim = aim;
        }

        public void Look()
        {
            Look(_aim?.GetInput() ?? Vector3.zero);
        }

        private void Look(Vector3 input)
        {
            if (isDisabled || _cameraDisabled) input = Vector3.zero;
            OnLook(input);
            root.SetLocalPositionAndRotation(_lookPosition, Quaternion.Euler(_yRotation, XRotation, 0));
        }

        private void OnLook(Vector3 input)
        {
            var deltaTime = Time.deltaTime;
            var scaledInput = input * deltaTime;
            switch (lookDimensions)
            {
                case Dimensions.Two:
                    if (input.magnitude == 0)
                    {
                        if (_resetTimer > 0)
                        {
                            _resetTimer -= deltaTime;
                            if (_resetTimer <= 0)
                            {
                                _lookPosition = Vector3.zero;
                            }
                        }
                    }
                    else
                    {
                        _lookPosition += scaledInput * playerLookDatum.PositionSensitivity;
                        if (_lookPosition.magnitude > playerLookDatum.Range) _lookPosition = _lookPosition.normalized * playerLookDatum.Range;
                        _resetTimer = playerLookDatum.ResetDelay;
                    }
                    break;
                case Dimensions.Three:
                    var rotationInput = new Vector3(scaledInput.x * playerLookDatum.XSensitivity, scaledInput.y * playerLookDatum.YSensitivity, scaledInput.z);
                    if (playerLookDatum.InvertX) rotationInput.x *= -1;
                    if (playerLookDatum.InvertY) rotationInput.y *= -1;
                    XRotation += rotationInput.x;
                    _yRotation = Mathf.Clamp(_yRotation + rotationInput.y, -playerLookDatum.MaxYAngle, playerLookDatum.MaxYAngle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GameManagerOnWorldDimensionsChanged(Dimensions oldValue, Dimensions newValue)
        {
            switch (oldValue)
            {
                case Dimensions.Two:
                    _lookPosition = Vector3.zero;
                    break;
                case Dimensions.Three:
                    XRotation = 0;
                    _yRotation = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null);
            }
            lookDimensions = newValue;
        }

        public Vector3 TransformInput(Vector3 input)
        {
            return root.rotation * input;
        }
    }
}
