using System;
using FMODUnity;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;
using Action = Systems.Actions.Action;

namespace Scriptables.Actions
{
    [Serializable]
    public struct ActionAudioEvent
    {
        [SerializeField] private EventReference eventReference;
        [SerializeField] private bool attachToGameObject;
        [SerializeField] private ActionEventType activationEventType;
        [SerializeField] private ActionEventType deactivationEventType;
        
        public EventReference EventReference => eventReference;
        public bool AttachToGameObject => attachToGameObject;
        public ActionEventType ActivationEventType => activationEventType;
        public ActionEventType DeactivationEventType => deactivationEventType;
    }
    
    [CreateAssetMenu(fileName = "ActionDatum", menuName = "Scriptable Objects/Actions/EmptyActionDatum")]
    public class ActionDatum : ScriptableObject
    {
        [SerializeField] private float activationTime;
        [SerializeField] private ActionAudioEvent[] actionAudioEvents;
        [SerializeField] private ActionVisualEffectDatum[] actionVisualEffectData;
        public float ActivationTime => activationTime;
        public ActionAudioEvent[] ActionAudioEvents => actionAudioEvents;
        public ActionVisualEffectDatum[] ActionVisualEffectData => actionVisualEffectData;

        public virtual Action AttachAction(GameObject actionHolder)
        {
            var action = actionHolder.AddComponent<EmptyAction>();
            action.Initialize(this);
            return action;
        }
    }
}