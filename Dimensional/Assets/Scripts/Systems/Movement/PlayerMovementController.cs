using System;
using System.Collections;
using Managers;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public class PlayerMovementController : MovementController
    {
        public enum MovementState
        {
            Sphere,
            Spring,
            Boomerang,
            Heavy,
        }
        
        public delegate void ShapeTypeChangeEventHandler(MovementState oldValue, MovementState newValue);
        public event ShapeTypeChangeEventHandler ShapeTypeChanged;
        
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;

        [Space] [SerializeField] private GameObject smokePrefab;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        
        private bool _isJumping;
        private float _jumpTimer;

        private bool _isSpringJump;
        private bool _isBalloonJump;

        private float _afterGroundPoundTimer;
        
        private bool _isCrouching;

        private bool _isGroundPounding;
        private float _groundPoundTimer;
        
        private bool _isBoomerang;
        private float _boomerangTimer;
        private float _boomerangMultiplier;

        private bool _isRolling;

        private bool _isAttacking;
        private bool _attacked;
        private float _attackTimer;

        private float _queueJumpTimer;
        private float _coyoteTimer;

        protected override void Awake()
        {
            base.Awake();
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
        }
        
        private void FixedUpdate()
        {
            var velocity = ForceController.GetVelocity();
            root.localScale = Vector3.Lerp(root.localScale, new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), Time.fixedDeltaTime * scaleSpeed);
            velocity.y = 0;
            if (velocity.magnitude > 0.1f) root.forward = velocity;

            if (_queueJumpTimer > 0)
            {
                if (IsGrounded && !_isGroundPounding)
                {
                    _queueJumpTimer = 0;
                    Jump();
                }
                _queueJumpTimer -= Time.fixedDeltaTime;
            }

            if (_afterGroundPoundTimer > 0)
            {
                _afterGroundPoundTimer -= Time.fixedDeltaTime;
            }

            if (_isBalloonJump)
            {
                _isBalloonJump = !IsGrounded;
            }
            
            if (_attacked) _attacked = !IsGrounded;

            if (_isJumping)
            {
                if (_isSpringJump) HandleJump(_playerMovementControllerDatum.SpringJumpTime, _playerMovementControllerDatum.SpringJumpHeight, _playerMovementControllerDatum.SpringJumpCurve);
                else if (_isBalloonJump) HandleJump(_playerMovementControllerDatum.BalloonJumpTime, _playerMovementControllerDatum.BalloonJumpHeight, _playerMovementControllerDatum.BalloonJumpCurve);
                else HandleJump(_playerMovementControllerDatum.JumpTime, _playerMovementControllerDatum.JumpHeight, _playerMovementControllerDatum.JumpCurve);
            }
            else if (_isGroundPounding) HandleCrouch();
            else if (_isBoomerang) HandleBoomerang();
            else if (_isAttacking) HandleAttacking();
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            base.OnMove(input, _isRolling ? _playerMovementControllerDatum.RollMovementControllerDatum : datum);
        }

        public void Jump()
        {
            if (_isAttacking) return;
            if (!IsGrounded)
            {
                if (_isBalloonJump || _isGroundPounding)
                {
                    _queueJumpTimer = _playerMovementControllerDatum.QueueJumpTime;
                    return;
                }
                _isJumping = true;
                _isRolling = false;
                _isBalloonJump = true;
                _jumpTimer = 0;
                ForceController.UseGravity = false;
                Instantiate(smokePrefab, root.position, Quaternion.identity);
                return;
            }

            if (_isGroundPounding) return;

            if (_isCrouching && !_isRolling)
            {
                _isRolling = true;
                ForceController.ApplyForce(root.rotation * Vector3.forward * _playerMovementControllerDatum.InitialRollSpeed, ForceMode.VelocityChange);
                return;
            }
            
            _isJumping = true;
            _jumpTimer = 0;
            ForceController.UseGravity = false;
            Instantiate(smokePrefab, root.position, Quaternion.identity);

            if (!(_afterGroundPoundTimer > 0)) return;
            _isSpringJump = true;
            _afterGroundPoundTimer = 0;
        }

        public void StopJumping()
        {
            if (!_isJumping) return;

            _isJumping = false;
            ForceController.UseGravity = true;

            _isSpringJump = false;
            
            var velocity = ForceController.GetVelocity();
            velocity.y *= _playerMovementControllerDatum.CutJumpMultiplier;
            ForceController.SetVelocity(velocity);
        }
        
        private void HandleJump(float jumpTime, float jumpHeight, AnimationCurve jumpCurve)
        {
            if (_jumpTimer > jumpTime)
            {
                _isJumping = false;
                ForceController.UseGravity = true;
                _isSpringJump = false;
                return;
            }
            
            var normalizedTime = _jumpTimer / jumpTime;
            var slope = GameManager.Derivative(jumpCurve, normalizedTime);
            var verticalVelocity = slope * (jumpHeight / jumpTime);
            
            var currentVelocity = ForceController.GetVelocity();
            ForceController.SetVelocity(new Vector3(
                currentVelocity.x,
                verticalVelocity,
                currentVelocity.z
            ));
            _jumpTimer += Time.fixedDeltaTime;
        }

        public void Crouch()
        {
            _isCrouching = true;
            if (IsGrounded || _isGroundPounding || _isAttacking) return;
            _isGroundPounding = true;
            _isRolling = false;
            _groundPoundTimer = 0;
            ForceController.UseGravity = false;
        }

        public void StopCrouching()
        {
            _isCrouching = false;
        }

        private void HandleCrouch()
        {
            if (IsGrounded)
            {
                _isGroundPounding = false;
                ForceController.UseGravity = true;
                _afterGroundPoundTimer = _playerMovementControllerDatum.AfterGroundPoundTime;
                return;
            }
            
            var groundPoundSpeed = _playerMovementControllerDatum.GroundPoundSpeed;
            var groundPoundTime = _playerMovementControllerDatum.GroundPoundTime;
            var groundPoundCurve = _playerMovementControllerDatum.GroundPoundCurve;
            
            var normalizedTime = Mathf.Min(_groundPoundTimer / groundPoundTime, 1);
            var verticalVelocity = groundPoundCurve.Evaluate(normalizedTime) * groundPoundSpeed;
            
            ForceController.SetVelocity(new Vector3(
                0,
                verticalVelocity,
                0
            ));
            _groundPoundTimer += Time.fixedDeltaTime;
        }

        public void Boomerang()
        {
            if (IsGrounded || _isAttacking) return;
            
            _isBoomerang = true;
            _boomerangTimer = 0;
            ForceController.UseGravity = false;
            
            var maxBoomerangSpeed = _playerMovementControllerDatum.MaxBoomerangSpeed;
            var velocity = ForceController.GetVelocity();
            
            _boomerangMultiplier = 1 - Mathf.Clamp(Mathf.Abs(velocity.y) / maxBoomerangSpeed, 0, 0.9f);

            _isJumping = false;
            _isSpringJump = false;
            _isGroundPounding = false;
        }

        public void StopBoomerang()
        {
            if (!_isBoomerang) return;
            
            _isBoomerang = false;
            ForceController.UseGravity = true;
        }

        private void HandleBoomerang()
        {
            var boomerangHeight = _playerMovementControllerDatum.BoomerangHeight * _boomerangMultiplier;
            var boomerangTime = _playerMovementControllerDatum.BoomerangTime * _boomerangMultiplier;
            var boomerangCurve = _playerMovementControllerDatum.BoomerangCurve;
            
            var normalizedTime = Mathf.Min(_boomerangTimer / boomerangTime, 1);
            var slope = GameManager.Derivative(boomerangCurve, normalizedTime);
            var verticalVelocity = slope * (boomerangHeight / boomerangTime);
            
            var currentVelocity = ForceController.GetVelocity();
            ForceController.SetVelocity(new Vector3(
                currentVelocity.x,
                verticalVelocity,
                currentVelocity.z
            ));
            _boomerangTimer += Time.fixedDeltaTime;
        }

        public void Attack()
        {
            if (_isAttacking || _attacked) return;
            _isAttacking = true;
            _attacked = true;
            _attackTimer = 0;
            
            _isJumping = false;
            _isSpringJump = false;
            _isGroundPounding = false;
            _isRolling = false;
            _isBoomerang = false;
            ForceController.UseGravity = false;
        }

        private void HandleAttacking()
        {
            if (_attackTimer > _playerMovementControllerDatum.AttackTime)
            {
                _isAttacking = false;
                ForceController.UseGravity = true;
                return;
            }
            
            var normalizedTime = _attackTimer / _playerMovementControllerDatum.AttackTime;
            var slope = GameManager.Derivative(_playerMovementControllerDatum.AttackCurve, normalizedTime);
            var velocity = slope * (_playerMovementControllerDatum.AttackVector / _playerMovementControllerDatum.AttackTime);
            
            var newVelocity = root.rotation * velocity;
            ForceController.SetVelocity(newVelocity);
            _attackTimer += Time.fixedDeltaTime;
        }
    }
}
