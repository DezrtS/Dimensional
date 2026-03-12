using System;
using Managers;
using Scriptables.Save;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class RainEffect : MonoBehaviour
    {
        [SerializeField] private BoolVariable boolVariable;
        
        [SerializeField] private Vector3 offset = new Vector3(0, 35, 0);
        [SerializeField] private ParticleSystem rainParticleSystem;
        
        private Transform _targetTransform;

        public void StartRain()
        {
            rainParticleSystem.Play();
        }

        public void StopRain()
        {
            rainParticleSystem.Stop();
        }

        private void Start()
        {
            _targetTransform = CameraManager.Instance.Camera.transform;
            if (boolVariable && boolVariable.Value) StartRain(); 
        }

        private void FixedUpdate()
        {
            transform.position = _targetTransform.position + offset;
        }
    }
}
