using System;
using Scriptables.Utilities;
using Systems.Actions;
using Systems.Visual_Effects;
using UnityEngine;

namespace Scriptables.Visual_Effects
{
    [Serializable]
    public struct ActionEffectPlayer
    {
        [SerializeField] private GameObject effectPlayerPrefab;
        [SerializeField] private bool attachPoolToTransform;
        [SerializeField] private ObjectPoolDatum objectPoolDatum;
        [SerializeField] private ActionEventType effectStartEventType;
        [SerializeField] private bool stopOnActionEnd;
        
        public GameObject EffectPlayerPrefab => effectPlayerPrefab;
        public bool AttachPoolToTransform => attachPoolToTransform;
        public ObjectPoolDatum ObjectPoolDatum => objectPoolDatum;
        public ActionEventType EffectStartEventType => effectStartEventType;
        public bool StopOnActionEnd => stopOnActionEnd;
    }

    public enum AnimationEventType
    {
        Trigger,
        Bool
    }

    [Serializable]
    public struct ActionAnimation
    {
        [SerializeField] private string animationName;
        [SerializeField] private AnimationEventType animationEventType;
        [SerializeField] private ActionEventType animationStartEventType;
        
        public string AnimationName => animationName;
        public AnimationEventType AnimationEventType => animationEventType;
        public ActionEventType AnimationStartEventType => animationStartEventType;

        public ActionAnimation(string animationName, AnimationEventType animationEventType,
            ActionEventType animationStartEventType)
        {
            this.animationName = animationName;
            this.animationEventType = animationEventType;
            this.animationStartEventType = animationStartEventType;
        }
    }
    
    [CreateAssetMenu(fileName = "ActionVisualEffectDatum", menuName = "Scriptable Objects/Visual Effects/Actions/ActionVisualEffectDatum")]
    public class ActionVisualEffectDatum : ScriptableObject
    {
        [SerializeField] private GameObject actionVisualEffectPrefab;
        [SerializeField] private ActionEffectPlayer[] actionParticleEffects;
        [SerializeField] private ActionAnimation[] actionAnimations;
        
        protected GameObject ActionVisualEffectPrefab => actionVisualEffectPrefab;
        public ActionEffectPlayer[] ActionParticleEffects => actionParticleEffects;
        public ActionAnimation[] ActionAnimations => actionAnimations;

        public virtual ActionVisualEffect AttachActionVisualEffect(Transform parent)
        {
            var actionVisualEffectObject = Instantiate(actionVisualEffectPrefab, parent);
            var actionVisualEffect = actionVisualEffectObject.GetComponent<ActionVisualEffect>();
            actionVisualEffect.Initialize(this);
            return actionVisualEffect;
        }
    }
}
