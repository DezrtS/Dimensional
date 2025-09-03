using System;
using Interfaces;
using Scriptables.Actions;
using UnityEngine;

namespace Systems.Actions
{
    public enum ActionType
    {
        
    }

    public class ActionContext
    {
        public IActivateActions ActionActivator;
        
        public GameObject SourceGameObject;
        public ScriptableObject SourceData;

        public Vector3 TargetDirection;

        public static ActionContext Construct(IActivateActions actionActivator, GameObject sourceGameObject, ScriptableObject sourceData, Vector3 targetDirection)
        {
            var context = new ActionContext()
            {
                ActionActivator = actionActivator,
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

        private IActivateActions _actionActivator;
        private float _activationTimer;
        
        public ActionDatum ActionDatum { get; private set; }
        public bool IsActive { get; private set; }
        protected bool IsTriggering { get; private set; }
        protected ActionContext PreviousContext { get; private set; }

        public virtual void Initialize(ActionDatum actionDatum)
        {
            ActionDatum = actionDatum;
            IsActive = false;
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

        protected void HandleActivation(ActionContext context)
        {
            IsActive = true;
            _actionActivator = context.ActionActivator;
            _activationTimer = ActionDatum.ActivationTime;
            PreviousContext = context;
            Activated?.Invoke(this, context);

            if (_activationTimer == 0) Trigger(context);
        }

        private void Trigger(ActionContext context)
        {
            if (!IsActive) return;
            OnTrigger(context);
        }

        protected virtual void OnTrigger(ActionContext context)
        {
            HandleTrigger(context);
        }

        protected void HandleTrigger(ActionContext context)
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

        protected void HandleDeactivation(ActionContext context)
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
        
        protected void HandleInterruption(ActionContext context)
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

        protected void HandleCancellation(ActionContext context)
        {
            IsActive = false;
            IsTriggering = false;
            _actionActivator = context.ActionActivator;
            _activationTimer = 0;
            Cancelled?.Invoke(this, context);
        }
    }
}
