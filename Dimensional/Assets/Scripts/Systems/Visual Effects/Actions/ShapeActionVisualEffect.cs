using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class ShapeActionVisualEffect : ActionVisualEffect
    {
        private static readonly int Sphere = Animator.StringToHash("Sphere");
        private ShapeActionVisualEffectDatum _actionVisualActionDatum;
        private Animator _animator;
        
        public override void Initialize(ActionVisualEffectDatum actionVisualActionDatum)
        {
            base.Initialize(actionVisualActionDatum);
            _actionVisualActionDatum = (ShapeActionVisualEffectDatum)actionVisualActionDatum;
        }

        protected override void ActionOnTriggered(Action action, ActionContext context)
        {
            base.ActionOnTriggered(action, context);
            _animator.SetTrigger(_actionVisualActionDatum.ShapeType.ToString());
        }
        
        protected override void ActionOnDeactivated(Action action, ActionContext context)
        {
            base.ActionOnDeactivated(action, context);
            _animator.SetTrigger(Sphere);
        }

        protected override void ActionOnInterrupted(Action action, ActionContext context)
        {
            base.ActionOnInterrupted(action, context);
            _animator.SetTrigger(Sphere);
        }

        protected override void ActionOnCancelled(Action action, ActionContext context)
        {
            base.ActionOnCancelled(action, context);
            _animator.SetTrigger(Sphere);
        }

        protected override void ActionOnEntityChanged(Action action, ActionContext context)
        {
            _animator = context.SourceGameObject.GetComponent<Animator>();
        }
    }
}
