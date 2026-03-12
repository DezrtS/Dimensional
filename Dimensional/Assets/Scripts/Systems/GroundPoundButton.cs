using System;
using Systems.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Systems
{
    public class GroundPoundButton : MonoBehaviour
    {
        [SerializeField] private SignalAsset signalAsset;
        [SerializeField] private SignalReceiver[] signalReceivers;

        private Health _health;
        private bool _isActive;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.HealthChanged += HealthOnHealthChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.HealthChanged -= HealthOnHealthChanged;
        }

        private void HealthOnHealthChanged(int oldValue, int newValue, int maxValue)
        {
            if (newValue < oldValue)
                Activate();
        }

        private void Activate()
        {
            if (_isActive) return;
            _isActive = true;

            var emitter = ScriptableObject.CreateInstance<SignalEmitter>();
            emitter.asset = signalAsset;

            foreach (var receiver in signalReceivers)
            {
                receiver.OnNotify(Playable.Null, emitter, null);
            }

            Destroy(emitter);
        }
    }
}