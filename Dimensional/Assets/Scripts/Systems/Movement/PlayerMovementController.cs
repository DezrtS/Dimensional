using System;
using FMODUnity;
using Interfaces;
using Managers;
using Scriptables.Actions;
using Scriptables.Movement;
using Systems.Actions.Movement;
using Systems.Player;
using Systems.Visual_Effects;
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
        [SerializeField] private ActionDatum defaultActionDatum;
        [Space] 
        [SerializeField] private float dizzyDuration;
        [Space] 
        [SerializeField] private float landEffectSpeedThreshold;
        [SerializeField] private EventReference landEffectSound;
        [SerializeField] private VisualEffectPlayer landEffect;
        [SerializeField] private float windStreaksVelocityThreshold = 10f;
        [SerializeField] private ParticleSystem windStreaksParticleSystem;

        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerController _playerController;
        private PlayerLook _playerLook;
        private Animator _animator;
        
        private bool _isCrouching;

        private bool _canDoubleJump;
        private bool _canDash;

        private float _queueJumpTimer;
        private float _coyoteTimer;
        private float _wallCoyoteTimer;
        private float _wallSlideDelayTimer;
        private float _dizzyTimer;

        private bool _cutJump;

        private float _previousYVelocity;

        private Vector3 _wallSlideDirection;
        
        private Action _jumpMovementAction;
        private Action _doubleJumpMovementAction;
        private Action _wallJumpMovementAction;
        private Action _rollJumpMovementAction;
        
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
            forward = forward.sqrMagnitude < 0.25f ? _playerLook.TransformInput(Vector3.forward) : _playerLook.TransformInput(forward);
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
            SetMovementActions();
        }

        private void SetMovementActions()
        {
            _jumpMovementAction = SetMovementAction(MovementActionType.JumpAction);
            _doubleJumpMovementAction = SetMovementAction(MovementActionType.DoubleJumpAction);
            _wallJumpMovementAction = SetMovementAction(MovementActionType.WallJumpAction);
            _rollJumpMovementAction = SetMovementAction(MovementActionType.RollJumpAction);

            _jumpMovementAction.Triggered += ActionOnTriggered;
            _doubleJumpMovementAction.Triggered += ActionOnTriggered;
            _wallJumpMovementAction.Triggered += ActionOnTriggered;
            _rollJumpMovementAction.Triggered += ActionOnTriggered;
            
            _dashMovementAction = SetMovementAction(MovementActionType.DashAction);
            _diveMovementAction = SetMovementAction(MovementActionType.DiveAction);
            _airMovementAction = SetMovementAction(MovementActionType.AirAction);
            
            _rollMovementAction = SetMovementAction(MovementActionType.RollAction);
            
            _leftSpecialMovementAction = SetMovementAction(MovementActionType.LeftSpecialAction);
            _rightSpecialMovementAction = SetMovementAction(MovementActionType.RightSpecialAction);
            
            _wallSlideMovementAction = SetMovementAction(MovementActionType.WallSlideAction);
        }

        private Action SetMovementAction(MovementActionType movementActionType)
        {
            var movementActionDatum = _playerController.GetMovementActionDatum(movementActionType);
            return !movementActionDatum ? defaultActionDatum.AttachAction(gameObject) : movementActionDatum.AttachAction(gameObject);
        }

        [ContextMenu("Reset Movement Actions")]
        public void ResetMovementActions()
        {
            _jumpMovementAction.Triggered -= ActionOnTriggered;
            _doubleJumpMovementAction.Triggered -= ActionOnTriggered;
            _wallJumpMovementAction.Triggered -= ActionOnTriggered;
            _rollJumpMovementAction.Triggered -= ActionOnTriggered;
            
            VerifyMovementAction(ref _jumpMovementAction, MovementActionType.JumpAction);
            VerifyMovementAction(ref _doubleJumpMovementAction, MovementActionType.DoubleJumpAction);
            VerifyMovementAction(ref _wallJumpMovementAction, MovementActionType.WallJumpAction);
            VerifyMovementAction(ref _rollJumpMovementAction, MovementActionType.RollJumpAction);
            VerifyMovementAction(ref _dashMovementAction, MovementActionType.DashAction);
            VerifyMovementAction(ref _diveMovementAction, MovementActionType.DiveAction);
            VerifyMovementAction(ref _airMovementAction, MovementActionType.AirAction);
            VerifyMovementAction(ref _rollMovementAction, MovementActionType.RollAction);
            VerifyMovementAction(ref _leftSpecialMovementAction, MovementActionType.LeftSpecialAction);
            VerifyMovementAction(ref _rightSpecialMovementAction, MovementActionType.RightSpecialAction);
            VerifyMovementAction(ref _wallSlideMovementAction, MovementActionType.WallSlideAction);
            
            _jumpMovementAction.Triggered += ActionOnTriggered;
            _doubleJumpMovementAction.Triggered += ActionOnTriggered;
            _wallJumpMovementAction.Triggered += ActionOnTriggered;
            _rollJumpMovementAction.Triggered += ActionOnTriggered;
        }

        private void VerifyMovementAction(ref Action action, MovementActionType movementActionType)
        {
            ActionDatum movementActionDatum = _playerController.GetMovementActionDatum(movementActionType);
            
            if (!movementActionDatum) movementActionDatum = defaultActionDatum;
            if (action.ActionDatum == movementActionDatum) return;
            
            action.Cancel(GetActionContext());
            action.Destroy();
            action = movementActionDatum.AttachAction(gameObject);
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
            if (velocity.magnitude > 1 && !IsDisabled) root.forward = velocity;

            if (_cutJump && (_jumpMovementAction.IsActive || _doubleJumpMovementAction.IsActive || _wallJumpMovementAction.IsActive || _rollJumpMovementAction.IsActive))
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
                if (_coyoteTimer <= 0) CancelActiveJumping();
            }

            if (_wallCoyoteTimer > 0)
            {
                _wallCoyoteTimer -= deltaTime;
                if (_wallCoyoteTimer <= 0) CancelActiveJumping();
            }

            if (_wallSlideDelayTimer > 0)
            {
                _wallSlideDelayTimer -= deltaTime;
            }

            if (_dizzyTimer > 0)
            {
                _dizzyTimer -= deltaTime;
                if (_dizzyTimer <= 0) _playerController.SetDizzyEyes(false);
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

        private void LateUpdate()
        {
            _previousYVelocity = ForceController.GetVelocity().y;
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (IsWallSliding)
            {
                var newInput = ForceController.GetCancelledVector(input, -_wallSlideDirection);
                base.OnMove(newInput, datum);

                if (input.magnitude < 0.5f) return;
                var angle = Vector3.Angle(_wallSlideDirection, input);
                if (angle < _playerMovementControllerDatum.WallSlideMinExitAngle) return;
                StopWallSliding();
                _wallCoyoteTimer = _playerMovementControllerDatum.WallJumpCoyoteTime;
                _wallSlideDelayTimer = _playerMovementControllerDatum.WallSlideDelayTime;
            }

            base.OnMove(input, datum);
        }

        private void OnGrounded(bool isGrounded)
        {
            if (isGrounded)
            {
                if (_diveMovementAction.IsActive || -_previousYVelocity < landEffectSpeedThreshold) return;
                landEffect.Play(false);
                AudioManager.PlayOneShot(landEffectSound, transform.position);
            }
            else
            {
                if (_jumpMovementAction.IsActive || _doubleJumpMovementAction.IsActive ||
                    _wallJumpMovementAction.IsActive || _rollJumpMovementAction.IsActive) return;
                _coyoteTimer = _playerMovementControllerDatum.CoyoteTime;
            }
            
            //if (!isGrounded)
            //{
            //    CancelActiveJumping();
            //}
            
            //switch (isGrounded)
            //{
                //case false when !_jumpMovementAction.IsActive || !_doubleJumpMovementAction.IsActive || !_wallJumpMovementAction.IsActive:
                    //_coyoteTimer = _playerMovementControllerDatum.CoyoteTime;
                    //break;
                //case true when _diveMovementAction.IsActive:
                //    _diveMovementAction.Deactivate(GetActionContext());
                //    break;
            //}
        }
        
        private void QueueJump() => _queueJumpTimer = _playerMovementControllerDatum.QueueJumpTime;

        public void StartJumping(bool resetCutJump = true)
        {
            if (resetCutJump) _cutJump = false;
            if (_wallJumpMovementAction.IsActive || _dashMovementAction.IsActive || _diveMovementAction.IsActive)
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
                else if (_rollMovementAction.IsActive)
                {
                    _rollJumpMovementAction.Activate(GetActionContext());
                }
                else
                {
                    _jumpMovementAction.Activate(GetActionContext());
                }
            }
            else
            {
                if (IsWallSliding || _wallCoyoteTimer > 0)
                {
                    _wallJumpMovementAction.Activate(GetActionContext());
                }
                else if (_canDoubleJump)
                {
                    StopRolling();
                    _doubleJumpMovementAction.Activate(GetActionContext());
                    _canDoubleJump = false;
                }
                else
                {
                    QueueJump();
                }
            }
        }

        private void ActionOnTriggered(Action action, ActionContext context)
        {
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
            if (_rollMovementAction.IsActive)
            {
                StopRolling();
                _isCrouching = false;
            }
            
            if (IsGrounded || _diveMovementAction.IsActive || _dashMovementAction.IsActive) return;
            
            CancelJumping();
            StartDiving();
        }

        private void StartDiving()
        {
            CancelAir();
            StopRolling();
            StopWallSliding();
            
            _isCrouching = false;
            _diveMovementAction.Activate(GetActionContext());
        }

        public void StartAir()
        {
            if (IsGrounded || _wallSlideMovementAction.IsActive) return;
            
            CancelJumping();
            CancelDashing();
            CancelDiving();
            StopRolling();
            
            _airMovementAction.Activate(GetActionContext());
        }

        private void StartRolling()
        {
            if (_rollMovementAction.IsActive) return;
            var actionContext = GetActionContext();
            _isCrouching = false;
            _rollMovementAction.Activate(actionContext);
            
            _dizzyTimer = 0;
            _playerController.SetDizzyEyes(true);
            _animator.SetBool("IsRolling", true);
            ForceController.ApplyForce(Quaternion.LookRotation(actionContext.TargetDirection) * Vector3.forward * _playerMovementControllerDatum.InitialRollSpeed, ForceMode.VelocityChange);
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
            CancelAir();
            
            _wallSlideMovementAction.Activate(GetActionContext());
            IsWallSliding = true;
            SkipGroundPlatformCheck = true;
        }

        public void StopJumping()
        {
            _cutJump = true;
            var actionContext = GetActionContext();
            _jumpMovementAction.Deactivate(actionContext);
            _doubleJumpMovementAction.Deactivate(actionContext);
            _wallJumpMovementAction.Deactivate(actionContext);
            _rollJumpMovementAction.Deactivate(actionContext);
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
            if (!_rollMovementAction.IsActive) return;
            _rollMovementAction.Deactivate(GetActionContext());
            _dizzyTimer = dizzyDuration;
            
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
            //CancelActiveJumping();
            
            _wallSlideMovementAction.Deactivate(GetActionContext());
            IsWallSliding = false;
            SkipGroundPlatformCheck = false;
            UnPlatform();
        }

        private void CancelJumping()
        {
            var actionContext = GetActionContext();
            _jumpMovementAction.Cancel(actionContext);
            _doubleJumpMovementAction.Cancel(actionContext);
            _wallJumpMovementAction.Cancel(actionContext);
            _rollJumpMovementAction.Cancel(actionContext);
        }

        private void CancelActiveJumping()
        {
            var actionContext = GetActionContext();
            if (_jumpMovementAction.IsActive && !_jumpMovementAction.IsTriggering) _jumpMovementAction.Cancel(actionContext);
            if (_doubleJumpMovementAction.IsActive && !_doubleJumpMovementAction.IsTriggering) _doubleJumpMovementAction.Cancel(actionContext);
            if (_wallJumpMovementAction.IsActive && !_wallJumpMovementAction.IsTriggering) _wallJumpMovementAction.Cancel(actionContext);
            if (_rollJumpMovementAction.IsActive && !_rollJumpMovementAction.IsTriggering) _rollJumpMovementAction.Cancel(actionContext);
        }

        private void CancelDashing() => _dashMovementAction.Cancel(GetActionContext());
        private void CancelDiving() => _diveMovementAction.Cancel(GetActionContext());
        private void CancelAir() => _airMovementAction.Cancel(GetActionContext());

        private void CancelRolling()
        {
            _rollMovementAction.Cancel(GetActionContext());
            _animator.SetBool("IsRolling", false);
        }
        private void CancelLeftSpecial() => _leftSpecialMovementAction.Cancel(GetActionContext());
        private void CancelRightSpecial() => _rightSpecialMovementAction.Cancel(GetActionContext());
        private void CancelWallSliding() => _wallSlideMovementAction.Cancel(GetActionContext());

        public void CancelAllActions()
        {
            var actionContext = GetActionContext();
            _jumpMovementAction.Cancel(actionContext);
            _doubleJumpMovementAction.Cancel(actionContext);
            _wallJumpMovementAction.Cancel(actionContext);
            _rollJumpMovementAction.Cancel(actionContext);
            _dashMovementAction.Cancel(actionContext);
            _diveMovementAction.Cancel(actionContext);
            _airMovementAction.Cancel(actionContext);
            _rollMovementAction.Cancel(actionContext);
            _animator.SetBool("IsRolling", false);
            _leftSpecialMovementAction.Cancel(actionContext);
            _rightSpecialMovementAction.Cancel(actionContext);
            _wallSlideMovementAction.Cancel(actionContext);
        }
        

        private void WallSlide()
        {
            // Potential Problem with _rollJumpMovementAction
            if (_wallSlideDelayTimer > 0 || _rollMovementAction.IsActive || _jumpMovementAction.IsActive || 
                _doubleJumpMovementAction.IsActive || _wallJumpMovementAction.IsTriggering || 
                _rollJumpMovementAction.IsActive || _dashMovementAction.IsActive || 
                _diveMovementAction.IsActive) return;

            var canWallSlide = CheckWallSlide();
            
            switch (canWallSlide)
            {
                case true when !_wallSlideMovementAction.IsActive:
                    StartWallSliding();
                    break;
                case false when _wallSlideMovementAction.IsActive:
                    if (!IsGrounded) _wallCoyoteTimer = _playerMovementControllerDatum.WallJumpCoyoteTime;
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
            if (input.magnitude < _playerMovementControllerDatum.WallSlideMinEnterMagnitude) return false;
            
            input.y = 0;
            if (MovementDimensions == Dimensions.Two) input.z = 0;
            
            input = input.normalized;
            var intervalAngle = (int)(360 / _playerMovementControllerDatum.WallSlideCheckIntervals);
            for (var i = 0; i < _playerMovementControllerDatum.WallSlideCheckIntervals; i++)
            {
                var direction = Quaternion.Euler(0, i * intervalAngle, 0) * Vector3.forward;
                if (Vector3.Dot(input, direction) < _playerMovementControllerDatum.WallSlideMinDirectionDot) continue;
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