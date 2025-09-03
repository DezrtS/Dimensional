using System;
using Scriptables.Shapes;
using UnityEngine;
using Scriptables.Actions;
using Systems.Movement;

namespace Systems.Shapes
{
    public abstract class ShapeController : MonoBehaviour
    {
        /*
        [SerializeField] private ShapeDatum shapeDatum;

        protected ShapeDatum ShapeDatum => shapeDatum;
        protected TestPlayerMovementComponent PlayerMovementController { get; private set; }

        private void Awake()
        {
            PlayerMovementController = GetComponent<TestPlayerMovementComponent>();
            PlayerMovementController.ActionTriggered += OnPlayerMovementControllerActionTriggered;
            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void OnPlayerMovementControllerActionTriggered(Scriptables.Shapes.Type shapeType, ActionContext actionContext)
        {
            if (shapeDatum.ShapeType != shapeType) return;
            if (!shapeDatum.HasActionType(actionContext.Type)) return;
            OnAction(actionContext);
        }

        protected abstract void OnAction(ActionContext actionContext);
        */
    }
}