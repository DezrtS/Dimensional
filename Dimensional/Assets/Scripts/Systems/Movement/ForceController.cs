using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public abstract class ForceController : MonoBehaviour
    {
        private class ForceEvent
        {
            public ForceEventDatum ForceEventDatum;
            public Quaternion Rotation;
        }
        
        [SerializeField] private bool isKinematic;
        [SerializeField] private bool useGravity;
        [SerializeField] private bool isDisabled;
        [Space] 
        [SerializeField] protected float maxFallSpeed;
        [SerializeField] protected float overSpeedDeceleration;

        private ForceEvent _forceEvent;
        private float _forceEventTimer;
        
        public bool IsKinematic
        {
            get => isKinematic;
            set
            {
                isKinematic = value;
                OnSetIsKinematic();
            }
        }

        public bool UseGravity
        {
            get => useGravity;
            set
            {
                useGravity = value;
                OnSetUseGravity();
            }
        }

        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;
                OnSetIsDisabled();
            }
        }

        public float MaxFallSpeed { get => maxFallSpeed; set => maxFallSpeed = value; }
        public abstract float Mass { get; set; }

        protected virtual void Awake()
        {
            OnSetIsKinematic();
            OnSetUseGravity();
            OnSetIsDisabled();
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            OnFixedUpdate(fixedDeltaTime);
            if (isDisabled || isKinematic) return;

            if (_forceEventTimer <= 0) return;
            var elapsedTime = _forceEvent.ForceEventDatum.Duration - _forceEventTimer;
            SetVelocity(_forceEvent.Rotation * _forceEvent.ForceEventDatum.GetVelocity(elapsedTime));
            _forceEventTimer -= fixedDeltaTime;
            if (_forceEventTimer > 0) return;
            _forceEvent = null;
            _forceEventTimer = 0;
        }
        
        protected virtual void OnFixedUpdate(float fixedDeltaTime) {}

        protected virtual void OnSetIsKinematic() {}
        protected virtual void OnSetUseGravity() {}
        protected virtual void OnSetIsDisabled() {}

        public abstract Vector3 GetVelocity();
        public void SetVelocity(Vector3 velocity)
        {
            if (isDisabled || isKinematic) return;
            OnSetVelocity(velocity);
        }
        protected abstract void OnSetVelocity(Vector3 velocity);
        
        public virtual Quaternion GetRotation() { return transform.rotation; }

        public void SetRotation(Quaternion rotation)
        {
            if (isDisabled) return;
            OnSetRotation(rotation);
        }
        protected virtual void OnSetRotation(Quaternion rotation) => transform.rotation = rotation;

        public void ApplyForce(Vector3 force, ForceMode forceMode)
        {
            if (isDisabled || isKinematic) return;
            OnApplyForce(force, forceMode);
        }
        protected abstract void OnApplyForce(Vector3 force, ForceMode forceMode);

        public void ApplyTorque(Vector3 torque, ForceMode forceMode)
        {
            if (isDisabled || isKinematic) return;
            OnApplyTorque(torque, forceMode);
        }
        protected abstract void OnApplyTorque(Vector3 torque, ForceMode forceMode);

        public void ApplyForceEvent(ForceEventDatum forceEventDatum, Quaternion rotation)
        { 
            if (_forceEvent != null) return; 
            _forceEvent = new ForceEvent() { ForceEventDatum = forceEventDatum, Rotation = rotation };
            _forceEventTimer = forceEventDatum.Duration;
        }

        public void Teleport(Vector3 position)
        {
            if (isDisabled) return;
            OnTeleport(position);
        }
        protected virtual void OnTeleport(Vector3 position) => transform.position = position;
        
        public void CancelVelocityInDirection(Vector3 direction)
        {
            if (isDisabled || isKinematic || direction.sqrMagnitude < Mathf.Epsilon) 
                return;
            
            var newVelocity = GetCancelledVector(GetVelocity(), direction);
            OnSetVelocity(newVelocity);
        }

        public static Vector3 GetCancelledVector(Vector3 vector, Vector3 direction)
        {
            var normalizedDirection = direction.normalized;
    
            // Calculate dot product to get velocity magnitude in target direction
            var velocityComponent = Vector3.Dot(vector, normalizedDirection);
    
            // Only cancel if velocity is moving IN the direction (positive dot product)
            if (!(velocityComponent > 0)) return vector;
            var velocityToCancel = velocityComponent * normalizedDirection;
            return vector - velocityToCancel;
        }
    }
}
