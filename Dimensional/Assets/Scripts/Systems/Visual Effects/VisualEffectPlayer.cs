using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Systems.Visual_Effects
{
    public class VisualEffectPlayer : EffectPlayer
    {
        [SerializeField] private VisualEffect[] visualEffects;

        private void Awake()
        {
            if (visualEffects.Length <= 0) visualEffects = GetComponents<VisualEffect>(); 
            foreach (var effect in visualEffects)
            {
                effect.Reinit();
            }
        }

        public override void Play(bool returnToPoolOnFinished)
        {
            base.Play(returnToPoolOnFinished);
            foreach (var effect in visualEffects)
            {
                effect.Reinit();
                effect.SendEvent("OnPlay");   
            }
        }
        
        public void PlayContinuous()
        {
            foreach (var effect in visualEffects)
            {
                effect.SendEvent("OnPlay");   
            }
        }

        public override void Stop()
        {
            base.Stop();
            foreach (var effect in visualEffects)
            {
                effect.Stop();
            }
        }
    }
}
