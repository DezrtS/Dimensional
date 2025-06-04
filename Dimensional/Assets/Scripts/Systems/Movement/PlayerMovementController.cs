using System;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public class PlayerMovementController : RigidbodyMovementController
    {
        public enum ShapeType
        {
            Sphere,
            Spring,
            Boomarang,
            Heavy,
        }
        
        public delegate void ShapeTypeChangeEventHandler(ShapeType oldValue, ShapeType newValue);
        public event ShapeTypeChangeEventHandler ShapeTypeChanged;
        
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;

        [SerializeField] private float boomarangMultiplier = 5;
        [SerializeField] private float boomarangTime = 1;
        
        [SerializeField] private float heavyMultiplier = 5;

        [SerializeField] private float springTime = 2;
        [SerializeField] private float springMultiplier = 2;
        [SerializeField] private float springScale = 0.5f;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private ShapeType _shapeType;

        private bool _isCrouching;
        private float _springTimer = 0;
        private float _boomarangTimer = 0;

        private float _boomarangPower;

        protected override void Awake()
        {
            base.Awake();
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
        }
        
        private void FixedUpdate()
        {
            var velocity = ForceController.GetVelocity();
            velocity.y = 0;
            if (velocity.magnitude > 0.1f) root.forward = velocity;
            OnShapeType();
        }

        public void ChangeShape(ShapeType shapeType)
        {
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
                case ShapeType.Boomarang:
                    _boomarangTimer = boomarangTime;
                    var velocity = ForceController.GetVelocity();
                    _boomarangPower = -velocity.y;
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
                case ShapeType.Boomarang:
                    root.localScale = Vector3.Lerp(root.localScale, Vector3.one, Time.fixedDeltaTime * scaleSpeed);
                    if (_boomarangTimer > 0)
                    {
                        _boomarangTimer -= Time.fixedDeltaTime;
                        ForceController.ApplyForce(new Vector3(0, _boomarangPower, 0), ForceMode.Force);
                        if (IsGrounded) _boomarangTimer = 0;
                    }
                    else
                    {
                        if (!IsGrounded) ForceController.ApplyForce(Vector3.down * boomarangMultiplier, ForceMode.Force);   
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
                    ForceController.ApplyForce(Vector3.up * _playerMovementControllerDatum.JumpPower, ForceMode.Impulse);   
                }
            }
            else
            {
                ChangeShape(ShapeType.Boomarang);
            }
        }

        public void StopJumping()
        {
            switch (_shapeType)
            {
                case ShapeType.Boomarang:
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
