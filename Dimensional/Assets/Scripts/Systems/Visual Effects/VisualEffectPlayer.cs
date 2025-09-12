using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Systems.Visual_Effects
{
    public class VisualEffectPlayer : EffectPlayer
    {
        [SerializeField] private VisualEffect visualEffect;

        private void Awake()
        {
            if (!visualEffect) visualEffect = GetComponent<VisualEffect>(); 
        }

        public override void Play(bool returnToPoolOnFinished = false)
        {
            base.Play(returnToPoolOnFinished);
            visualEffect.Reinit();
            visualEffect.SendEvent("OnPlay");
        }

        public override void Stop()
        {
            base.Stop();
            visualEffect.Stop();
        }
    }
}
