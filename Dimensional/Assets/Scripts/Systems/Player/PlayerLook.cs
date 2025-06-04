using System;
using Interfaces;
using Managers;
using Scriptables.Player;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private PlayerLookDatum playerLookDatum;
        [SerializeField] private Dimensions lookDimensions;
        [SerializeField] private bool lockCursor;
        [Space] 
        [SerializeField] private Transform root;
        
        private IAim _aim;

        // 2D
        private Vector3 _lookPosition;
        private float _resetTimer;
        
        // 3D
        private float _xRotation;
        private float _yRotation;
        
        public bool IsDisabled { get; set; }
        public float XRotation => _xRotation;

        private void Awake()
        {
            GameManager.WorldDimensionsChanged += GameManagerOnWorldDimensionsChanged;
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
            if (IsDisabled) input = Vector3.zero;
            OnLook(input);
            root.SetLocalPositionAndRotation(_lookPosition, Quaternion.Euler(_yRotation, _xRotation, 0));
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
                    _xRotation += rotationInput.x;
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
                    _xRotation = 0;
                    _yRotation = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null);
            }
            lookDimensions = newValue;
        }
    }
}
