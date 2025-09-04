using System;
using Interfaces;
using Managers;
using Scriptables.Movement;
using Systems.Actions.Movement;
using Systems.Player;
using UnityEngine;
using Action = Systems.Actions.Action;
using ActionContext = Systems.Actions.ActionContext;

namespace Systems.Movement
{
    public class PlayerMovementController : MovementController, IActivateActions
    {
        [Space] 
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        [Space] 
        [SerializeField] private bool disableWallSlideCheck;
        [SerializeField] private bool disableWallPlatformCheck;
        [Space] 
        [SerializeField] private float windStreaksVelocityThreshold = 10f;
        [SerializeField] private ParticleSystem windStreaksParticleSystem;
        [SerializeField] private GameObject smokePrefab;
        [SerializeField] private GameObject balloonJumpSmokePrefab;
        [SerializeField] private GameObject groundPoundSmokePrefab;

        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerController _playerController;
        private PlayerLook _playerLook;
        private Animator _animator;
        
        private bool _isCrouching;
        private bool _isRolling;

        private bool _canDoubleJump;
        private bool _canDash;

        private float _queueJumpTimer;
        private float _coyoteTimer;
        private float _wallCoyoteTimer;

        private bool _cutJump;

        private Vector3 _wallSlideDirection;
        
        private Action _jumpMovementAction;
        private Action _doubleJumpMovementAction;
        private Action _wallJumpMovementAction;
        
        private Action _dashMovementAction;
        private Action _diveMovementAction;
        private Action _airMovementAction;
        private Action _rollMovementAction;
        private Action _leftSpecialMovementAction;
        private Action _rightSpecialMovementAction;
        
        private Action _wallSlideMovementAction;
        
        public bool IsWallSliding { get; private set; }

        private Vector3 GetForward()
        {
            var forward = Mover?.GetInput() ?? Vector3.zero;
            forward = forward.magnitude < 0.5f ? root.forward : _playerLook.TransformInput(forward);
            forward.y = 0;
            
            if (MovementDimensions != Dimensions.Two) return forward;
            forward.z = 0;
            if (forward.x == 0) forward.x = 1;
            return forward;
        }

        protected override void OnAwake()
        {
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
            
            _playerController = GetComponent<PlayerController>();
            _playerLook = GetComponent<PlayerLook>();
            _animator = GetComponent<Animator>();
            
            Grounded += OnGrounded;
        }

        protected override void OnInitialized(IMove move)
        {
            _jumpMovementAction = _playerController.GetMovementActionDatum(MovementActionType.JumpAction).AttachAction(gameObject);
            _doubleJumpMovementAction = _playerController.GetMovementActionDatum(MovementActionType.DoubleJumpAction).AttachAction(gameObject);
            _wallJumpMovementAction = _playerController.GetMovementActionDatum(MovementActionType.WallJumpAction).AttachAction(gameObject);
            
            _dashMovementAction = _playerController.GetMovementActionDatum(MovementActionType.DashAction).AttachAction(gameObject);
            _diveMovementAction = _playerController.GetMovementActionDatum(MovementActionType.DiveAction).AttachAction(gameObject);
            _airMovementAction = _playerController.GetMovementActionDatum(MovementActionType.AirAction).AttachAction(gameObject);
            
            _wallSlideMovementAction = _playerController.GetMovementActionDatum(MovementActionType.WallSlideAction).AttachAction(gameObject);
        }

        private void FixedUpdate()
        {
            _animator.SetBool("IsGrounded", IsGrounded);

            var deltaTime = Time.fixedDeltaTime;
            var velocity = ForceController.GetVelocity();
            root.localScale = Vector3.Lerp(root.localScale,
                new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), deltaTime * scaleSpeed);
            _animator.SetFloat("yVelocity", velocity.y);
            
            if (velocity.magnitude > 0.1f) windStreaksParticleSystem.transform.forward = -velocity.normalized;
            if (velocity.magnitude > windStreaksVelocityThreshold && !windStreaksParticleSystem.isPlaying) windStreaksParticleSystem.Play();
            else if (velocity.magnitude < windStreaksVelocityThreshold && windStreaksParticleSystem.isPlaying) windStreaksParticleSystem.Stop();
            
            velocity.y = 0;
            _animator.SetFloat("xzVelocity", velocity.magnitude / CurrentMovementControllerDatum.MaxSpeed);
            if (velocity.magnitude > 0.1f) root.forward = velocity;

            if (_cutJump && (_jumpMovementAction.IsActive || _doubleJumpMovementAction.IsActive ||
                             _wallJumpMovementAction.IsActive))
            {
                //Debug.Log("AutoCutJump");
                StopJumping();
            }

            if (_queueJumpTimer > 0)
            {
                if ((IsGrounded || IsWallSliding) && !_diveMovementAction.IsActive)
                {
                    _queueJumpTimer = 0;
                    StartJumping(false);
                }

                _queueJumpTimer -= deltaTime;
            }

            if (_coyoteTimer > 0)
            {
                _coyoteTimer -= deltaTime;
            }

            if (_wallCoyoteTimer > 0)
            {
                _wallCoyoteTimer -= deltaTime;
            }

            if (!_canDash) _canDash = IsGrounded;
            if (!_canDoubleJump) _canDoubleJump = IsGrounded;
        }

        protected override void OnUpdate()
        {
            WallSlide();
            if (!IsWallSliding || disableWallPlatformCheck) return;
            CheckWallPlatform();
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (IsWallSliding)
            {
                var newInput = ForceController.GetCancelledVector(input, -_wallSlideDirection);
                base.OnMove(newInput, datum);

                if (input.magnitude < 0.5f) return;
                if (Vector3.Angle(_wallSlideDirection, input) <
                    _playerMovementControllerDatum.WallSlideMinExitAngle) return;
                StopWallSliding();
                _wallCoyoteTimer = _playerMovementControllerDatum.WallJumpCoyoteTime;
            }

            base.OnMove(input, datum);
        }

        private void OnGrounded(bool isGrounded)
        {
            switch (isGrounded)
            {
                case false when !_jumpMovementAction.IsActive || !_doubleJumpMovementAction.IsActive || !_wallJumpMovementAction.IsActive:
                    _coyoteTimer = _playerMovementControllerDatum.CoyoteTime;
                    break;
                case true when _diveMovementAction.IsActive:
                    _diveMovementAction.Deactivate(GetActionContext());
                    //Instantiate(groundPoundSmokePrefab, root.position, Quaternion.identity);
                    break;
            }
        }
        
        private void QueueJump() => _queueJumpTimer = _playerMovementControllerDatum.QueueJumpTime;

        public void StartJumping(bool resetCutJump = true)
        {
            if (resetCutJump) _cutJump = false;
            if (_jumpMovementAction.IsActive || _doubleJumpMovementAction.IsActive || _wallJumpMovementAction.IsActive || _dashMovementAction.IsActive || _diveMovementAction.IsActive)
            {
                QueueJump();
                return;
            }

            CancelJumping();
            CancelDiving();
            CancelAir();
            
            if (IsGrounded || _coyoteTimer > 0)
            {
                if (_isCrouching)
                {
                    StartRolling();
                }
                else
                {
                    _jumpMovementAction.Activate(GetActionContext());
                    //Instantiate(smokePrefab, root.position, Quaternion.identity);   
                }
            }
            else
            {
                if (IsWallSliding || _wallCoyoteTimer > 0)
                {
                    _wallJumpMovementAction.Activate(GetActionContext());
                    //Instantiate(smokePrefab, root.position, Quaternion.identity);
                }
                else if (_canDoubleJump)
                {
                    StopRolling();
                    _doubleJumpMovementAction.Activate(GetActionContext());
                    //Instantiate(balloonJumpSmokePrefab, root.position, Quaternion.identity);
                    _canDoubleJump = false;
                }
                else
                {
                    QueueJump();
                }
            }
            
            StopWallSliding();
        }

        public void StartDashing()
        {
            if (!_canDash) return;
            
            CancelJumping();
            CancelDiving();
            CancelAir();
            StopRolling();

            _canDash = false;
            _dashMovementAction.Activate(GetActionContext());
        }

        public void StartCrouching()
        {
            _isCrouching = true;
            if (_isRolling) StopRolling();
            
            if (IsGrounded || _diveMovementAction.IsActive || _dashMovementAction.IsActive) return;
            
            CancelJumping();
            StartDiving();
        }

        private void StartDiving()
        {
            _isCrouching = false;
            _diveMovementAction.Activate(GetActionContext());
        }

        public void StartAir()
        {
            _airMovementAction.Activate(GetActionContext());
        }

        private void StartRolling()
        {
            if (_isRolling) return;
            _isRolling = true;
            CurrentMovementControllerDatum = _playerMovementControllerDatum.RollMovementControllerDatum;
            _animator.SetBool("IsRolling", true);
            ForceController.ApplyForce(root.rotation * Vector3.forward * _playerMovementControllerDatum.InitialRollSpeed, ForceMode.VelocityChange);
        }

        public void StartLeftSpecial()
        {
            _leftSpecialMovementAction.Activate(GetActionContext());
        }

        public void StartRightSpecial()
        {
            _rightSpecialMovementAction.Activate(GetActionContext());
        }

        private void StartWallSliding()
        {
            _wallSlideMovementAction.Activate(GetActionContext());
            IsWallSliding = true;
            SkipGroundPlatformCheck = true;
            CurrentMovementControllerDatum = _playerMovementControllerDatum.WallSlideMovementControllerDatum;
        }

        public void StopJumping()
        {
            _cutJump = true;
            _jumpMovementAction.Deactivate(GetActionContext());
            _doubleJumpMovementAction.Deactivate(GetActionContext());
            _wallJumpMovementAction.Deactivate(GetActionContext());
        }

        public void StopDashing()
        {
            _dashMovementAction.Deactivate(GetActionContext());
        }

        public void StopCrouching()
        {
            _isCrouching = false;
        }

        private void StopDiving()
        {
            _diveMovementAction.Deactivate(GetActionContext());
        }
        
        public void StopAir()
        {
            _airMovementAction.Deactivate(GetActionContext());
        }

        private void StopRolling()
        {
            _isRolling = false;
            CurrentMovementControllerDatum = _playerMovementControllerDatum;
            _animator.SetBool("IsRolling", false);
        }

        public void StopLeftSpecial()
        {
         _leftSpecialMovementAction.Deactivate(GetActionContext());   
        }

        public void StopRightSpecial()
        {
            _rightSpecialMovementAction.Deactivate(GetActionContext());
        }

        private void StopWallSliding()
        {
            if (!IsWallSliding) return;
            
            _wallSlideMovementAction.Deactivate(GetActionContext());
            IsWallSliding = false;
            SkipGroundPlatformCheck = false;
            CurrentMovementControllerDatum = _playerMovementControllerDatum;
            UnPlatform();
        }

        private void CancelJumping()
        {
            _jumpMovementAction.Cancel(GetActionContext());
            _doubleJumpMovementAction.Cancel(GetActionContext());
            _wallJumpMovementAction.Cancel(GetActionContext());
        }

        private void CancelDashing() => _dashMovementAction.Cancel(GetActionContext());
        private void CancelDiving() => _diveMovementAction.Cancel(GetActionContext());
        private void CancelAir() => _airMovementAction.Cancel(GetActionContext());
        private void CancelRolling() => _rollMovementAction.Cancel(GetActionContext());
        private void CancelLeftSpecial() => _leftSpecialMovementAction.Cancel(GetActionContext());
        private void CancelRightSpecial() => _rightSpecialMovementAction.Cancel(GetActionContext());
        private void CancelWallSliding() => _wallSlideMovementAction.Cancel(GetActionContext());
        

        private void WallSlide()
        {
            if (_isRolling || _jumpMovementAction.IsActive || 
                _doubleJumpMovementAction.IsActive || _wallJumpMovementAction.IsActive 
                || _dashMovementAction.IsActive || _diveMovementAction.IsActive) return;

            var canWallSlide = CheckWallSlide();
            
            switch (canWallSlide)
            {
                case true when !_wallSlideMovementAction.IsActive:
                    StartWallSliding();
                    break;
                case false when _wallSlideMovementAction.IsActive:
                    StopWallSliding();
                    break;
            }
        }

        private bool CheckWallSlide()
        {
            if (disableWallSlideCheck || IsGrounded) return false;
            if (_wallSlideMovementAction.IsActive) return CheckWallSlideDirection(_wallSlideDirection);
            
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
                if (!CheckWallSlideDirection(direction)) continue;
                _wallSlideDirection = direction;
                
                return true;
            }
            return false;
        }

        private bool CheckWallSlideDirection(Vector3 direction)
        {
            return Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset,
                direction, _playerMovementControllerDatum.WallSlideCheckDistance,
                _playerMovementControllerDatum.WallSlideCheckLayerMask, QueryTriggerInteraction.Ignore);
        }

        private void CheckWallPlatform()
        {
            if (Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset,
                    _wallSlideDirection, out var hit, _playerMovementControllerDatum.WallSlideCheckDistance,
                    platformCheckLayerMask, QueryTriggerInteraction.Ignore))
            {
                Platform(hit.transform);
            }
            else if (IsPlatformed)
            {
                UnPlatform();
            }
        }

        public ActionContext GetActionContext()
        {
            Vector3 targetDirection;
            if (IsWallSliding || _wallCoyoteTimer > 0) targetDirection = -_wallSlideDirection;
            else targetDirection = GetForward();
            
            return ActionContext.Construct(this, _playerController, gameObject, _playerMovementControllerDatum, targetDirection);
        }
    }
}