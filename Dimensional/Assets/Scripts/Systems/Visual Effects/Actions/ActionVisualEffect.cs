using System.Collections.Generic;
using System.Linq;
using Scriptables.Actions;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;
using Utilities;

namespace Systems.Visual_Effects
{
    public class ActionVisualEffect : MonoBehaviour
    {
        private Action _action;

        private Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>> _actionEffectPlayersDictionary;
        
        protected ActionContext PreviousContext { get; private set; }

        public virtual void Initialize(ActionVisualEffectDatum actionVisualActionDatum)
        {
            //_actionVisualActionDatum = actionVisualActionDatum;
            _actionEffectPlayersDictionary = new Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>>();
            foreach (var actionParticleEffect in actionVisualActionDatum.ActionParticleEffects)
            {
                if (!_actionEffectPlayersDictionary.ContainsKey(actionParticleEffect.ActionEventType))
                {
                    _actionEffectPlayersDictionary.Add(actionParticleEffect.ActionEventType, new List<ObjectPool<EffectPlayer>>());
                }
                
                _actionEffectPlayersDictionary[actionParticleEffect.ActionEventType].Add(new ObjectPool<EffectPlayer>(actionParticleEffect.ObjectPoolDatum, actionParticleEffect.EffectPlayerPrefab, actionParticleEffect.AttachPoolToTransform ? transform : null));
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
        }

        protected virtual void ActionOnTriggered(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Triggered, context);
        }

        protected virtual void ActionOnDeactivated(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Deactivated, context);
        }

        protected virtual void ActionOnInterrupted(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Interrupted, context);
        }

        protected virtual void ActionOnCancelled(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Cancelled, context);
        }

        protected virtual void ActionOnEntityChanged(Action action, ActionContext context) { }

        private void PlayParticleEffects(ActionEventType actionEventType, ActionContext context)
        {
            if (!_actionEffectPlayersDictionary.ContainsKey(actionEventType)) return;
            
            for (var i = 0; i < _actionEffectPlayersDictionary[actionEventType].Count; i++)
            {
                var effectPlayerObjectPool = _actionEffectPlayersDictionary[actionEventType][i];
                var effectPlayer = effectPlayerObjectPool.GetObject();
                effectPlayer.transform.position = transform.position;
                effectPlayer.transform.forward = context.TargetDirection;
                effectPlayer.Play(true);
            }
        }

        public virtual void Destroy()
        {
            foreach (var actionEffectPlayers in _actionEffectPlayersDictionary.Select(keyValuePair => keyValuePair.Value))
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
