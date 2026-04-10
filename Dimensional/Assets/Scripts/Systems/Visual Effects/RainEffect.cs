using System;
using FMOD.Studio;
using FMODUnity;
using Managers;
using Scriptables.Save;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Systems.Visual_Effects
{
    public class RainEffect : MonoBehaviour
    {
        [SerializeField] private BoolVariable boolVariable;
        
        [SerializeField] private EventReference eventReference;
        
        [SerializeField] private Vector3 offset = new Vector3(0, 35, 0);
        [SerializeField] private ParticleSystem rainParticleSystem;
        
        private Transform _targetTransform;
        private EventInstance _eventInstance;

        public void StartRain()
        {
            rainParticleSystem.Play();
            _eventInstance.start();
        }

        public void StopRain()
        {
            rainParticleSystem.Stop();
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        private void Start()
        {
            _eventInstance = AudioManager.CreateEventInstance(eventReference);
            _targetTransform = CameraManager.Instance.Camera.transform;
            if (boolVariable && boolVariable.Value) StartRain(); 
        }

        private void FixedUpdate()
        {
            transform.position = _targetTransform.position + offset;
        }
    }
}
