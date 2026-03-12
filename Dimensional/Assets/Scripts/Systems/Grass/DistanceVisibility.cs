using System;
using Managers;
using UnityEngine;

namespace Systems.Grass
{
    public class DistanceVisibility : MonoBehaviour
    {
        private Transform _cameraTransform;
        private GrassSettings _grassSettings;
        private GrassMesh _grassMesh;
        
        private bool _previousIsVisible;
        
        private void Start()
        {
            _cameraTransform = CameraManager.Instance.Camera.transform;
            _grassSettings = EffectManager.Instance.GrassSystem.grassSettings;
            _grassMesh = GetComponent<GrassMesh>();
        }

        private void FixedUpdate()
        {
            var cameraPosition = _cameraTransform.position;
            var dist = Vector3.Distance(cameraPosition, transform.position);
            var isVisible = dist <= _grassSettings.MaxGrassDistance;

            if (isVisible != _previousIsVisible) _grassMesh.SetGrassInstancesIsVisible(isVisible);
            _previousIsVisible = isVisible;
        }
    }
}
