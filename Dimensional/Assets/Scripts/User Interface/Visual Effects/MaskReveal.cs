using System;
using UnityEngine;

namespace User_Interface.Visual_Effects
{
    public class MaskReveal : MonoBehaviour
    {
        private static readonly int InvertMask = Shader.PropertyToID("_InvertMask");
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        private static readonly int Transparency = Shader.PropertyToID("_Transparency");
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");
        
        public event Action Finished;
        
        [SerializeField] private bool invertTransition;
        [SerializeField] private bool reverseDirection;
        [Space]
        [SerializeField] private Material material;
        [SerializeField] private float duration;
        [SerializeField] private float rotation;
        [SerializeField] private AnimationCurve progressCurve;
        [SerializeField] private AnimationCurve transparencyCurve;
        [SerializeField] private AnimationCurve rotationCurve;
        
        private float _timer;
        private bool _reverse;

        [ContextMenu("Transition")]
        public void Transition() => Transition(invertTransition, reverseDirection);
        public void Transition(bool invert, bool reverse)
        {
            _timer = duration;
            _reverse = reverse;
            material.SetFloat(InvertMask, invert ? 1f : 0f);
        }

        private void Update()
        {
            if (_timer <= 0) return;
            
            _timer -= Time.deltaTime;
            var ratio = _timer / duration;
            if (!_reverse) ratio = 1f - ratio;
            material.SetFloat(Progress, progressCurve.Evaluate(ratio));
            material.SetFloat(Transparency, transparencyCurve.Evaluate(ratio));
            material.SetFloat(Rotation, rotationCurve.Evaluate(ratio) * rotation);
            
            if (!(_timer <= 0)) return;
            Finished?.Invoke();
            _timer = 0;
        }
    }
}
