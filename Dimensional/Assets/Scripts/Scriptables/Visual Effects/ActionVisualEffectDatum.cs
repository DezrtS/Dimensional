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
        [SerializeField] private ActionEventType actionEventType;
        
        public GameObject EffectPlayerPrefab => effectPlayerPrefab;
        public bool AttachPoolToTransform => attachPoolToTransform;
        public ObjectPoolDatum ObjectPoolDatum => objectPoolDatum;
        public ActionEventType ActionEventType => actionEventType;
    }
    
    [CreateAssetMenu(fileName = "ActionVisualEffectDatum", menuName = "Scriptable Objects/Visual Effects/Actions/ActionVisualEffectDatum")]
    public class ActionVisualEffectDatum : ScriptableObject
    {
        [SerializeField] private GameObject actionVisualEffectPrefab;
        [SerializeField] private ActionEffectPlayer[] actionParticleEffects;
        
        protected GameObject ActionVisualEffectPrefab => actionVisualEffectPrefab;
        public ActionEffectPlayer[] ActionParticleEffects => actionParticleEffects;

        public virtual ActionVisualEffect AttachActionVisualEffect(Transform parent)
        {
            var actionVisualEffectObject = Instantiate(actionVisualEffectPrefab, parent);
            var actionVisualEffect = actionVisualEffectObject.GetComponent<ActionVisualEffect>();
            actionVisualEffect.Initialize(this);
            return actionVisualEffect;
        }
    }
}
