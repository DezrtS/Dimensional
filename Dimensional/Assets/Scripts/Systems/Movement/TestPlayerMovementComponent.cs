using System;
using Managers;
using Scriptables.Actions;
using Scriptables.Movement;
using Systems.Movement.Components;
using Systems.Player;
using UnityEngine;

namespace Systems.Movement
{
    public class TestPlayerMovementComponent : MovementController
    {
        //public delegate void ActionTriggerHandler(Scriptables.Shapes.Type shapeType, ActionContext actionContext);
        //public event ActionTriggerHandler ActionTriggered;
        
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerController _playerController;
        private PlayerLook _playerLook;
        
        private PositionMovementComponent _positionMovementComponent;
        private WallSlideMovementComponent _wallSlideMovementComponent;

        private float _queueJumpTimer;
        private float _coyoteTimer;
        private float _wallCoyoteTimer;
        private float _springJumpTimer;
        
        public bool IsWallSliding { get; set; }
        public bool IsJumping { get; set; }
        public bool IsDashing { get; set; }
        
        public bool CanDoubleJump { get; set; }
        public bool CanDash { get; set; }
        
        public Vector3 WallSlideDirection { get; private set; }

        protected override void OnAwake()
        {
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
            _playerController = GetComponent<PlayerController>();
            _playerLook = GetComponent<PlayerLook>();
            
            _positionMovementComponent = GetComponent<PositionMovementComponent>();
            _wallSlideMovementComponent = GetComponent<WallSlideMovementComponent>();
            
            _positionMovementComponent.Initialize(this);
            _wallSlideMovementComponent.Initialize(this);
            
            _wallSlideMovementComponent.Deactivated += WallSlideMovementComponentOnDeactivated;
            _wallSlideMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _wallSlideMovementComponent.SetWallSlideMovementData(_playerMovementControllerDatum.WallSlideSpeed, _playerMovementControllerDatum.WallSlideTime, _playerMovementControllerDatum.WallSlideCurve, _playerMovementControllerDatum.WallSlideCheckOffset, _playerMovementControllerDatum.WallSlideCheckDistance, _playerMovementControllerDatum.WallSlideCheckLayerMask);
            
            Grounded += OnGrounded;
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            
            if (_queueJumpTimer > 0) _queueJumpTimer -= fixedDeltaTime;
            if (_coyoteTimer > 0) _coyoteTimer -= fixedDeltaTime;
        }

        private void OnGrounded(bool isGrounded)
        {
            if (isGrounded && _queueJumpTimer > 0);
        }

        public void TriggerAction(Type actionType, bool activated)
        {
            //if (IsWallSliding || CheckWallCoyoteTime()) actionType = Type.WallJumpAction;
            
            //var actionShape = Array.Find(_playerController.ActionShapes, x => x.ActionType == actionType);
            //ActionTriggered?.Invoke(actionShape.ShapeType, new ActionContext() { Type = actionType, Activated = activated });
        }
        
        public bool CheckQueueJump() => _queueJumpTimer > 0;
        public bool CheckCoyoteTime() => _coyoteTimer > 0;
        public bool CheckWallCoyoteTime() => _wallCoyoteTimer > 0;
        public bool CheckSpringJump() => _springJumpTimer > 0;
        
        public Vector3 GetForward()
        {
            var forward = Mover?.GetInput() ?? Vector3.zero;
            forward = forward.magnitude < 0.5f ? root.forward : _playerLook.TransformInput(forward);
            forward.y = 0;
            
            if (MovementDimensions != Dimensions.Two) return forward;
            forward.z = 0;
            if (forward.x == 0) forward.x = 1;
            return forward;
        }

        public bool CanTriggerAction(Type actionType)
        {
            switch (actionType)
            {
                //case Type.JumpAction:
                //    return CheckQueueJump();
            }

            return false;
        }

        public void CutJump()
        {
            if (!IsJumping) return;
            
            _positionMovementComponent.SetOverrideUpVelocity(false);
            var velocity = ForceController.GetVelocity();
            velocity.y *= _playerMovementControllerDatum.CutJumpMultiplier;
            ForceController.SetVelocity(velocity);
            ForceController.UseGravity = true;
        }
        
        protected override void OnUpdate()
        {
            WallSlide();
            if (!IsWallSliding) return;
            CheckWallPlatform();
        }
        
        public void WallSlide()
        {
            if (IsGrounded) return;
            if (CheckWallSlide()) OnWallSlide();
        }

        private bool CheckWallSlide()
        {
            //if (disableWallSlideCheck) return false;
            
            var velocity = ForceController.GetVelocity();
            if (!(velocity.y < _playerMovementControllerDatum.WallSlideYVelocityThreshold)) return false;
            velocity.y = 0;

            var input = _playerLook.TransformInput(Mover.GetInput());
            if (input.magnitude < 0.5f) return false;
            
            input.y = 0;
            if (MovementDimensions == Dimensions.Two) input.z = 0;
            
            input = input.normalized;
            var intervalAngle = (int)(360 / _playerMovementControllerDatum.WallSlideCheckIntervals);
            for (var i = 0; i < _playerMovementControllerDatum.WallSlideCheckIntervals; i++)
            {
                var direction = Quaternion.Euler(0, i * intervalAngle, 0) * Vector3.forward;
                if (Vector3.Dot(input, direction) < 0) continue;
                if (!Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset,
                        direction, _playerMovementControllerDatum.WallSlideCheckDistance,
                        _playerMovementControllerDatum.WallSlideCheckLayerMask, QueryTriggerInteraction.Ignore)) continue;
                WallSlideDirection = direction;
                
                return true;
            }
            return false;
        }

        private void CheckWallPlatform()
        {
            if (Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset,
                    WallSlideDirection, out var hit, _playerMovementControllerDatum.WallSlideCheckDistance,
                    platformCheckLayerMask, QueryTriggerInteraction.Ignore))
            {
                Platform(hit.transform);
            }
            else if (IsPlatformed)
            {
                UnPlatform();
            }
        }
        
        private void OnWallSlide()
        {
            IsWallSliding = true;
            SkipGroundPlatformCheck = true;
            CurrentMovementControllerDatum = _playerMovementControllerDatum.WallSlideMovementControllerDatum;
            _wallSlideMovementComponent.SetWallSlideDirection(WallSlideDirection);
            _wallSlideMovementComponent.Activate();
        }
        
        private void WallSlideMovementComponentOnDeactivated()
        {
            IsWallSliding = false;
            SkipGroundPlatformCheck = false;
            UnPlatform();
        }
        
        private void MovementComponentOnDeactivated()
        {
            if (!IsWallSliding) CurrentMovementControllerDatum = _playerMovementControllerDatum; 
            //ActivateBoomerangEffect(false);
            //_animator.SetTrigger("Sphere");
            //if (debugResetMovementComponentData) SetMovementComponentData();
        }
    }
}
