using System;
using System.Collections;
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
    /*
    public class PlayerMovementController2 : MovementController
    {
        [Space] 
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        [Space] 
        [SerializeField] private bool disableWallSlideCheck;
        [SerializeField] private bool canShiftWallSlide;
        [SerializeField] private MovementControllerDatum wallSlideMovementControllerDatum;
        [Space]
        [SerializeField] private bool swapBoomerangMovement;
        [SerializeField] private float grappleBoomerangSpeed;
        [SerializeField] private float grappleBoomerangTime;
        [SerializeField] private AnimationCurve grappleBoomerangCurve;
        [SerializeField] private float checkRadius;
        [SerializeField] private LayerMask checkLayer;
        [SerializeField] private float maxAngleDifference;
        [Space]
        [SerializeField] private VisualEffect[] visualEffects;
        [Space] 
        [SerializeField] private GameObject smokePrefab; 
        [SerializeField] private GameObject balloonJumpSmokePrefab; 
        [SerializeField] private GameObject groundPoundSmokePrefab;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerLook _playerLook;
        private Animator _animator;
        
        private bool _isJumping;
        private bool _isParachuting;
        private bool _isBoomeranging;
        private bool _isWallJumping;
        private bool _isCrouching;
        private bool _isGroundPounding;
        private bool _isRolling;
        private bool _isWallSliding;
        private bool _isAttacking;
        
        private bool _cutJump;
        
        private bool _canDoubleJump;
        private bool _canSpringJump;
        private bool _canBoomerang;
        private bool _canAttack;

        private Vector3 _wallSlideDirection;
        private PowerLevel _groundPoundPowerLevel;
        
        private float _springJumpTimer;
        private float _queueJumpTimer;
        private float _coyoteTimer;
        private float _wallSlideCoyoteTimer;
        
        private Coroutine _jumpCoroutine;
        private Coroutine _parachuteCoroutine;
        private Coroutine _boomerangCoroutine;
        private Coroutine _groundPoundCoroutine;
        private Coroutine _wallSlideCoroutine;
        private Coroutine _wallJumpCoroutine;
        private Coroutine _attackCoroutine;
        
        private BoomerangTarget _boomerangTarget;

        protected override void OnAwake()
        {
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
            _playerLook = GetComponent<PlayerLook>();
            _animator = GetComponent<Animator>();
            Grounded += OnGrounded;
        }
        
        private void FixedUpdate()
        {
            if (swapBoomerangMovement)
            {
                var results = new Collider[10];
                var size = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, results, checkLayer, QueryTriggerInteraction.Collide);
    
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
                    if (!(angle <= maxAngleDifference) || !(angle < minAngle)) continue;
                    minAngle = angle;
                    closestTarget = boomerangTarget;
                }
    
                _boomerangTarget = closestTarget; // Store the closest valid target
            }          
            
            var deltaTime = Time.fixedDeltaTime;
            var velocity = ForceController.GetVelocity();
            root.localScale = Vector3.Lerp(root.localScale, new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), deltaTime * scaleSpeed);
            velocity.y = 0;
            _animator.SetFloat("xzVelocity", velocity.magnitude);
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
                if ((IsGrounded || _isWallSliding) && !_isGroundPounding)
                {
                    _queueJumpTimer = 0;
                    Jump(true);
                }
                _queueJumpTimer -= deltaTime;
            }
            
            if (_coyoteTimer > 0)
            {
                _coyoteTimer -= deltaTime;
            }

            if (_wallSlideCoyoteTimer > 0)
            {
                _wallSlideCoyoteTimer -= deltaTime;
            }

            if (!_canAttack) _canAttack = IsGrounded;
            if (!_canDoubleJump) _canDoubleJump = IsGrounded;
            if (!_canBoomerang) _canBoomerang = IsGrounded;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (disableWallSlideCheck || IsGrounded || _isWallSliding || _isGroundPounding || _isRolling || _isAttacking || _isBoomeranging || _isParachuting) return;
            
            var velocity = ForceController.GetVelocity();
            if (!(velocity.y < _playerMovementControllerDatum.WallSlideYVelocityThreshold)) return;
            velocity.y = 0;

            if (velocity.magnitude < 0.5f) return;
            var intervalAngle = (int)(360 / _playerMovementControllerDatum.WallSlideChecks);
            for (var i = 0; i < _playerMovementControllerDatum.WallSlideChecks; i++)
            {
                var direction = Quaternion.Euler(0, i * intervalAngle, 0) * Vector3.forward;
                if (Vector3.Dot(velocity, direction) < 0) continue;
                if (!Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset, direction, _playerMovementControllerDatum.WallSlideCheckDistance, _playerMovementControllerDatum.WallSlideLayerMask)) continue;
                _wallSlideDirection = direction;
                OnWallSlide();
            }
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (_isWallSliding)
            {
                if (canShiftWallSlide) base.OnMove(input, wallSlideMovementControllerDatum);
                
                if (input.magnitude < 0.5f) return;
                if (Vector3.Angle(_wallSlideDirection, input) < _playerMovementControllerDatum.WallSlideMinExitAngle) return;
                CancelWallSlide();
                _wallSlideCoyoteTimer = _playerMovementControllerDatum.WallSlideCoyoteTime;
            }
            else if (_isParachuting)
            {
                base.OnMove(input, _playerMovementControllerDatum.ParachuteMovementControllerDatum);
                return;
            }
            base.OnMove(input, _isRolling ? _playerMovementControllerDatum.RollMovementControllerDatum : datum);
        }

        private void OnGrounded(bool isGrounded)
        {
            if (!isGrounded && !_isJumping)
            {
                _coyoteTimer = _playerMovementControllerDatum.CoyoteTime;
            }
        }

        private void CancelWallSlide()
        {
            ForceController.UseGravity = true;
            if (_isWallSliding) StopCoroutine(_wallSlideCoroutine);
            _isWallSliding = false;
        }

        private void OnWallSlide()
        {
            if (_wallSlideCoroutine != null) StopCoroutine(_wallSlideCoroutine);
            _wallSlideCoroutine = StartCoroutine(WallSlideCoroutine());
            root.forward = -_wallSlideDirection;
        }

        private void OnWallJump()
        {
            if (_wallJumpCoroutine != null) StopCoroutine(_wallJumpCoroutine);
            _wallJumpCoroutine = StartCoroutine(WallJumpCoroutine());
        }

        private void QueueJump()
        {
            _queueJumpTimer = _playerMovementControllerDatum.QueueJumpTime;
        }

        private void CancelJump()
        {
            _animator.SetTrigger("Sphere");
            ForceController.UseGravity = true;
            _cutJump = false;
            if (_isJumping)
            {
                StopCoroutine(_jumpCoroutine);
                _isJumping = false;
            }

            if (!_isWallJumping) return;
            StopCoroutine(_wallJumpCoroutine);
            _isWallJumping = false;
        }

        public void Jump(bool skipCutJump = false)
        {
            if (!skipCutJump) _cutJump = false;
            if (_isAttacking || _isGroundPounding)
            {
                QueueJump();
                return;
            }

            if (!IsGrounded && _coyoteTimer <= 0)
            {
                if (_isWallSliding || _wallSlideCoyoteTimer > 0)
                {
                    OnJump(_playerMovementControllerDatum.WallJumpHeightTime, _playerMovementControllerDatum.WallJumpHeight, _playerMovementControllerDatum.WallJumpHeightCurve);
                    OnWallJump(); // May need to separate from CancelJump();
                    Instantiate(smokePrefab, root.position, Quaternion.identity);
                }
                else if (_canDoubleJump)
                {
                    _isRolling = false;
                    _canDoubleJump = false;
                    OnJump(_playerMovementControllerDatum.BalloonJumpTime, _playerMovementControllerDatum.BalloonJumpHeight, _playerMovementControllerDatum.BalloonJumpCurve);
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
                    if (_canSpringJump)
                    {
                        var timeMultiplier = _groundPoundPowerLevel switch
                        {
                            PowerLevel.Low =>
                                1,
                            PowerLevel.Medium =>
                                _playerMovementControllerDatum.SpringJumpMediumPowerTimeMultiplier,
                            PowerLevel.High =>
                                _playerMovementControllerDatum.SpringJumpHighPowerTimeMultiplier,
                            _ => throw new System.ArgumentException("Unsupported power level")
                        };
                        
                        var heightMultiplier = _groundPoundPowerLevel switch
                        {
                            PowerLevel.Low =>
                                1,
                            PowerLevel.Medium =>
                                _playerMovementControllerDatum.SpringJumpMediumPowerHeightMultiplier,
                            PowerLevel.High =>
                                _playerMovementControllerDatum.SpringJumpHighPowerHeightMultiplier,
                            _ => throw new System.ArgumentException("Unsupported power level")
                        };
                        
                        OnJump(_playerMovementControllerDatum.SpringJumpTime * timeMultiplier, _playerMovementControllerDatum.SpringJumpHeight * heightMultiplier, _playerMovementControllerDatum.SpringJumpCurve);
                        Instantiate(smokePrefab, root.position, Quaternion.identity);
                        _animator.SetTrigger("Spring");
                    }
                    else
                    {
                        OnJump(_playerMovementControllerDatum.JumpTime, _playerMovementControllerDatum.JumpHeight, _playerMovementControllerDatum.JumpCurve);
                        Instantiate(smokePrefab, root.position, Quaternion.identity);
                    }
                }
            }
        }

        private void OnJump(float jumpTime, float jumpHeight, AnimationCurve jumpCurve)
        {
            CancelWallSlide();
            CancelJump();
            CancelParachute();
            CancelBoomerang();
            
            _jumpCoroutine = StartCoroutine(JumpCoroutine(jumpTime, jumpHeight, jumpCurve));
        }

        public void StopJumping()
        {
            _cutJump = true;
        }

        private void CancelParachute()
        {
            ForceController.UseGravity = true;
            if (!_isParachuting) return;
            StopCoroutine(_parachuteCoroutine);
            _isParachuting = false;
            _animator.SetTrigger("Sphere");
        }

        public void Parachute()
        {
            if (IsGrounded || _isAttacking || _isWallJumping || _isWallSliding) return;
            OnParachute();
        }

        private void OnParachute()
        {
            CancelJump();
            CancelBoomerang();
            CancelGroundPound();
            _isRolling = false;
            _parachuteCoroutine = StartCoroutine(ParachuteCoroutine());
        }
        
        public void StopParachute()
        {
            if (!_isParachuting) return;
            CancelParachute();
        }

        private void CancelBoomerang()
        {
            ForceController.UseGravity = true;
            if (!_isBoomeranging) return;
            StopCoroutine(_boomerangCoroutine);
            _isBoomeranging = false;
            _animator.SetTrigger("Sphere");
            foreach (var visualEffect in visualEffects)
            {
                visualEffect.Stop();
            }
        }

        public void Boomerang()
        {
            if (IsGrounded || !_canBoomerang || _isAttacking || _isWallSliding) return;

            if (swapBoomerangMovement && !_boomerangTarget) return;
            OnBoomerang();
        }

        private void OnBoomerang()
        {
            CancelJump();
            CancelParachute();
            CancelGroundPound();
            _isRolling = false;
            if (!swapBoomerangMovement) _canBoomerang = false;
            _boomerangCoroutine = StartCoroutine(swapBoomerangMovement ? BoomerangCoroutine2() : BoomerangCoroutine());
            foreach (var visualEffect in visualEffects)
            {
                visualEffect.SendEvent("OnPlay");
            }
        }

        public void StopBoomerang()
        {
            if (!_isBoomeranging) return;
            CancelBoomerang();
        }
        
        private void Roll()
        {
            if (_isRolling) return;
            _isRolling = true;
            ForceController.ApplyForce(root.rotation * Vector3.forward * _playerMovementControllerDatum.InitialRollSpeed, ForceMode.VelocityChange);
        }

        private void CancelGroundPound()
        {
            ForceController.UseGravity = true;
            if (_isGroundPounding) StopCoroutine(_groundPoundCoroutine);
            _isGroundPounding = false;
        }

        public void Crouch()
        {
            _isCrouching = true;
            if (IsGrounded || _isGroundPounding || _isAttacking) return;
            _isCrouching = false;
            OnGroundPound();
        }

        private void OnGroundPound()
        {
            _isRolling = false;
            CancelJump();
            CancelParachute();
            CancelBoomerang();
            CancelWallSlide();
            
            if (_groundPoundCoroutine != null) StopCoroutine(_groundPoundCoroutine);
            _groundPoundCoroutine = StartCoroutine(GroundPoundCoroutine());
        }

        public void StopCrouching()
        {
            _isCrouching = false;
        }

        public void Attack()
        {
            if (_isAttacking || !_canAttack) return;
            OnAttack();
        }

        private void OnAttack()
        {
            _isRolling = false;
            CancelJump();
            CancelBoomerang();
            CancelGroundPound();
            CancelWallSlide();

            _canAttack = false;
            if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }

        private IEnumerator JumpCoroutine(float jumpTime, float jumpHeight, AnimationCurve jumpCurve)
        {
            ForceController.UseGravity = false;
            _isJumping = true;
            var jumpTimer = 0f;
            
            while (jumpTimer < jumpTime)
            {
                var normalizedTime = jumpTimer / jumpTime;
                var slope = GameManager.Derivative(jumpCurve, normalizedTime);
                var verticalVelocity = slope / jumpTime * jumpHeight;
                
                var currentVelocity = ForceController.GetVelocity();
                currentVelocity.y = verticalVelocity;
                
                if (_cutJump)
                {
                    _cutJump = false;
                    currentVelocity.y *= _playerMovementControllerDatum.CutJumpMultiplier;
                    ForceController.SetVelocity(currentVelocity);
                    break;
                }
                
                ForceController.SetVelocity(currentVelocity);
                jumpTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = true;
            _isJumping = false;
            _animator.SetTrigger("Sphere");
        }
        
        private IEnumerator ParachuteCoroutine()
        {
            _animator.SetTrigger("Parachute");
            _isParachuting = true;

            var velocity = ForceController.GetVelocity();
            while (velocity.y > 0 && !IsGrounded)
            {
                yield return new WaitForFixedUpdate();
                velocity = ForceController.GetVelocity();
            }
            
            var initialY = velocity.y;
            var parachuteOpenTimer = 0f;
            
            var parachuteFallSpeed = _playerMovementControllerDatum.ParachuteFallSpeed;
            var parachuteOpenTime = _playerMovementControllerDatum.ParachuteOpenTime * _playerMovementControllerDatum.ParachuteOpenTimeMultiplierCurve.Evaluate(initialY / parachuteFallSpeed);

            while (parachuteOpenTimer < parachuteOpenTime)
            {
                if (IsGrounded)
                {
                    CancelParachute();
                    yield break;
                }
                parachuteOpenTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = false;
            initialY = ForceController.GetVelocity().y;
            var parachuteSlowDownTimer = 0f;
            
            var diff = parachuteFallSpeed - initialY;
            var parachuteSlowDownTime = _playerMovementControllerDatum.ParachuteSlowDownTime * _playerMovementControllerDatum.ParachuteSlowDownMultiplierCurve.Evaluate(initialY / parachuteFallSpeed);
            var parachuteSlowDownCurve = _playerMovementControllerDatum.ParachuteSlowDownCurve;

            while (!IsGrounded)
            {
                var normalizedTime = Mathf.Clamp01(parachuteSlowDownTimer / parachuteSlowDownTime);
                var verticalVelocity = initialY + diff * parachuteSlowDownCurve.Evaluate(normalizedTime);
                
                velocity = ForceController.GetVelocity();
                velocity.y = verticalVelocity;
                ForceController.SetVelocity(velocity);
                
                parachuteSlowDownTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = true;
            _isParachuting = false;
            _animator.SetTrigger("Sphere");
        }
        
        private IEnumerator BoomerangCoroutine()
        {
            _animator.SetTrigger("Boomerang");
            ForceController.UseGravity = false;
            _isBoomeranging = true;
            
            var direction = Mover?.GetInput() ?? Vector3.zero;
            direction = direction.magnitude < 0.5f ? root.forward : _playerLook.TransformInput(direction);
            if (MovementDimensions == Dimensions.Two)
            {
                direction.y = direction.z;
                direction.z = 0;
            }
            
            var boomerangTimer = 0f;
            
            var boomerangSpeed = _playerMovementControllerDatum.BoomerangSpeed;
            var boomerangTime = _playerMovementControllerDatum.BoomerangTime;
            var boomerangCurve = _playerMovementControllerDatum.BoomerangCurve;

            while (boomerangTimer < boomerangTime)
            {
                if (IsGrounded)
                {
                    CancelBoomerang();
                    yield break;
                }
                
                var normalizedTime = boomerangTimer / boomerangTime;
                var boomerangVelocity = boomerangSpeed * boomerangCurve.Evaluate(normalizedTime) * direction;
                
                ForceController.SetVelocity(boomerangVelocity);
                
                boomerangTimer += Time.deltaTime;
                yield return null;
            }

            var boomerangReturnTimer = 0f;
            
            var boomerangReturnSpeed = _playerMovementControllerDatum.BoomerangReturnSpeed;
            var boomerangReturnTime = _playerMovementControllerDatum.BoomerangReturnTime;
            var boomerangReturnCurve = _playerMovementControllerDatum.BoomerangReturnCurve;
            
            while (boomerangReturnTimer < boomerangReturnTime)
            {
                var normalizedTime = Mathf.Clamp01(boomerangReturnTimer / boomerangReturnTime);
                var returnVelocity = boomerangReturnSpeed * boomerangReturnCurve.Evaluate(normalizedTime) * -direction;
                
                ForceController.SetVelocity(returnVelocity);
                
                boomerangReturnTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = true;
            _isBoomeranging = false;
            _animator.SetTrigger("Sphere");
            foreach (var visualEffect in visualEffects)
            {
                visualEffect.Stop();
            }
        }
        
        private IEnumerator BoomerangCoroutine2()
        {
            _animator.SetTrigger("Boomerang");
            ForceController.UseGravity = false;
            _isBoomeranging = true;

            var boomerangTarget = _boomerangTarget;
            boomerangTarget.Interact(InteractContext.Construct(gameObject));
            
            var direction = boomerangTarget.transform.position - transform.position;
            if (MovementDimensions == Dimensions.Two) direction.z = 0;
            direction = direction.normalized;
            
            var boomerangTimer = 0f;

            var boomerangSpeed = grappleBoomerangSpeed; //_playerMovementControllerDatum.BoomerangSpeed;
            var boomerangTime = grappleBoomerangTime; //_playerMovementControllerDatum.BoomerangTime;
            var boomerangCurve = grappleBoomerangCurve; //_playerMovementControllerDatum.BoomerangReturnCurve;

            while (true)
            {
                if (IsGrounded)
                {
                    CancelBoomerang();
                    yield break;
                }
                
                var normalizedTime = boomerangTimer / boomerangTime;
                var boomerangVelocity = boomerangSpeed * boomerangCurve.Evaluate(normalizedTime) * direction;
                
                ForceController.SetVelocity(boomerangVelocity);
                
                direction = boomerangTarget.transform.position - transform.position;
                if (MovementDimensions == Dimensions.Two) direction.z = 0;
                direction = direction.normalized;

                if (Vector3.Dot(boomerangVelocity, direction) < 0) break;
                
                boomerangTimer += Time.deltaTime;
                yield return null;
            }

            // COMMENT OUT
            var boomerangReturnTimer = 0f;
            
            var boomerangReturnSpeed = _playerMovementControllerDatum.BoomerangReturnSpeed;
            var boomerangReturnTime = _playerMovementControllerDatum.BoomerangReturnTime;
            var boomerangReturnCurve = _playerMovementControllerDatum.BoomerangReturnCurve;
            
            while (boomerangReturnTimer < boomerangReturnTime)
            {
                var normalizedTime = Mathf.Clamp01(boomerangReturnTimer / boomerangReturnTime);
                var returnVelocity = boomerangReturnSpeed * boomerangReturnCurve.Evaluate(normalizedTime) * -direction;
                
                ForceController.SetVelocity(returnVelocity);
                
                boomerangReturnTimer += Time.deltaTime;
                yield return null;
            }
            // STOP COMMENT OUT
            
            ForceController.UseGravity = true;
            _isBoomeranging = false;
            _animator.SetTrigger("Sphere");
            foreach (var visualEffect in visualEffects)
            {
                visualEffect.Stop();
            }
        }

        private IEnumerator GroundPoundCoroutine()
        {
            ForceController.UseGravity = false;
            _isGroundPounding = true;
            _groundPoundPowerLevel = PowerLevel.Low;
            //OnGrounded += MovementControllerOnGrounded
            
            var groundPoundTimer = 0f;
            
            var groundPoundSpeed = _playerMovementControllerDatum.GroundPoundSpeed;
            var groundPoundTime = _playerMovementControllerDatum.GroundPoundTime;
            var groundPoundCurve = _playerMovementControllerDatum.GroundPoundCurve;
            
            var groundPoundMediumPowerTimeThreshold = _playerMovementControllerDatum.GroundPoundMediumPowerTimeThreshold;
            var groundPoundHighPowerTimeThreshold = _playerMovementControllerDatum.GroundPoundHighPowerTimeThreshold;

            while (!IsGrounded)
            {
                var normalizedTime = Mathf.Clamp01(groundPoundTimer / groundPoundTime);
                var verticalVelocity = groundPoundCurve.Evaluate(normalizedTime) * groundPoundSpeed;
                ForceController.SetVelocity(Vector3.up * verticalVelocity);
                groundPoundTimer += Time.deltaTime;
                switch (_groundPoundPowerLevel)
                {
                    case PowerLevel.Low:
                        if (groundPoundTimer >= groundPoundMediumPowerTimeThreshold) _groundPoundPowerLevel = PowerLevel.Medium;
                        break;
                    case PowerLevel.Medium:
                        if (groundPoundTimer >= groundPoundHighPowerTimeThreshold) _groundPoundPowerLevel = PowerLevel.High;
                        break;
                    case PowerLevel.High:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return null;
            }
            
            //OnGrounded -= MovementControllerOnGrounded
            
            Instantiate(groundPoundSmokePrefab, root.position, Quaternion.identity);
            
            _springJumpTimer = _playerMovementControllerDatum.GroundPoundSpringJumpTime;
            _canSpringJump = true;

            ForceController.UseGravity = true;
            _isGroundPounding = false;
        }

        private void GroundPoundOnGrounded()
        {
            //OnGrounded -= MovementControllerOnGrounded
        }
        
        private IEnumerator WallSlideCoroutine()
        {
            ForceController.UseGravity = false;
            _isWallSliding = true;
            //OnGrounded += MovementControllerOnGrounded
            
            var wallSlideTimer = 0f;
            
            var wallSlideSpeed = _playerMovementControllerDatum.WallSlideSpeed;
            var wallSlideTime = _playerMovementControllerDatum.WallSlideTime;
            var wallSlideCurve = _playerMovementControllerDatum.WallSlideCurve;

            while (!IsGrounded && Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset, _wallSlideDirection, _playerMovementControllerDatum.WallSlideCheckDistance, _playerMovementControllerDatum.WallSlideLayerMask))
            {
                var normalizedTime = Mathf.Clamp01(wallSlideTimer / wallSlideTime);
                var verticalVelocity = wallSlideCurve.Evaluate(normalizedTime) * wallSlideSpeed;
                var velocity = ForceController.GetVelocity();
                if (!canShiftWallSlide) velocity = Vector3.zero;
                velocity.y = verticalVelocity;
                ForceController.SetVelocity(velocity);
                wallSlideTimer += Time.deltaTime;
                yield return null;
            }
            
            //OnGrounded -= MovementControllerOnGrounded

            ForceController.UseGravity = true;
            _isWallSliding = false;
        }
        
        private IEnumerator WallJumpCoroutine()
        {
            ForceController.UseGravity = false;
            _isWallJumping = true;
            //OnGrounded += MovementControllerOnGrounded
            
            var wallJumpSpeedTimer = 0f;
            
            var wallJumpSpeed = _playerMovementControllerDatum.WallJumpSpeed;
            var wallJumpSpeedTime = _playerMovementControllerDatum.WallJumpSpeedTime;
            var wallJumpSpeedCurve = _playerMovementControllerDatum.WallJumpSpeedCurve;

            while (wallJumpSpeedTimer < wallJumpSpeedTime)
            {
                var normalizedTime = wallJumpSpeedTimer / wallJumpSpeedTime;
                var jumpVelocity = wallJumpSpeedCurve.Evaluate(normalizedTime) * wallJumpSpeed * -_wallSlideDirection;
                var velocity = ForceController.GetVelocity();
                velocity.x = jumpVelocity.x;
                velocity.z = jumpVelocity.z;
                ForceController.SetVelocity(velocity);
                wallJumpSpeedTimer += Time.deltaTime;
                yield return null;
            }
            
            //OnGrounded -= MovementControllerOnGrounded

            ForceController.UseGravity = true;
            _isWallJumping = false;
        }

        private IEnumerator AttackCoroutine()
        {
            ForceController.UseGravity = false;
            _isAttacking = true;

            var attackTimer = 0f;

            var attackVector = _playerMovementControllerDatum.AttackVector;
            var attackTime = _playerMovementControllerDatum.AttackTime;
            var attackCurve = _playerMovementControllerDatum.AttackCurve;

            var attackDirection = Mover?.GetInput() ?? Vector3.zero;
            attackDirection = attackDirection.magnitude < 0.5f ? root.forward : _playerLook.TransformInput(attackDirection);
            attackDirection.y = 0;
            if (MovementDimensions == Dimensions.Two)
            {
                attackDirection.z = 0;
                if (attackDirection.x == 0) attackDirection.x = 1;
            }
            var rotation = Quaternion.LookRotation(attackDirection.normalized);

            while (attackTimer < attackTime)
            {
                var normalizedTime = attackTimer / attackTime;
                var slope = GameManager.Derivative(attackCurve, normalizedTime);
                var velocity = slope / attackTime * attackVector;
            
                var newVelocity = rotation * velocity;
                ForceController.SetVelocity(newVelocity);
                attackTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = true;
            _isAttacking = false;
        }
    }
    */

    public class PlayerMovementController : MovementController
    {
        [Space] [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        [Space] [SerializeField] private bool debugResetMovementComponentData;
        [Space] [SerializeField] private bool disableWallSlideCheck;
        [Space] [SerializeField] private VisualEffect[] visualEffects;
        [Space] [SerializeField] private GameObject smokePrefab;
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
            _boomerangMovementComponent.SetBoomerangMovementData(_playerMovementControllerDatum.BoomerangFallSpeedThreshold, _playerMovementControllerDatum.BoomerangFallTimeThreshold, _playerMovementControllerDatum.BoomerangTime, _playerMovementControllerDatum.BoomerangCurve);
            _glideMovementComponent.SetGlideMovementData(_playerMovementControllerDatum.GlideFallSpeedThreshold, _playerMovementControllerDatum.GlideFallTimeThreshold, _playerMovementControllerDatum.GlideFallSpeed, _playerMovementControllerDatum.GlideSlowDownTime, _playerMovementControllerDatum.GlideSlowDownCurve);
            _grappleMovementComponent.SetGrappleMovementData(_playerMovementControllerDatum.GrappleSpeed, _playerMovementControllerDatum.GrappleTime, _playerMovementControllerDatum.GrappleCurve);
            _groundPoundMovementComponent.SetGroundPoundMovementData(_playerMovementControllerDatum.GroundPoundSpeed, _playerMovementControllerDatum.GroundPoundTime, _playerMovementControllerDatum.GroundPoundCurve, _playerMovementControllerDatum.GroundPoundMediumPowerTimeThreshold, _playerMovementControllerDatum.GroundPoundHighPowerTimeThreshold);
            _wallSlideMovementComponent.SetWallSlideMovementData(_playerMovementControllerDatum.WallSlideSpeed, _playerMovementControllerDatum.WallSlideTime, _playerMovementControllerDatum.WallSlideCurve, _playerMovementControllerDatum.WallSlideCheckOffset, _playerMovementControllerDatum.WallSlideCheckDistance, _playerMovementControllerDatum.WallSlideCheckLayerMask);
            _wallDashMovementComponent.SetWallDashMovementData(_playerMovementControllerDatum.WallDashSpeed, _playerMovementControllerDatum.WallDashTime, _playerMovementControllerDatum.WallDashCurve);
            _dashMovementComponent.SetDashMovementData(_playerMovementControllerDatum.AttackVector, _playerMovementControllerDatum.AttackTime, _playerMovementControllerDatum.AttackCurve);
        }

        protected override void OnFixedUpdate()
        {
            UpdateGrappleTarget();

            var deltaTime = Time.fixedDeltaTime;
            var velocity = ForceController.GetVelocity();
            root.localScale = Vector3.Lerp(root.localScale,
                new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), deltaTime * scaleSpeed);
            velocity.y = 0;
            _animator.SetFloat("xzVelocity", velocity.magnitude);
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
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (_isWallSliding)
            {
                base.OnMove(input, _playerMovementControllerDatum.WallSlideMovementControllerDatum);

                if (input.magnitude < 0.5f) return;
                if (Vector3.Angle(_wallSlideDirection, input) <
                    _playerMovementControllerDatum.WallSlideMinExitAngle) return;
                StopWallSliding();
                _wallJumpCoyoteTimer = _playerMovementControllerDatum.WallJumpCoyoteTime;
            }
            else if (_glideMovementComponent.IsActive)
            {
                base.OnMove(input, _playerMovementControllerDatum.GlideMovementControllerDatum);
                return;
            }

            base.OnMove(input, _isRolling ? _playerMovementControllerDatum.RollMovementControllerDatum : datum);
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
                else if (_canDoubleJump)
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
                    if (_canSpringJump)
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
            if (IsGrounded || !_canBoomerang || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive) return;
            OnBoomerang();
        }

        public void Glide()
        {
            if (IsGrounded || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive) return;
            OnGlide();
        }

        public void Grapple()
        {
            if (IsGrounded || _dashMovementComponent.IsActive || _wallSlideMovementComponent.IsActive || !_boomerangTarget) return;
            OnGrapple();
        }

        public void GroundPound()
        {
            if (IsGrounded || _dashMovementComponent.IsActive) return;
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
            input.y = 0;
            if (MovementDimensions == Dimensions.Two) input.z = 0;
            
            Debug.DrawRay(transform.position, input, Color.red, Time.fixedDeltaTime);

            if (input.magnitude < 0.5f) return false;
            
            input = input.normalized;
            var intervalAngle = (int)(360 / _playerMovementControllerDatum.WallSlideCheckIntervals);
            for (var i = 0; i < _playerMovementControllerDatum.WallSlideCheckIntervals; i++)
            {
                var direction = Quaternion.Euler(0, i * intervalAngle, 0) * Vector3.forward;
                if (Vector3.Dot(input, direction) < 0) continue;
                if (!Physics.Raycast(transform.position + _playerMovementControllerDatum.WallSlideCheckOffset,
                        direction, _playerMovementControllerDatum.WallSlideCheckDistance,
                        _playerMovementControllerDatum.WallSlideCheckLayerMask)) continue;
                _wallSlideDirection = direction;
                return true;
            }
            return false;
        }

        public void Attack()
        {
            if (_isAttacking || !_canAttack) return;
            OnDash();
        }

        public void Crouch()
        {
            _isCrouching = true;
            GroundPound();
        }

        public void Roll()
        {
            if (_isRolling) return;
            _isRolling = true;
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
            _grappleMovementComponent.SetGrappleTarget(_boomerangTarget.transform.position);
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
        }

        private void OnWallSlide()
        {
            _isWallSliding = true;
            _wallSlideMovementComponent.SetWallSlideDirection(_wallSlideDirection);
            _wallSlideMovementComponent.Activate();
        }
        
        private void WallSlideMovementComponentOnDeactivated()
        {
            _isWallSliding = false;
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
        }
    }
}