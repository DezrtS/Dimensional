using System;
using FMOD.Studio;
using FMODUnity;
using Managers;
using Systems.Movement;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class WalkSmoke : MonoBehaviour
    {
        [SerializeField] private VisualEffectPlayer visualEffectPlayer;
        [SerializeField] private float minDelay;
        [SerializeField] private float minSpeed;

        [SerializeField] private EventReference walkSound;

        private ForceController _forceController;
        private float _timeAtLastPlay;
        private EventInstance _walkSoundInstance;

        private void Awake()
        {
            _forceController = GetComponent<ForceController>();
        }

        private void Start()
        {
            _walkSoundInstance = AudioManager.CreateEventInstance(walkSound);
            AudioManager.AttachInstanceToGameObject(_walkSoundInstance, gameObject);
        }

        private bool CanEmit(float time)
        {
            if (_forceController.GetVelocity().sqrMagnitude < minSpeed) return false;
            return (time - _timeAtLastPlay >= minDelay); 
        }

        public void Play()
        {
            var time = Time.timeSinceLevelLoad;
            if (!CanEmit(time)) return;
            visualEffectPlayer.PlayContinuous();
            _timeAtLastPlay = time;
        }

        public void PlayWithSound()
        {
            var time = Time.timeSinceLevelLoad;
            if (!CanEmit(time)) return;
            visualEffectPlayer.PlayContinuous();
            _timeAtLastPlay = time;
            _walkSoundInstance.start();
        }
    }
}
