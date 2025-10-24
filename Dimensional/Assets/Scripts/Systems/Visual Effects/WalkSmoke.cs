using System;
using Systems.Movement;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class WalkSmoke : MonoBehaviour
    {
        [SerializeField] private VisualEffectPlayer visualEffectPlayer;
        [SerializeField] private float minDelay;
        [SerializeField] private float minSpeed;

        private ForceController _forceController;
        private float _timeAtLastPlay;

        private void Awake()
        {
            _forceController = GetComponent<ForceController>();
        }

        public void Play()
        {
            if (_forceController.GetVelocity().sqrMagnitude < minSpeed) return;
            var time = Time.timeSinceLevelLoad;
            if (time - _timeAtLastPlay < minDelay) return; 
            visualEffectPlayer.PlayContinuous();
            _timeAtLastPlay = time;
        }
    }
}
