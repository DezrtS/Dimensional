using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;
using Utilities;
using Action = Systems.Actions.Action;

namespace Systems.Visual_Effects
{
    public class ActionVisualEffect : MonoBehaviour
    {
        private Action _action;

        private Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>> _effectStartEventDictionary;
        private List<ObjectPool<EffectPlayer>> _effectEndEventList;
        private Dictionary<ActionEventType, List<ActionAnimation>> _actionAnimationDictionary;
        private List<string> _animationEndList;

        protected ActionContext PreviousContext { get; private set; }
        protected Animator Animator { get; private set; }

        public virtual void Initialize(ActionVisualEffectDatum actionVisualActionDatum)
        {
            _effectStartEventDictionary = new Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>>();
            _effectEndEventList = new List<ObjectPool<EffectPlayer>>();
            _actionAnimationDictionary = new Dictionary<ActionEventType, List<ActionAnimation>>();
            _animationEndList = new List<string>();
            
            foreach (var actionParticleEffect in actionVisualActionDatum.ActionParticleEffects)
            {
                if (actionParticleEffect.EffectStartEventType == ActionEventType.None) continue;
                
                if (!_effectStartEventDictionary.ContainsKey(actionParticleEffect.EffectStartEventType))
                {
                    _effectStartEventDictionary.Add(actionParticleEffect.EffectStartEventType, new List<ObjectPool<EffectPlayer>>());
                }

                var effectPlayerObject = new ObjectPool<EffectPlayer>(actionParticleEffect.ObjectPoolDatum,
                    actionParticleEffect.EffectPlayerPrefab,
                    actionParticleEffect.AttachPoolToTransform ? transform : null);
                _effectStartEventDictionary[actionParticleEffect.EffectStartEventType].Add(effectPlayerObject);
                
                if (actionParticleEffect.StopOnActionEnd) _effectEndEventList.Add(effectPlayerObject);
            }

            foreach (var actionAnimation in actionVisualActionDatum.ActionAnimations)
            {
                AddAnimation(actionAnimation);
            }
        }

        public virtual void SetAction(Action action)
        {
            if (_action)
            {
                _action.Activated -= ActionOnActivated;
                _action.Triggered -= ActionOnTriggered;
                _action.Deactivated -= ActionOnDeactivated;
                _action.Interrupted -= ActionOnInterrupted;
                _action.Cancelled -= ActionOnCancelled;
                _action.EntityChanged -= ActionOnEntityChanged;
            }
            
            //_actionDatum = action.ActionDatum;
            _action = action;
            _action.Activated += ActionOnActivated;
            _action.Triggered += ActionOnTriggered;
            _action.Deactivated += ActionOnDeactivated;
            _action.Interrupted += ActionOnInterrupted;
            _action.Cancelled += ActionOnCancelled;
            _action.EntityChanged += ActionOnEntityChanged;
        }

        protected virtual void ActionOnActivated(Action action, ActionContext context)
        {
            PreviousContext = context;
            PlayParticleEffects(ActionEventType.Activated, context);
            TriggerAnimations(ActionEventType.Activated, context);
        }

        protected virtual void ActionOnTriggered(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Triggered, context);
            TriggerAnimations(ActionEventType.Triggered, context);
        }

        protected virtual void ActionOnDeactivated(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Deactivated, context);
            TriggerAnimations(ActionEventType.Deactivated, context);
            EndParticleEffects();
            EndAnimations();
        }

        protected virtual void ActionOnInterrupted(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Interrupted, context);
            TriggerAnimations(ActionEventType.Interrupted, context);
            EndParticleEffects();
            EndAnimations();
        }

        protected virtual void ActionOnCancelled(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Cancelled, context);
            TriggerAnimations(ActionEventType.Cancelled, context);
            EndParticleEffects();
            EndAnimations();
        }

        protected virtual void ActionOnEntityChanged(Action action, ActionContext context)
        {
            Animator = context.SourceGameObject.GetComponent<Animator>();
        }

        private void PlayParticleEffects(ActionEventType actionEventType, ActionContext context)
        {
            if (!_effectStartEventDictionary.TryGetValue(actionEventType, out var objectPools)) return;

            foreach (var effectPlayer in objectPools.Select(objectPool => objectPool.GetObject()))
            {
                effectPlayer.transform.position = transform.position;
                effectPlayer.transform.forward = context.TargetDirection;
                effectPlayer.Play(true);
            }
        }

        private void EndParticleEffects()
        {
            foreach (var effectPlayerObjectPool in _effectEndEventList)
            {
                effectPlayerObjectPool.RecallPool();
            }
        }

        protected void AddAnimation(ActionAnimation actionAnimation)
        {
            _actionAnimationDictionary.TryAdd(actionAnimation.AnimationStartEventType, new List<ActionAnimation>());
            _actionAnimationDictionary[actionAnimation.AnimationStartEventType].Add(actionAnimation);
            if (actionAnimation.AnimationEventType == AnimationEventType.Bool) _animationEndList.Add(actionAnimation.AnimationName);
        }

        private void TriggerAnimations(ActionEventType actionEventType, ActionContext context)
        {
            if (!_actionAnimationDictionary.TryGetValue(actionEventType, out var actionAnimations)) return;
            foreach (var actionAnimation in actionAnimations)
            {
                switch (actionAnimation.AnimationEventType)
                {
                    case AnimationEventType.Bool:
                        Animator.SetBool(actionAnimation.AnimationName, true);
                        break;
                    case AnimationEventType.Trigger:
                        Animator.SetTrigger(actionAnimation.AnimationName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void EndAnimations()
        {
            foreach (var animationName in _animationEndList)
            {
                Animator.SetBool(animationName, false);
            }
        }

        public virtual void Destroy()
        {
            foreach (var actionEffectPlayers in _effectStartEventDictionary.Select(keyValuePair => keyValuePair.Value))
            {
                for (var i = actionEffectPlayers.Count - 1; i >= 0; i--)
                {
                    var objectPool = actionEffectPlayers[i];
                    objectPool.DestroyObjectPool();
                    actionEffectPlayers.RemoveAt(i);
                }
            }
            
            Destroy(this);
        }
    }
}
