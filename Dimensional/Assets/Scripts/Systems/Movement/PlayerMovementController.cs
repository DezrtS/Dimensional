using System;
using System.Collections;
using Managers;
using Scriptables.Movement;
using Systems.Player;
using UnityEngine;

namespace Systems.Movement
{
    public class PlayerMovementController : MovementController
    {
        [SerializeField] private Transform root;
        [SerializeField] private float scaleStrength = 0.1f;
        [SerializeField] private float scaleSpeed = 10;
        
        [SerializeField] private float wallSlideThreshold = 0.2f;
        
        [SerializeField] private bool disableWallSlideCheck;
        [SerializeField] private float wallSlideChecks = 8;
        [SerializeField] private float wallSlideCheckDistance;
        [SerializeField] private Vector3 wallSlideCheckOffset;
        [SerializeField] private LayerMask wallSlideLayerMask;

        [Space] 
        [SerializeField] private GameObject smokePrefab; 
        [SerializeField] private GameObject balloonJumpSmokePrefab; 
        [SerializeField] private GameObject groundPoundSmokePrefab;
        
        private PlayerMovementControllerDatum _playerMovementControllerDatum;
        private PlayerLook _playerLook;
        private Animator _animator;
        
        private bool _isJumping;
        private bool _isWallJumping;
        private bool _isCrouching;
        private bool _isGroundPounding;
        private bool _isRolling;
        private bool _isWallSliding;
        private bool _isBoomeranging;
        private bool _isAttacking;
        
        private bool _cutJump;
        
        private bool _canDoubleJump;
        private bool _canSpringJump;
        private bool _canAttack;

        private Vector3 _wallSlideDirection;
        
        private float _springJumpTimer;
        private float _queueJumpTimer;
        private float _coyoteTimer;
        
        private Coroutine _jumpCoroutine;
        private Coroutine _boomerangCoroutine;
        private Coroutine _groundPoundCoroutine;
        private Coroutine _wallSlideCoroutine;
        private Coroutine _wallJumpCoroutine;
        private Coroutine _attackCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _playerMovementControllerDatum = (PlayerMovementControllerDatum)MovementControllerDatum;
            _playerLook = GetComponent<PlayerLook>();
            _animator = GetComponent<Animator>();
        }
        
        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var velocity = ForceController.GetVelocity();
            root.localScale = Vector3.Lerp(root.localScale, new Vector3(1, 1 + Mathf.Abs(velocity.y * scaleStrength), 1), deltaTime * scaleSpeed);
            velocity.y = 0;
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

            if (!_canAttack) _canAttack = IsGrounded;
            if (!_canDoubleJump) _canDoubleJump = IsGrounded;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (disableWallSlideCheck || IsGrounded || _isWallSliding || _isGroundPounding || _isRolling || _isAttacking || _isBoomeranging) return;
            
            var velocity = ForceController.GetVelocity();
            if (!(velocity.y < wallSlideThreshold)) return;
            velocity.y = 0;

            if (velocity.magnitude < 0.5f) return;
            var intervalAngle = (int)(360 / wallSlideChecks);
            for (var i = 0; i < wallSlideChecks; i++)
            {
                var direction = Quaternion.Euler(0, i * intervalAngle, 0) * Vector3.forward;
                if (Vector3.Dot(velocity, direction) < 0) continue;
                if (!Physics.Raycast(transform.position + wallSlideCheckOffset, direction, wallSlideCheckDistance, wallSlideLayerMask)) continue;
                _wallSlideDirection = direction;
                OnWallSlide();
            }
        }

        protected override void OnMove(Vector3 input, MovementControllerDatum datum)
        {
            if (_isWallSliding)
            {
                if (input.magnitude < 0.5f) return;
                if (Vector3.Angle(_wallSlideDirection, input) < _playerMovementControllerDatum.MinExitAngle) return;
                CancelWallSlide();
            }
            else if (_isBoomeranging)
            {
                base.OnMove(input, _playerMovementControllerDatum.BoomerangMovementControllerDatum);
                return;
            }
            base.OnMove(input, _isRolling ? _playerMovementControllerDatum.RollMovementControllerDatum : datum);
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

            if (!IsGrounded)
            {
                if (_isWallSliding)
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
                        OnJump(_playerMovementControllerDatum.SpringJumpTime, _playerMovementControllerDatum.SpringJumpHeight, _playerMovementControllerDatum.SpringJumpCurve);
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
            CancelBoomerang();
            
            _jumpCoroutine = StartCoroutine(JumpCoroutine(jumpTime, jumpHeight, jumpCurve));
        }

        public void StopJumping()
        {
            _cutJump = true;
        }

        private void CancelBoomerang()
        {
            ForceController.UseGravity = true;
            if (!_isBoomeranging) return;
            StopCoroutine(_boomerangCoroutine);
            _isBoomeranging = false;
            _animator.SetTrigger("Sphere");
        }

        public void Boomerang()
        {
            if (IsGrounded || _isAttacking || _isWallSliding) return;
            OnBoomerang();
        }

        private void OnBoomerang()
        {
            CancelJump();
            CancelGroundPound();
            _isRolling = false;
            _boomerangCoroutine = StartCoroutine(BoomerangCoroutine());
        }

        public void StopBoomerang()
        {
            if (!_isBoomeranging) return;
            CancelBoomerang();
        }
        
        private void Roll()
        {
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
        
        private IEnumerator BoomerangCoroutine()
        {
            _animator.SetTrigger("Boomerang");
            ForceController.UseGravity = false;
            _isBoomeranging = true;
            
            var velocity = ForceController.GetVelocity();
            var initialY = velocity.y;
            var boomerangSlowDownTimer = 0f;
            
            var boomerangSlowDownSpeed = _playerMovementControllerDatum.BoomerangSlowDownSpeed;
            var diff = boomerangSlowDownSpeed - initialY;
            var multiplier = _playerMovementControllerDatum.BoomerangSlowDownMultiplierCurve.Evaluate(initialY / boomerangSlowDownSpeed);
            var boomerangSlowDownTime = _playerMovementControllerDatum.BoomerangSlowDownTime * multiplier;
            var boomerangSlowDownCurve = _playerMovementControllerDatum.BoomerangSlowDownCurve;

            while (boomerangSlowDownTimer < boomerangSlowDownTime)
            {
                if (IsGrounded)
                {
                    CancelBoomerang();
                    yield break;
                }
                
                var normalizedTime = boomerangSlowDownTimer / boomerangSlowDownTime;
                var verticalVelocity = initialY + diff * boomerangSlowDownCurve.Evaluate(normalizedTime);
                
                velocity = ForceController.GetVelocity();
                velocity.y = verticalVelocity;
                ForceController.SetVelocity(velocity);
                
                boomerangSlowDownTimer += Time.deltaTime;
                yield return null;
            }

            var boomerangFallTimer = 0f;
            
            var boomerangFallSpeed = _playerMovementControllerDatum.BoomerangFallSpeed;
            diff = boomerangFallSpeed - boomerangSlowDownSpeed;
            var boomerangFallTime = _playerMovementControllerDatum.BoomerangFallTime;
            var boomerangFallCurve = _playerMovementControllerDatum.BoomerangFallCurve;
            
            while (!IsGrounded)
            {
                var normalizedTime = Mathf.Clamp01(boomerangFallTimer / boomerangFallTime);
                var verticalVelocity = boomerangSlowDownSpeed + diff * boomerangFallCurve.Evaluate(normalizedTime);
                
                velocity = ForceController.GetVelocity();
                velocity.y = verticalVelocity;
                ForceController.SetVelocity(velocity);
                
                boomerangFallTimer += Time.deltaTime;
                yield return null;
            }
            
            ForceController.UseGravity = true;
            _isBoomeranging = false;
            _animator.SetTrigger("Sphere");
        }

        private IEnumerator GroundPoundCoroutine()
        {
            ForceController.UseGravity = false;
            _isGroundPounding = true;
            //OnGrounded += MovementControllerOnGrounded
            
            var groundPoundTimer = 0f;
            
            var groundPoundSpeed = _playerMovementControllerDatum.GroundPoundSpeed;
            var groundPoundTime = _playerMovementControllerDatum.GroundPoundTime;
            var groundPoundCurve = _playerMovementControllerDatum.GroundPoundCurve;

            while (!IsGrounded)
            {
                var normalizedTime = Mathf.Clamp01(groundPoundTimer / groundPoundTime);
                var verticalVelocity = groundPoundCurve.Evaluate(normalizedTime) * groundPoundSpeed;
                ForceController.SetVelocity(Vector3.up * verticalVelocity);
                groundPoundTimer += Time.deltaTime;
                yield return null;
            }
            
            //OnGrounded -= MovementControllerOnGrounded
            
            Instantiate(groundPoundSmokePrefab, root.position, Quaternion.identity);
            
            _springJumpTimer = _playerMovementControllerDatum.AfterGroundPoundTime;
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

            while (!IsGrounded && Physics.Raycast(transform.position + wallSlideCheckOffset, _wallSlideDirection, wallSlideCheckDistance, wallSlideLayerMask))
            {
                var normalizedTime = Mathf.Clamp01(wallSlideTimer / wallSlideTime);
                var verticalVelocity = wallSlideCurve.Evaluate(normalizedTime) * wallSlideSpeed;
                ForceController.SetVelocity(Vector3.up * verticalVelocity);
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
}
