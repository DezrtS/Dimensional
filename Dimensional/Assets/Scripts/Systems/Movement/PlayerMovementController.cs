using System;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public class PlayerMovementController : MovementController
    {
        public enum ShapeType
        {
            Sphere,
            Spring,
            Boomerang,
            Heavy,
        }
        
        public delegate void ShapeTypeChangeEventHandler(ShapeType oldValue, ShapeType newValue);
        public event ShapeTypeChangeEventHandler ShapeTypeChanged;
        
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;

        [SerializeField] private float boomerangMultiplier = 5;
        [SerializeField] private float boomerangTime = 1;
        
        [SerializeField] private float heavyMultiplier = 5;

        [SerializeField] private float springTime = 2;
        [SerializeField] private float springMultiplier = 2;
        [SerializeField] private float springScale = 0.5f;

        [Space] [SerializeField] private GameObject smokePrefab;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private ShapeType _shapeType;

        private bool _isCrouching;
        private float _springTimer = 0;
        private float _boomerangTimer = 0;

        private float _boomerangPower;

        private bool _wasGrounded;
        private bool _spawnSmoke = false;

        protected override void Awake()
        {
            base.Awake();
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
        }
        
        private void FixedUpdate()
        {
            var velocity = ForceController.GetVelocity();
            _spawnSmoke = Mathf.Abs(velocity.y) > 3f;
            if (_spawnSmoke && !_wasGrounded && IsGrounded) Instantiate(smokePrefab, root.position, Quaternion.identity);
            _wasGrounded = IsGrounded;
            velocity.y = 0;
            if (velocity.magnitude > 0.1f) root.forward = velocity;
            OnShapeType();
        }

        public void ChangeShape(ShapeType shapeType)
        {
            if (_shapeType == shapeType) return;
            ShapeTypeChanged?.Invoke(_shapeType, shapeType);
            _shapeType = shapeType;

            ForceController.UseGravity = true;
            switch (_shapeType)
            {
                case ShapeType.Sphere:
                    break;
                case ShapeType.Spring:
                    _springTimer = springTime;
                    break;
                case ShapeType.Boomerang:
                    _boomerangTimer = boomerangTime;
                    var velocity = ForceController.GetVelocity();
                    _boomerangPower = -velocity.y;
                    ForceController.UseGravity = false;
                    break;
                case ShapeType.Heavy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnShapeType()
        {
            switch (_shapeType)
            {
                case ShapeType.Sphere or ShapeType.Heavy:
                {
                    var velocity = ForceController.GetVelocity();
                    root.localScale = Vector3.Lerp(root.localScale, new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), Time.fixedDeltaTime * scaleSpeed);
                    if (_shapeType == ShapeType.Heavy && !IsGrounded) ForceController.ApplyForce(Vector3.down * heavyMultiplier, ForceMode.Force);
                    break;
                }
                case ShapeType.Spring:
                    if (_springTimer > 0)
                    {
                        _springTimer -= Time.fixedDeltaTime;
                        if (_springTimer <= 0)
                        {
                            _springTimer = 0;
                        }
                    }

                    var scale = (1 - _springTimer / springTime);
                    root.localScale = Vector3.Lerp(root.localScale, new Vector3(1 + springScale * scale, 1 - springScale * scale, 1 + springScale * scale), Time.fixedDeltaTime * scaleSpeed);
                    break;
                case ShapeType.Boomerang:
                    root.localScale = Vector3.Lerp(root.localScale, Vector3.one, Time.fixedDeltaTime * scaleSpeed);
                    if (_boomerangTimer > 0)
                    {
                        _boomerangTimer -= Time.fixedDeltaTime;
                        ForceController.ApplyForce(new Vector3(0, _boomerangPower, 0), ForceMode.Force);
                        if (IsGrounded) _boomerangTimer = 0;
                    }
                    else
                    {
                        if (!IsGrounded) ForceController.ApplyForce(Vector3.down * boomerangMultiplier, ForceMode.Force);   
                    }
                    break;
            }
        }

        public void Jump()
        {
            if (IsGrounded)
            {
                if (_isCrouching)
                {
                    ChangeShape(ShapeType.Spring);
                }
                else
                {
                    if (_shapeType == ShapeType.Heavy) ChangeShape(ShapeType.Sphere);
                    Instantiate(smokePrefab, root.position, Quaternion.identity);
                    ForceController.ApplyForce(Vector3.up * _playerMovementControllerDatum.JumpPower, ForceMode.Impulse);   
                }
            }
            else
            {
                ChangeShape(ShapeType.Boomerang);
            }
        }

        public void StopJumping()
        {
            switch (_shapeType)
            {
                case ShapeType.Boomerang:
                    ChangeShape(ShapeType.Sphere);
                    break;
                case ShapeType.Spring:
                    SpringJump();
                    break;
            }
        }

        private void SpringJump()
        {
            ForceController.ApplyForce(Vector3.up * _playerMovementControllerDatum.JumpPower * springMultiplier * (1 - _springTimer / springTime), ForceMode.Impulse);
            Instantiate(smokePrefab, root.position, Quaternion.identity);
            ChangeShape(ShapeType.Sphere);
        }

        public void Crouch()
        {
            if (IsGrounded)
            {
                _isCrouching = true;
            }
            else
            {
                ChangeShape(ShapeType.Heavy);   
            }
        }

        public void StopCrouching()
        {
            _isCrouching = false;
            if (_shapeType == ShapeType.Spring) ChangeShape(ShapeType.Sphere);
        }
    }
}
