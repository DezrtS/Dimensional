using System;
using FMODUnity;
using Managers;
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
    public struct ActionScreenShakeEvent
    {
        [SerializeField] private bool hasScreenShake;
        [SerializeField] private ActionEventType activationEventType;
        [SerializeField] private ScreenShakeEventData screenShakeEventData;

        public bool HasScreenShake => hasScreenShake;
        public ActionEventType ActivationEventType => activationEventType;
        public ScreenShakeEventData ScreenShakeEventData => screenShakeEventData;
    }
    
    [CreateAssetMenu(fileName = "ActionDatum", menuName = "Scriptable Objects/Actions/EmptyActionDatum")]
    public class ActionDatum : ScriptableObject
    {
        [SerializeField] private float activationTime;
        [SerializeField] private bool deactivateParentActionsOnDeactivate;
        [SerializeField] private ActionDatum[] subActionData;
        [SerializeField] private ActionAudioEvent[] actionAudioEvents;
        [SerializeField] private ActionContextModifierDatum[] actionContextModifierData;
        [SerializeField] private ActionScreenShakeEvent actionScreenShakeEvent;
        [SerializeField] private ActionVisualEffectDatum[] actionVisualEffectData;
        
        public float ActivationTime => activationTime;
        public bool DeactivateParentActionsOnDeactivate => deactivateParentActionsOnDeactivate;
        public ActionDatum[] SubActionData => subActionData;
        public ActionAudioEvent[] ActionAudioEvents => actionAudioEvents;
        public ActionContextModifierDatum[] ActionContextModifierData => actionContextModifierData;
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