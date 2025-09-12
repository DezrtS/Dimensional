using Interfaces;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;

namespace Scriptables.Actions
{
    
    public abstract class ActionDatum : ScriptableObject
    {
        [SerializeField] private float activationTime;
        [SerializeField] private ActionVisualEffectDatum[] actionVisualEffectData;
        public float ActivationTime => activationTime;
        public ActionVisualEffectDatum[] ActionVisualEffectData => actionVisualEffectData;

        public abstract Action AttachAction(GameObject actionHolder);
    }
}