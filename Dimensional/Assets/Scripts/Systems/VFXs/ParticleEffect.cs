using System;
using UnityEngine;

namespace Systems.VFXs
{
    public class ParticleEffect : MonoBehaviour
    {
        public delegate void ParticleEffectEventHandler(ParticleEffect particleEffect, int id);
        public event ParticleEffectEventHandler Finished;
        
        [SerializeField] private int id;
        [SerializeField] private float duration;
        private ParticleSystem _particleSystem;

        private float _durationTimer;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            if (_durationTimer <= 0) return;
            
            _durationTimer -= Time.fixedDeltaTime;
            if (_durationTimer <= 0) Finished?.Invoke(this, id);
        }

        public void Play()
        {
            _particleSystem.Play();
            _durationTimer = duration;
        }

        public void Stop()
        {
            _particleSystem.Stop();
        }
    }
}
