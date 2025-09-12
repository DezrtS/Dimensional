using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.Actions;
using Systems.Visual_Effects;
using UnityEngine;

namespace Systems.Actions
{
    public enum ActionType
    {
        
    }

    public enum ActionEventType
    {
        None,
        Activated,
        Triggered,
        Deactivated,
        Interrupted,
        Cancelled,
    }

    public class ActionContext
    {
        public IActivateActions ActionActivator;
     
        public IEntity SourceEntity;
        public GameObject SourceGameObject;
        public ScriptableObject SourceData;

        public Vector3 TargetDirection;

        public static ActionContext Construct(IActivateActions actionActivator, IEntity sourceEntity, GameObject sourceGameObject, ScriptableObject sourceData, Vector3 targetDirection)
        {
            var context = new ActionContext()
            {
                ActionActivator = actionActivator,
                SourceEntity = sourceEntity,
                SourceGameObject = sourceGameObject,
                SourceData = sourceData,
                TargetDirection = targetDirection
            };
            
            return context;
        }
    }
    
    public abstract class Action : MonoBehaviour
    {
        public delegate void ActionEventHandler(Action action, ActionContext context);

        public event ActionEventHandler Activated;
        public event ActionEventHandler Triggered;
        public event ActionEventHandler Deactivated;
        public event ActionEventHandler Interrupted;
        public event ActionEventHandler Cancelled;

        public event ActionEventHandler EntityChanged;

        private IActivateActions _actionActivator;
        private float _activationTimer;
        
        private List<ActionVisualEffect> _actionVisualEffects;
        
        public ActionDatum ActionDatum { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsTriggering { get; private set; }
        protected ActionContext PreviousContext { get; private set; }

        public virtual void Initialize(ActionDatum actionDatum)
        {
            ActionDatum = actionDatum;
            IsActive = false;

            _actionVisualEffects = new List<ActionVisualEffect>();
            foreach (var actionVisualEffectDatum in actionDatum.ActionVisualEffectData)
            {
                var actionVisualEffect = actionVisualEffectDatum.AttachActionVisualEffect(transform);
                actionVisualEffect.SetAction(this);
                _actionVisualEffects.Add(actionVisualEffect);
            }
        }

        private void FixedUpdate()
        {
            if (!IsActive) return;
            var fixedDeltaTime = Time.fixedDeltaTime;
            UpdateActivationTimer(fixedDeltaTime);
            OnFixedUpdate(fixedDeltaTime);
        }

        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }

        private void UpdateActivationTimer(float fixedDeltaTime)
        {
            if (_activationTimer <= 0) return;
            
            _activationTimer -= fixedDeltaTime;
            if (_activationTimer <= 0) Trigger(_actionActivator.GetActionContext());
        }

        public void Activate(ActionContext context)
        {
            if (IsActive) return;
            OnActivation(context);
        }
        
        protected virtual void OnActivation(ActionContext context)
        {
            HandleActivation(context);
        }

        protected virtual void HandleActivation(ActionContext context)
        {
            IsActive = true;
            _actionActivator = context.ActionActivator;
            _activationTimer = ActionDatum.ActivationTime;
            UpdateActionContext(context);
            Activated?.Invoke(this, context);

            if (_activationTimer == 0) Trigger(context);
        }

        protected void Trigger(ActionContext context)
        {
            if (!IsActive) return;
            OnTrigger(context);
        }

        protected virtual void OnTrigger(ActionContext context)
        {
            HandleTrigger(context);
        }

        protected virtual void HandleTrigger(ActionContext context)
        {
            IsTriggering = true;
            _actionActivator = context.ActionActivator;
            _activationTimer = 0;
            Triggered?.Invoke(this, context);
        }

        public void Deactivate(ActionContext context)
        {
            if (!IsActive) return;
            if (_activationTimer > 0)
            {
                OnInterruption(context);
                return;
            }
            
            OnDeactivation(context);
        }

        protected virtual void OnDeactivation(ActionContext context)
        {
            HandleDeactivation(context);
        }

        protected virtual void HandleDeactivation(ActionContext context)
        {
            IsActive = false;
            IsTriggering = false;
            _actionActivator = context.ActionActivator;
            _activationTimer = 0;
            Deactivated?.Invoke(this, context);
        }

        public void Interrupt(ActionContext context)
        {
            if (!IsActive) return;
            OnInterruption(context);
        }

        protected virtual void OnInterruption(ActionContext context)
        {
            HandleInterruption(context);
        }
        
        protected virtual void HandleInterruption(ActionContext context)
        {
            IsActive = false;
            IsTriggering = false;
            _actionActivator = context.ActionActivator;
            _activationTimer = 0;
            Interrupted?.Invoke(this, context);
        }

        public void Cancel(ActionContext context)
        {
            if (!IsActive) return;
            OnCancellation(context);
        }

        protected virtual void OnCancellation(ActionContext context)
        {
            HandleCancellation(context);
        }

        protected virtual void HandleCancellation(ActionContext context)
        {
            IsActive = false;
            IsTriggering = false;
            _actionActivator = context.ActionActivator;
            _activationTimer = 0;
            Cancelled?.Invoke(this, context);
        }

        private void UpdateActionContext(ActionContext context)
        {
            var entityChanged = CheckEntityChanged(context);
            if (entityChanged)
            {
                OnEntityChanged(context);
                EntityChanged?.Invoke(this, context);
            }
            
            PreviousContext = context;
        }

        private bool CheckEntityChanged(ActionContext context)
        {
            if (PreviousContext == null) return true;
            
            var previousEntityId = PreviousContext.SourceEntity.Id;
            return previousEntityId != context.SourceEntity.Id;
        }

        protected abstract void OnEntityChanged(ActionContext context);

        public void Destroy()
        {
            for (var i = _actionVisualEffects.Count - 1; i >= 0; i--)
            {
                var actionVisualEffect = _actionVisualEffects[i];
                _actionVisualEffects.RemoveAt(i);
                actionVisualEffect.Destroy();
                Destroy(actionVisualEffect.gameObject);
            }
            
            Destroy(this);
        }
    }
}
