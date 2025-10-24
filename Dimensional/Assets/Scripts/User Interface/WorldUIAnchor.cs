using Managers;
using Scriptables.User_Interface;
using UnityEngine;

namespace User_Interface
{
    public abstract class WorldUIAnchor : MonoBehaviour
    {
        protected WorldUIAnchorDatum WorldUIAnchorDatum { get; private set; }

        protected Transform WorldTransform { get; private set; }
        public Transform TargetTransform { get; private set; }
        protected Transform CameraTransform { get; private set; }
        
        protected Camera Camera { get; private set; }

        public void Initialize(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform)
        {
            WorldUIAnchorDatum = worldUIAnchorDatum;
            WorldTransform = worldTransform;
            OnInitialize(worldUIAnchorDatum, worldTransform);
            transform.localScale = Vector3.zero;
        }

        protected abstract void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform);

        public void SetTargetTransform(Transform targetTransform)
        {
            TargetTransform = targetTransform;
            OnSetTargetTransform(targetTransform);
        }

        protected abstract void OnSetTargetTransform(Transform targetTransform);

        private void Start()
        {
            Camera = CameraManager.Instance.Camera;
            CameraTransform = Camera.transform;
        }

        private void FixedUpdate()
        {
            if (!TargetTransform) return;

            OnFixedUpdate();
            var distanceRatio = GetDistanceRatio();
            var angleRatio = GetAngleRatio();
            //Debug.Log($"Distance: {distanceRatio}, Angle: {angleRatio}");
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

        private float GetDistanceRatio()
        {
            var difference =  WorldTransform.position +  WorldUIAnchorDatum.Offset - TargetTransform.position;
            var distance = difference.magnitude;
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
