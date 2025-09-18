using System;
using Managers;
using Scriptables.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Interactables
{
    public class InteractableIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        private InteractableIconDatum _interactableIconDatum;
        private Transform _interactableTransform;
        
        private Camera _camera;
        private Transform _cameraTransform;

        public void Initialize(InteractableIconDatum interactableIconDatum, Transform interactableTransform)
        {
            _interactableTransform = interactableTransform;
            _interactableIconDatum = interactableIconDatum;
            image.sprite = _interactableIconDatum.Icon;
        }

        private void Start()
        {
            _camera = CameraManager.Instance.Camera;
            _cameraTransform = _camera.transform;
        }

        private void LateUpdate()
        {
            var screenPosition = _camera.WorldToScreenPoint(_interactableTransform.position + _interactableIconDatum.Offset);
            transform.position = screenPosition;
        }

        private void FixedUpdate()
        {
            var difference =  _interactableTransform.position +  _interactableIconDatum.Offset - _cameraTransform.position;
            var ratio = Mathf.Max(GetDistanceRatio(difference), GetDotRatio(difference));
            if (ratio > 1)
            {
                if (image.enabled) image.enabled = false;
                return;
            }
            if (!image.enabled) image.enabled = true;
            var size = _interactableIconDatum.SizeCurve.Evaluate(ratio);
            transform.localScale = new Vector3(size, size, size);
        }

        private float GetDistanceRatio(Vector3 difference)
        {
            var distance = difference.magnitude;
            return (distance - _interactableIconDatum.MinRange) / (_interactableIconDatum.MaxRange - _interactableIconDatum.MinRange);
        }

        private float GetDotRatio(Vector3 difference)
        {
            var dot = Vector3.Dot(_cameraTransform.forward, difference.normalized);
            return (dot - _interactableIconDatum.MinDot) / (_interactableIconDatum.MaxDot - _interactableIconDatum.MinDot);
        }
    }
}
