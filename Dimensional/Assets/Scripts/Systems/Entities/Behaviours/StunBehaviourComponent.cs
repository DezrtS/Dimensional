using System;
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
            _isRecovering = true;
            _timer = 0;
            Landed?.Invoke();
        }

        private void FixedUpdate()
        {
            if (_isStunned)
            {
                _timer += Time.fixedDeltaTime;
                var currentVelocity = _forceController.GetVelocityComponent(VelocityType.Movement);
                var velocity = _rotation * new Vector3(0, 0, stunBehaviourDatum.ZCurve.Evaluate(_timer / stunBehaviourDatum.ZDuration) * stunBehaviourDatum.ZVelocity);
                velocity.y = currentVelocity.y;
                _forceController.SetVelocityComponent(VelocityType.Movement, velocity);   
            }
            else if (_isRecovering)
            {
                _timer += Time.fixedDeltaTime;
                if (_timer < stunBehaviourDatum.RecoverDelay) return;
                _isRecovering = false;
                _timer = 0;
                Recovered?.Invoke();
                if (_animator) _animator.SetTrigger(RecoverHash);
            }
        }
    }
}
