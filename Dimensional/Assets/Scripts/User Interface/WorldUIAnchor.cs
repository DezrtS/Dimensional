using Managers;
using Scriptables.User_Interface;
using Systems.Player;
using UnityEngine;

namespace User_Interface
{
    public abstract class WorldUIAnchor : MonoBehaviour
    {
        protected WorldUIAnchorDatum WorldUIAnchorDatum { get; private set; }

        protected GameObject HolderGameObject { get; private set; }
        protected Transform WorldTransform { get; private set; }
        public Transform TargetTransform { get; private set; }
        protected Transform CameraTransform { get; private set; }

        private bool _isTargetInRange;
        protected Camera Camera { get; private set; }

        public void Initialize(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform)
        {
            WorldUIAnchorDatum = worldUIAnchorDatum;
            HolderGameObject = holderGameObject;
            WorldTransform = worldTransform;
            OnInitialize(worldUIAnchorDatum, holderGameObject, worldTransform);
            transform.localScale = Vector3.zero;
        }

        protected abstract void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform);

        private void Start()
        {
            Camera = CameraManager.Instance.Camera;
            CameraTransform = Camera.transform;
            TargetTransform = PlayerController.Instance.transform;
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
            var targetDistance = GetTargetDistance();
            SetIsTargetInRange(targetDistance <= WorldUIAnchorDatum.Range);
            
            var distanceRatio = WorldUIAnchorDatum.UseDistanceScaling ? GetDistanceRatio() : 0;
            var angleRatio = WorldUIAnchorDatum.UseAngleScaling ? GetAngleRatio() : 0;
            var ratio = Mathf.Clamp01(Mathf.Max(distanceRatio, angleRatio));
            var size = WorldUIAnchorDatum.SizeCurve.Evaluate(ratio);
            transform.localScale = new Vector3(size, size, size);
        }
        
        protected virtual void OnFixedUpdate() {}

        private void LateUpdate()
        {
            var screenPosition = Camera.WorldToScreenPoint(WorldTransform.position + WorldUIAnchorDatum.Offset);
            transform.position = screenPosition;
        }

        private void SetIsTargetInRange(bool isTargetInRange)
        {
            if (_isTargetInRange == isTargetInRange) return;
            _isTargetInRange = isTargetInRange;
            OnSetIsTargetInRange(isTargetInRange);
        }
        
        protected virtual void OnSetIsTargetInRange(bool isTargetInRange) {}

        private float GetTargetDistance()
        {
            var difference =  WorldTransform.position +  WorldUIAnchorDatum.Offset - TargetTransform.position;
            return difference.magnitude;
        }

        private float GetDistanceRatio()
        {
            var distance = GetTargetDistance();
            return (distance - WorldUIAnchorDatum.MinDistance) / (WorldUIAnchorDatum.MaxDistance - WorldUIAnchorDatum.MinDistance);
        }
        
        private float GetAngleRatio()
        {
            var difference =  WorldTransform.position +  WorldUIAnchorDatum.Offset - CameraTransform.position;
            var angle = Vector3.Angle(CameraTransform.forward, difference.normalized);
            return (angle - WorldUIAnchorDatum.MinAngle) / (WorldUIAnchorDatum.MaxAngle - WorldUIAnchorDatum.MinAngle);
        }
    }
}
