using Managers;
using Scriptables.User_Interface;
using UnityEngine;

namespace User_Interface
{
    public abstract class WorldUIAnchor : MonoBehaviour
    {
        private WorldUIAnchorDatum _worldUIAnchorDatum;
        private Transform _worldTransform;

        private Camera _camera;
        private Transform _cameraTransform;

        public Transform TargetTransform { get; private set; }

        public void Initialize(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform)
        {
            _worldUIAnchorDatum = worldUIAnchorDatum;
            _worldTransform = worldTransform;
            OnInitialize(worldUIAnchorDatum, worldTransform);
            transform.localScale = new Vector3(0, 0, 0);
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
            _camera = CameraManager.Instance.Camera;
            _cameraTransform = _camera.transform;
        }

        private void FixedUpdate()
        {
            if (!TargetTransform) return;

            var distanceRatio = GetDistanceRatio();
            var angleRatio = GetAngleRatio();
            //Debug.Log($"Distance: {distanceRatio}, Angle: {angleRatio}");
            var ratio = Mathf.Clamp01(Mathf.Max(distanceRatio, angleRatio));
            var size = _worldUIAnchorDatum.SizeCurve.Evaluate(ratio);
            transform.localScale = new Vector3(size, size, size);
        }

        private void LateUpdate()
        {
            var screenPosition = _camera.WorldToScreenPoint(_worldTransform.position + _worldUIAnchorDatum.Offset);
            transform.position = screenPosition;
        }

        private float GetDistanceRatio()
        {
            var difference =  _worldTransform.position +  _worldUIAnchorDatum.Offset - TargetTransform.position;
            var distance = difference.magnitude;
            return (distance - _worldUIAnchorDatum.MinDistance) / (_worldUIAnchorDatum.MaxDistance - _worldUIAnchorDatum.MinDistance);
        }
        
        private float GetAngleRatio()
        {
            var difference =  _worldTransform.position +  _worldUIAnchorDatum.Offset - _cameraTransform.position;
            var angle = Vector3.Angle(_cameraTransform.forward, difference.normalized);
            return (angle - _worldUIAnchorDatum.MinAngle) / (_worldUIAnchorDatum.MaxAngle - _worldUIAnchorDatum.MinAngle);
        }
    }
}
