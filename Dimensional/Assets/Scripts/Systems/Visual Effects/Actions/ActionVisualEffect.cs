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

        private Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>> _effectStartEventDictionary;
        private List<ObjectPool<EffectPlayer>> _effectEndEventList;
        
        protected ActionContext PreviousContext { get; private set; }

        public virtual void Initialize(ActionVisualEffectDatum actionVisualActionDatum)
        {
            //_actionVisualActionDatum = actionVisualActionDatum;
            _effectStartEventDictionary = new Dictionary<ActionEventType, List<ObjectPool<EffectPlayer>>>();
            _effectEndEventList = new List<ObjectPool<EffectPlayer>>();
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
            EndParticleEffects();
        }

        protected virtual void ActionOnInterrupted(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Interrupted, context);
            EndParticleEffects();
        }

        protected virtual void ActionOnCancelled(Action action, ActionContext context)
        {
            PlayParticleEffects(ActionEventType.Cancelled, context);
            EndParticleEffects();
        }

        protected virtual void ActionOnEntityChanged(Action action, ActionContext context) { }

        private void PlayParticleEffects(ActionEventType actionEventType, ActionContext context)
        {
            if (!_effectStartEventDictionary.ContainsKey(actionEventType)) return;
            
            for (var i = 0; i < _effectStartEventDictionary[actionEventType].Count; i++)
            {
                var effectPlayerObjectPool = _effectStartEventDictionary[actionEventType][i];
                var effectPlayer = effectPlayerObjectPool.GetObject();
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
