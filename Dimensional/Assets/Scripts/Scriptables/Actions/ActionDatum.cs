using System;
using FMODUnity;
using Scriptables.Actions.Modifiers;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;
using Action = Systems.Actions.Action;

namespace Scriptables.Actions
{
    [Serializable]
    public class ActionAudioEvent
    {
        [SerializeField] private EventReference eventReference;
        [SerializeField] private bool attachToGameObject;
        [SerializeField] private bool createInstance;
        [SerializeField] private ActionEventType activationEventType;
        
        public EventReference EventReference => eventReference;
        public bool AttachToGameObject => attachToGameObject;
        public bool CreateInstance => createInstance;
        public ActionEventType ActivationEventType => activationEventType;
    }

    [Serializable]
    public class ActionScreenShakeEvent
    {
        [SerializeField] private ActionEventType activationEventType;
        [SerializeField] private float duration;
        [SerializeField] private float amplitude;
        [SerializeField] private AnimationCurve amplitudeCurve;
        [SerializeField] private float frequency;
        [SerializeField] private AnimationCurve frequencyCurve;
        
        public ActionEventType ActivationEventType => activationEventType;
        public float Duration => duration;
        public float Amplitude => amplitude;
        public AnimationCurve AmplitudeCurve => amplitudeCurve;
        public float Frequency => frequency;
        public AnimationCurve FrequencyCurve => frequencyCurve;
    }
    
    [CreateAssetMenu(fileName = "ActionDatum", menuName = "Scriptable Objects/Actions/EmptyActionDatum")]
    public class ActionDatum : ScriptableObject
    {
        [SerializeField] private float activationTime;
        [SerializeField] private ActionAudioEvent[] actionAudioEvents;
        [SerializeField] private ActionContextModifierDatum[] actionContextModifierData;
        [SerializeField] private bool hasScreenShake;
        [SerializeField] private ActionScreenShakeEvent actionScreenShakeEvent;
        [SerializeField] private ActionVisualEffectDatum[] actionVisualEffectData;
        
        public float ActivationTime => activationTime;
        public ActionAudioEvent[] ActionAudioEvents => actionAudioEvents;
        public ActionContextModifierDatum[] ActionContextModifierData => actionContextModifierData;
        public bool HasScreenShake => hasScreenShake;
        public ActionScreenShakeEvent ActionScreenShakeEvent => actionScreenShakeEvent;
        public ActionVisualEffectDatum[] ActionVisualEffectData => actionVisualEffectData;

        public virtual Action AttachAction(GameObject actionHolder)
        {
            var action = actionHolder.AddComponent<EmptyAction>();
            action.Initialize(this);
            return action;
        }
    }
}