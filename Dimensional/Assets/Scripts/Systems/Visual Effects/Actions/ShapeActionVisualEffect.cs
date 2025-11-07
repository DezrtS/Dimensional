using System.Collections.Generic;
using Scriptables.Visual_Effects;
using Systems.Actions;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class ShapeActionVisualEffect : ActionVisualEffect
    {
        private static readonly int Sphere = Animator.StringToHash("Sphere");

        public override void Initialize(ActionVisualEffectDatum actionVisualActionDatum)
        {
            base.Initialize(actionVisualActionDatum);
            var shapeActionVisualEffectDatum = (ShapeActionVisualEffectDatum)actionVisualActionDatum;
            AddAnimation(new ActionAnimation(shapeActionVisualEffectDatum.ShapeType.ToString(),
                AnimationEventType.Trigger, shapeActionVisualEffectDatum.ActivationEventType));
        }

        protected override void ActionOnDeactivated(Action action, ActionContext context)
        {
            base.ActionOnDeactivated(action, context);
            Animator.SetTrigger(Sphere);
        }

        protected override void ActionOnInterrupted(Action action, ActionContext context)
        {
            base.ActionOnInterrupted(action, context);
            Animator.SetTrigger(Sphere);
        }

        protected override void ActionOnCancelled(Action action, ActionContext context)
        {
            base.ActionOnCancelled(action, context);
            Animator.SetTrigger(Sphere);
        }
    }
}
