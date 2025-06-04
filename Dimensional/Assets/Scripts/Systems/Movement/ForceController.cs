using UnityEngine;

namespace Systems.Movement
{
    public abstract class ForceController : MonoBehaviour
    {
        private bool _isKinematic;
        private bool _useGravity;
        private bool _isDisabled;
        
        public bool IsKinematic
        {
            get => _isKinematic;
            set
            {
                _isKinematic = value;
                OnSetIsKinematic();
            }
        }

        public bool UseGravity
        {
            get => _useGravity;
            set
            {
                _useGravity = value;
                OnSetUseGravity();
            }
        }

        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                _isDisabled = value;
                OnSetIsDisabled();
            }
        }

        protected virtual void OnSetIsKinematic() {}
        protected virtual void OnSetUseGravity() {}
        protected virtual void OnSetIsDisabled() {}

        public abstract Vector3 GetVelocity();
        public void SetVelocity(Vector3 velocity)
        {
            if (_isDisabled) return;
            OnSetVelocity(velocity);
        }
        protected abstract void OnSetVelocity(Vector3 velocity);
        
        public virtual Quaternion GetRotation() { return transform.rotation; }

        public void SetRotation(Quaternion rotation)
        {
            if (_isDisabled) return;
            OnSetRotation(rotation);
        }
        protected virtual void OnSetRotation(Quaternion rotation) => transform.rotation = rotation;

        public void ApplyForce(Vector3 force, ForceMode forceMode)
        {
            if (_isDisabled) return;
            OnApplyForce(force, forceMode);
        }
        protected abstract void OnApplyForce(Vector3 force, ForceMode forceMode);

        public void ApplyTorque(Vector3 torque, ForceMode forceMode)
        {
            if (_isDisabled) return;
            OnApplyTorque(torque, forceMode);
        }
        protected abstract void OnApplyTorque(Vector3 torque, ForceMode forceMode);

        public void Teleport(Vector3 position)
        {
            if (_isDisabled) return;
            OnTeleport(position);
        }
        protected virtual void OnTeleport(Vector3 position) => transform.position = position;
    }
}
