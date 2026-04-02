using System;
using System.Collections;
using Systems.Forces;
using Scriptables.Entities;
using Systems.Movement;
using UnityEngine;

namespace Systems.Entities.Behaviours
{
    public class StunBehaviourComponent : MonoBehaviour
    {
        private static readonly int StunHash = Animator.StringToHash("Stun");
        private static readonly int RecoverHash = Animator.StringToHash("Recover");
        private static readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
        public event Action Stunned;
        public event Action Landed;
        public event Action Recovered;
        
        [SerializeField] private StunBehaviourDatum stunBehaviourDatum;
        
        private bool _isStunned;
        private bool _isRecovering;
        private float _timer;
        
        private MovementController _movementController;
        private ComplexForceController _forceController;
        private Quaternion _rotation;
        private Animator _animator;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            _forceController = GetComponent<ComplexForceController>();
            _animator = GetComponent<Animator>();
        }

        public void Stun(Vector3 direction)
        {
            if (_isStunned || _isRecovering) return;
            if (_animator) _animator.SetBool(IsStunnedHash, true);
            _isStunned = true;
            _isRecovering = false;
            _timer = 0;
            _rotation = Quaternion.LookRotation(direction);
            _forceController.SetVelocityComponent(VelocityType.Movement, new Vector3(0, stunBehaviourDatum.YVelocity, 0));
            _movementController.Grounded += MovementControllerOnGrounded;
            Stunned?.Invoke();
            if (_animator) _animator.SetTrigger(StunHash);
        }

        private void MovementControllerOnGrounded(bool isGrounded)
        {
            if (!isGrounded) return;
            _movementController.Grounded -= MovementControllerOnGrounded;
            _isStunned = false;
            _timer = 0;
            Landed?.Invoke();
            StartCoroutine(RecoverRoutine());
        }

        private void FixedUpdate()
        {
            if (!_isStunned) return;
            _timer += Time.fixedDeltaTime;
            var currentVelocity = _forceController.GetVelocityComponent(VelocityType.Movement);
            var velocity = _rotation * new Vector3(0, 0, stunBehaviourDatum.ZCurve.Evaluate(_timer / stunBehaviourDatum.ZDuration) * stunBehaviourDatum.ZVelocity);
            velocity.y = currentVelocity.y;
            _forceController.SetVelocityComponent(VelocityType.Movement, velocity);
        }

        private IEnumerator RecoverRoutine()
        {
            yield return new WaitForSeconds(stunBehaviourDatum.RecoverDelay);
            if (_animator) _animator.SetTrigger(RecoverHash);
            yield return new WaitForSeconds(0.25f);
            _isRecovering = false;
            Recovered?.Invoke();
            if (_animator) _animator.SetBool(IsStunnedHash, false);
        }
    }
}
