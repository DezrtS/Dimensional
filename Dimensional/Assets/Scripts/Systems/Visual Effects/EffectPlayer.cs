using System;
using Interfaces;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public abstract class EffectPlayer : MonoBehaviour, IObjectPoolable<EffectPlayer>
    {
        public event Action<EffectPlayer> Returned;
        public event Action<EffectPlayer> Finished;

        [SerializeField] private bool hasDuration;
        [SerializeField] private float duration;

        private float _timer;
        private bool _returnToPoolOnFinished;
        
        public void ReturnToPool() => Returned?.Invoke(this);

        private void FixedUpdate()
        {
            if (!hasDuration && _timer > 0) return;
            
            var fixedDeltaTime = Time.fixedDeltaTime;
            _timer -= fixedDeltaTime;
            
            if (_timer > 0) return;
            Finished?.Invoke(this);
            if (_returnToPoolOnFinished) ReturnToPool();
        }

        public virtual void Play(bool returnToPoolOnFinished = false)
        {
            _returnToPoolOnFinished = returnToPoolOnFinished;
            _timer = duration;
        }

        public virtual void Stop()
        {
            _timer = 0;
        }
    }
}
