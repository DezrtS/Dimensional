using System;
using Interfaces;
using Managers;
using Scriptables.Movement;
using Systems.Interactables;
using Systems.Movement.Components;
using Systems.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Systems.Movement
{
    public class PlayerMovementController : MovementController
    {
        [Space] 
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        [Space] 
        [SerializeField] private bool debugResetMovementComponentData;
        [SerializeField] private bool disableDoubleJump;
        [SerializeField] private bool disableSpringJump;
        [SerializeField] private bool disableBoomerang;
        [SerializeField] private bool disableGlide;
        [SerializeField] private bool disableGrapple;
        [SerializeField] private bool disableGroundPound;
        [SerializeField] private bool disableAttack;
        [Space] 
        [SerializeField] private bool disableWallSlideCheck;
        [SerializeField] private bool disableWallPlatformCheck;
        [Space] 
        [SerializeField] private VisualEffect[] visualEffects;
        [Space] 
        [SerializeField] private float windStreaksVelocityThreshold = 10f;
        [SerializeField] private ParticleSystem windStreaksParticleSystem;
        [SerializeField] private GameObject smokePrefab;
        [SerializeField] private GameObject balloonJumpSmokePrefab;
        [SerializeField] private GameObject groundPoundSmokePrefab;

        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerLook _playerLook;
        private Animator _animator;
        
        private bool _isCrouching;
        private bool _isRolling;
        private bool _isWallSliding;
        private bool _isWallDashing;
        private bool _isAttacking;

        private bool _canDoubleJump;
        private bool _canSpringJump;
        private bool _canBoomerang;
        private bool _canAttack;

        private Vector3 _wallSlideDirection;

        private float _springJumpTimer;
        private float _queueJumpTimer;
        private float _coyoteTimer;
        private float _wallJumpCoyoteTimer;
        
        private JumpMovementComponent _jumpMovementComponent;
        private BoomerangMovementComponent _boomerangMovementComponent;
        private GlideMovementComponent _glideMovementComponent;
        private GrappleMovementComponent _grappleMovementComponent;
        private GroundPoundMovementComponent _groundPoundMovementComponent;
        private WallSlideMovementComponent _wallSlideMovementComponent;
        private WallDashMovementComponent _wallDashMovementComponent;
        private DashMovementComponent _dashMovementComponent;

        private BoomerangTarget _boomerangTarget;

        protected override void OnAwake()
        {
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
            
            _playerLook = GetComponent<PlayerLook>();
            _animator = GetComponent<Animator>();
            Grounded += OnGrounded;
            
            _jumpMovementComponent = GetComponent<JumpMovementComponent>();
            _boomerangMovementComponent = GetComponent<BoomerangMovementComponent>();
            _glideMovementComponent = GetComponent<GlideMovementComponent>();
            _grappleMovementComponent = GetComponent<GrappleMovementComponent>();
            _groundPoundMovementComponent = GetComponent<GroundPoundMovementComponent>();
            _wallSlideMovementComponent = GetComponent<WallSlideMovementComponent>();
            _wallDashMovementComponent = GetComponent<WallDashMovementComponent>();
            _dashMovementComponent = GetComponent<DashMovementComponent>();
            
            _jumpMovementComponent.Initialize(this);
            _boomerangMovementComponent.Initialize(this);
            _glideMovementComponent.Initialize(this);
            _grappleMovementComponent.Initialize(this);
            _groundPoundMovementComponent.Initialize(this);
            _wallSlideMovementComponent.Initialize(this);
            _wallDashMovementComponent.Initialize(this);
            _dashMovementComponent.Initialize(this);
            
            SetMovementComponentData();

            _groundPoundMovementComponent.GroundPounded += GroundPoundMovementComponentOnGroundPounded;
            _wallSlideMovementComponent.Deactivated += WallSlideMovementComponentOnDeactivated;
            _wallDashMovementComponent.Deactivated += WallDashMovementComponentOnDeactivated;
            _dashMovementComponent.Deactivated += DashMovementComponentOnDeactivated;

            _jumpMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _boomerangMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _glideMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _grappleMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _groundPoundMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _wallSlideMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _wallDashMovementComponent.Deactivated += MovementComponentOnDeactivated;
            _dashMovementComponent.Deactivated += MovementComponentOnDeactivated;
        }

        private void SetMovementComponentData()
        {
            //_boomerangMovementComponent.SetBoomerangMovementData(_playerMovementControllerDatum.BoomerangFallSpeedThreshold, _playerMovementControllerDatum.BoomerangFallTimeThreshold, _playerMovementControllerDatum.BoomerangTime, _playerMovementControllerDatum.BoomerangCurve);
            _glideMovementComponent.SetGlideMovementData(_playerMovementControllerDatum.GlideFallSpeedThreshold, _playerMovementControllerDatum.GlideFallTimeThreshold, _playerMovementControllerDatum.GlideFallSpeed);
            _grappleMovementComponent.SetGrappleMovementData(_playerMovementControllerDatum.GrappleSpeed, _playerMovementControllerDatum.GrappleTime, _playerMovementControllerDatum.GrappleCurve);
            _groundPoundMovementComponent.SetGroundPoundMovementData(_playerMovementControllerDatum.GroundPoundSpeed, _playerMovementControllerDatum.GroundPoundTime, _playerMovementControllerDatum.GroundPoundCurve, _playerMovementControllerDatum.GroundPoundMediumPowerTimeThreshold, _playerMovementControllerDatum.GroundPoundHighPowerTimeThreshold);
            _wallSlideMovementComponent.SetWallSlideMovementData(_playerMovementControllerDatum.WallSlideSpeed, _playerMovementControllerDatum.WallSlideTime, _playerMovementControllerDatum.WallSlideCurve, _playerMovementControllerDatum.WallSlideCheckOffset, _playerMovementControllerDatum.WallSlideCheckDistance, _playerMovementControllerDatum.WallSlideCheckLayerMask);
            _wallDashMovementComponent.SetWallDashMovementData(_playerMovementControllerDatum.WallDashSpeed, _playerMovementControllerDatum.WallDashTime, _playerMovementControllerDatum.WallDashCurve);
            _dashMovementComponent.SetDashMovementData(_playerMovementControllerDatum.AttackVector, _playerMovementControllerDatum.AttackTime, _playerMovementControllerDatum.AttackCurve);
        }

        private void FixedUpdate()
        {
            UpdateGrappleTarget();
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

            if (_springJumpTimer > 0)
            {
                _springJumpTimer -= deltaTime;
                if (_springJumpTimer <= 0)
                {
                    _canSpringJump = false;
                }
            }

            if (_queueJumpTimer > 0)
            {
                if ((IsGrounded || _isWallSliding) && !_groundPoundMovementComponent.IsActive)
                {
                    _queueJumpTimer = 0;
                    Jump(false);
                }

                _queueJumpTimer -= deltaTime;
            }

            if (_coyoteTimer > 0)
            {
                _coyoteTimer -= deltaTime;
            }

            if (_wallJumpCoyoteTimer > 0)
            {
                _wallJumpCoyoteTimer -= deltaTime;
            }

            if (!_canAttack) _canAttack = IsGrounded;
            if (!_canDoubleJump) _canDoubleJump = IsGrounded;
            if (!_canBoomerang) _canBoomerang = IsGrounded;
        }

        private void UpdateGrappleTarget()
        {
            var results = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, _playerMovementControllerDatum.GrappleTargetCheckRadius, results, _playerMovementControllerDatum.GrappleTargetCheckLayerMask,
                QueryTriggerInteraction.Collide);

            BoomerangTarget closestTarget = null;
            var minAngle = float.MaxValue;

            var playerForward = _playerLook.TransformInput(Vector3.forward); // Cache player's forward direction

            for (var i = 0; i < size; i++)
            {
                var boomerangTarget = results[i].GetComponent<BoomerangTarget>();
                if (!boomerangTarget) continue;

                // Check if target is interactable
                if (!boomerangTarget.CanInteract(InteractContext.Construct(gameObject))) continue;

                // Calculate direction to target and angular offset
                var directionToTarget = (boomerangTarget.transform.position - transform.position).normalized;
                var angle = Vector3.Angle(playerForward, directionToTarget);

                // Check if within maximum allowed angle and has smallest angle found
                if (!(angle <= _playerMovementControllerDatum.GrappleTargetCheckMaxAngleDifference) || !(angle < minAngle)) continue;
                minAngle = angle;
                closestTarget = boomerangTarget;
            }

            _boomerangTarget = closestTarget; // Store the closest valid target
        }

        protected override void OnUpdate()
        {
            WallSlide();
            if (!_isWallSliding || disableWallPlatformCheck) return;
            CheckWallPlatform();
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (_isWallSliding)
            {
                var newInput = ForceController.GetCancelledVector(input, -_wallSlideDirection);
                base.OnMove(newInput, datum);

                if (input.magnitude < 0.5f) return;
                if (Vector3.Angle(_wallSlideDirection, input) <
                    _playerMovementControllerDatum.WallSlideMinExitAngle) return;
                StopWallSliding();
                _wallJumpCoyoteTimer = _playerMovementControllerDatum.WallJumpCoyoteTime;
            }

            base.OnMove(input, datum);
        }

        private void OnGrounded(bool isGrounded)
        {
            if (!isGrounded && !_jumpMovementComponent.IsActive)
            {
                _coyoteTimer = _playerMovementControllerDatum.CoyoteTime;
            }
        }

        private void MovementComponentOnDeactivated()
        {
            if (!_isRolling && !_isWallSliding && !_glideMovementComponent.IsActive) CurrentMovementControllerDatum = _playerMovementControllerDatum; 
            ActivateBoomerangEffect(false);
            _animator.SetTrigger("Sphere");
            if (debugResetMovementComponentData) SetMovementComponentData();
        }

        private void ActivateBoomerangEffect(bool enable)
        {
            if (enable)
            {
                foreach (var visualEffect in visualEffects)
                {
                    visualEffect.SendEvent("OnPlay");
                }
            }
            else
            {
                foreach (var visualEffect in visualEffects)
                {
                    visualEffect.Stop();
                }
            }
        }

        public void Jump(bool resetCutJump = true)
        {
            if (resetCutJump) _jumpMovementComponent.SetCutJump(false);

            if (_dashMovementComponent.IsActive || _groundPoundMovementComponent.IsActive)
            {
                QueueJump();
                return;
            }
            
            if (!IsGrounded && _coyoteTimer <= 0)
            {
                if (_wallSlideMovementComponent.IsActive || _wallJumpCoyoteTimer > 0)
                {
                    OnJump(_playerMovementControllerDatum.WallJumpTime,
                        _playerMovementControllerDatum.WallJumpHeight,
                        _playerMovementControllerDatum.WallJumpCurve);
                    OnWallDash(); // May need to separate from CancelJump();
                    Instantiate(smokePrefab, root.position, Quaternion.identity);
                }
                else if (_canDoubleJump && !disableDoubleJump)
                {
                    StopRolling();
                    _canDoubleJump = false;
                    OnJump(_playerMovementControllerDatum.DoubleJumpTime,
                        _playerMovementControllerDatum.DoubleJumpHeight,
                        _playerMovementControllerDatum.DoubleJumpCurve);
                    Instantiate(balloonJumpSmokePrefab, root.position, Quaternion.identity);
                    _animator.SetTrigger("Balloon");
                }
                else
                {
                    QueueJump();
                }
            }
            else
            {
                if (_isCrouching)
                {
                    Roll();
                }
                else
                {
                    if (_canSpringJump && !disableSpringJump)
                    {
                        var timeMultiplier = 1f;
                        var heightMultiplier = 1f;

                        switch (_groundPoundMovementComponent.GroundPoundPowerLevel)
                        {
                            case PowerLevel.Low:
                                break;
                            case PowerLevel.Medium:
                                timeMultiplier = _playerMovementControllerDatum.SpringJumpMediumPowerTimeMultiplier;
                                heightMultiplier = _playerMovementControllerDatum.SpringJumpMediumPowerHeightMultiplier;
                                break;
                            case PowerLevel.High:
                                timeMultiplier = _playerMovementControllerDatum.SpringJumpHighPowerTimeMultiplier;
                                heightMultiplier = _playerMovementControllerDatum.SpringJumpHighPowerHeightMultiplier;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        OnJump(_playerMovementControllerDatum.SpringJumpTime * timeMultiplier,
                            _playerMovementControllerDatum.SpringJumpHeight * heightMultiplier,
                            _playerMovementControllerDatum.SpringJumpCurve);
                        Instantiate(smokePrefab, root.position, Quaternion.identity);
                        _animator.SetTrigger("Spring");
                    }
                    else
                    {
                        OnJump(_playerMovementControllerDatum.JumpTime, _playerMovementControllerDatum.JumpHeight,
                            _playerMovementControllerDatum.JumpCurve);
                        Instantiate(smokePrefab, root.position, Quaternion.identity);
                    }
                }
            }
        }

        private void QueueJump() => _queueJumpTimer = _playerMovementControllerDatum.QueueJumpTime;

        public void Boomerang()
        {
            if (IsGrounded || !_canBoomerang || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive || disableBoomerang) return;
            OnBoomerang();
        }

        public void Glide()
        {
            if (IsGrounded || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive || disableGlide) return;
            OnGlide();
        }

        public void Grapple()
        {
            if (IsGrounded || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive || !_boomerangTarget || disableGrapple) return;
            OnGrapple();
        }

        public void GroundPound()
        {
            if (IsGrounded || _dashMovementComponent.IsActive || disableGroundPound) return;
            OnGroundPound();
        }

        public void WallSlide()
        {
            if (IsGrounded || _isRolling || _wallSlideMovementComponent.IsActive || _boomerangMovementComponent.IsActive || _grappleMovementComponent.IsActive || _glideMovementComponent.IsActive || _groundPoundMovementComponent.IsActive || _dashMovementComponent.IsActive) return;
            if (CheckWallSlide()) OnWallSlide();
        }

        private bool CheckWallSlide()
        {
            if (disableWallSlideCheck) return false;
            
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
                _wallSlideDirection = direction;
                
                return true;
            }
            return false;
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

        public void Attack()
        {
            if (_isAttacking || !_canAttack || disableAttack) return;
            OnDash();
        }

        public void Crouch()
        {
            if (IsGrounded && _isRolling)
            {
                StopRolling();
                return;
            }
            
            _isCrouching = true;
            GroundPound();
        }

        public void Roll()
        {
            if (_isRolling) return;
            _isRolling = true;
            CurrentMovementControllerDatum = _playerMovementControllerDatum.RollMovementControllerDatum;
            _animator.SetBool("IsRolling", true);
            ForceController.ApplyForce(root.rotation * Vector3.forward * _playerMovementControllerDatum.InitialRollSpeed, ForceMode.VelocityChange);
        }

        private void OnJump(float jumpTime, float jumpHeight, AnimationCurve jumpCurve)
        {
            StopJumping();
            StopBoomeranging();
            StopGliding();
            StopGrappling();
            StopWallSliding();
            
            _jumpMovementComponent.SetJumpMovementData(jumpTime, jumpHeight, jumpCurve, _playerMovementControllerDatum.CutJumpMultiplier);
            _jumpMovementComponent.Activate();
        }

        private void OnBoomerang()
        {
            StopJumping();
            StopGliding();
            StopGrappling();
            StopGroundPounding();
            StopWallSliding();
            StopRolling();
            
            _canBoomerang = false;

            ActivateBoomerangEffect(true);
            _animator.SetTrigger("Boomerang");
            _boomerangMovementComponent.Activate();
        }

        private void OnGlide()
        {
            StopJumping();
            StopBoomeranging();
            StopGrappling();
            StopGroundPounding();
            StopRolling();
            
            CurrentMovementControllerDatum = _playerMovementControllerDatum.GlideMovementControllerDatum;
            _animator.SetTrigger("Parachute");
            _glideMovementComponent.Activate();
        }

        private void OnGrapple()
        {
            StopJumping();
            StopBoomeranging();
            StopGliding();
            StopGrappling();
            StopGroundPounding();
            StopWallSliding();
            StopRolling();
            
            ActivateBoomerangEffect(true);
            _animator.SetTrigger("Boomerang");
            _boomerangTarget.Interact(InteractContext.Construct(gameObject));
            _grappleMovementComponent.SetGrappleTarget(_boomerangTarget.transform);
            _grappleMovementComponent.Activate();
        }

        private void OnGroundPound()
        {
            StopJumping();
            StopBoomeranging();
            StopGliding();
            StopGrappling();
            StopWallSliding();
            StopCrouching();
            StopRolling();
            
            _groundPoundMovementComponent.Activate();
        }
        
        private void GroundPoundMovementComponentOnGroundPounded()
        {
            _canSpringJump = true;
            _springJumpTimer = _playerMovementControllerDatum.GroundPoundSpringJumpTime;
            Instantiate(groundPoundSmokePrefab, root.position, Quaternion.identity);
        }

        private void OnWallSlide()
        {
            _isWallSliding = true;
            SkipGroundPlatformCheck = true;
            CurrentMovementControllerDatum = _playerMovementControllerDatum.WallSlideMovementControllerDatum;
            _wallSlideMovementComponent.SetWallSlideDirection(_wallSlideDirection);
            _wallSlideMovementComponent.Activate();
        }
        
        private void WallSlideMovementComponentOnDeactivated()
        {
            _isWallSliding = false;
            SkipGroundPlatformCheck = false;
            UnPlatform();
        }

        private void OnWallDash()
        {
            _isWallDashing = true;
            _wallDashMovementComponent.SetWallDashDirection(-_wallSlideDirection);
            _wallDashMovementComponent.Activate();
        }
        
        private void WallDashMovementComponentOnDeactivated()
        {
            _isWallDashing = false;
        }

        private void OnDash()
        {
            StopJumping();
            StopBoomeranging();
            StopGliding();
            StopGrappling();
            StopGroundPounding();
            StopWallSliding();
            StopRolling();
            
            _canAttack = false;
            _isAttacking = true;
            
            var attackDirection = Mover?.GetInput() ?? Vector3.zero;
            attackDirection = attackDirection.magnitude < 0.5f ? root.forward : _playerLook.TransformInput(attackDirection);
            attackDirection.y = 0;
            if (MovementDimensions == Dimensions.Two)
            {
                attackDirection.z = 0;
                if (attackDirection.x == 0) attackDirection.x = 1;
            }

            _dashMovementComponent.SetDashDirection(attackDirection);
            _dashMovementComponent.Activate();
        }
        
        private void DashMovementComponentOnDeactivated()
        {
            _isAttacking = false;
        }

        public void StopJumping()
        {
            _jumpMovementComponent.Deactivate();
            if (_isWallDashing) _wallDashMovementComponent.Deactivate();
        }

        public void CutJump() => _jumpMovementComponent.SetCutJump(true);

        public void StopBoomeranging()
        {
            _boomerangMovementComponent.Deactivate();
        }

        public void StopGliding()
        {
            _glideMovementComponent.Deactivate();
        }

        public void StopGrappling()
        {
            _grappleMovementComponent.Deactivate();
        }

        public void StopGroundPounding()
        {
            _groundPoundMovementComponent.Deactivate();
        }

        public void StopWallSliding()
        {
            _wallSlideMovementComponent.Deactivate();
        }

        public void StopWallDashing()
        {
            _wallDashMovementComponent.Deactivate();
        }

        public void StopDashing()
        {
            _dashMovementComponent.Deactivate();
        }

        public void StopCrouching()
        {
            _isCrouching = false;
        }

        public void StopRolling()
        {
            _isRolling = false;
            CurrentMovementControllerDatum = _playerMovementControllerDatum;
            _animator.SetBool("IsRolling", false);
        }
    }
}