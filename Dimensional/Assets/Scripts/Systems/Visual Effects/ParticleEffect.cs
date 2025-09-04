using System;
using Interfaces;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class ParticleEffect : MonoBehaviour, IObjectPoolable<ParticleEffect>
    {
        public delegate void ParticleEffectEventHandler(ParticleEffect particleEffect, int id);
        public event ParticleEffectEventHandler Finished;
        public event Action<ParticleEffect> Returned;
        
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
        
        public void ReturnToPool() => Returned?.Invoke(this);
    }
}
