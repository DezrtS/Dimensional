using System;
using Managers;
using Scriptables.Cutscenes;
using Systems.Cutscenes;
using Systems.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Systems
{
    public class GroundPoundButton : MonoBehaviour
    {
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        [SerializeField] private SignalAsset signalAsset;
        [SerializeField] private SignalReceiver[] signalReceivers;

        [SerializeField] private Health health;
        
        private bool _isActive;

        private void OnEnable()
        {
            health.HealthChanged += HealthOnHealthChanged;
        }

        private void OnDisable()
        {
            health.HealthChanged -= HealthOnHealthChanged;
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

            if (cutscene && cutsceneDatum) CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            if (!signalAsset) return; 
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