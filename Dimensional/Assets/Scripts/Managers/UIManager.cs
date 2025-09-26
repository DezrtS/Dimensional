using Scriptables.Interactables;
using Scriptables.Selection_Wheels;
using Systems.Actions.Movement;
using UnityEngine;
using User_Interface.Selection_Wheels;
using User_Interface.Visual_Effects;
using Utilities;
using Action = System.Action;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public static event Action TransitionFinished;

        [SerializeField] private GameObject controls;
        [SerializeField] private bool transitionOnAwake;
        [Space]
        [SerializeField] private Transform interactableIconTransform;
        [Space]
        [SerializeField] private GameObject fade;
        [SerializeField] private GameObject selectionWheelTransform;
        [SerializeField] private SelectionWheelDatum actionSelectionWheelDatum;
        
        private MaskReveal _maskReveal;
        private SelectionWheel _actionSelectionWheel;
        private SelectionWheel _shapeSelectionWheel;
        
        public MovementActionType SelectedMovementActionType { get; private set; }

        private void Awake()
        {
            _maskReveal = GetComponent<MaskReveal>();
            _maskReveal.Finished += MaskRevealOnFinished;
            
            if (transitionOnAwake) Transition(false);

            if (!actionSelectionWheelDatum) return;
            _actionSelectionWheel = actionSelectionWheelDatum.AttachSelectionWheel(selectionWheelTransform);
            _actionSelectionWheel.GenerateSelectionWheel();
        }

        public void SpawnInteractableIcon(InteractableIconDatum interactableIconDatum, Transform interactableTransform)
        {
            interactableIconDatum.Spawn(interactableIconTransform, interactableTransform);
        }

        public void Transition(bool reverse, float duration = -1)
        {
            _maskReveal.Transition(true, reverse, duration);
        }
        
        public void EnableFade() => fade.SetActive(true);
        public void DisableFade() => fade.SetActive(false);

        public void ActivateActionSelectionWheel() => _actionSelectionWheel.ActivateSelectionWheel();

        public void ActivateShapeSelectionWheel(MovementActionType movementActionType, SelectionWheelDatum selectionWheelDatum)
        {
            if (_shapeSelectionWheel) Destroy(_shapeSelectionWheel);
            SelectedMovementActionType = movementActionType;
            _shapeSelectionWheel = selectionWheelDatum.AttachSelectionWheel(selectionWheelTransform);
            _shapeSelectionWheel.GenerateSelectionWheel();
            _shapeSelectionWheel.ActivateSelectionWheel(false);
        }

        private static void MaskRevealOnFinished()
        {
            TransitionFinished?.Invoke();
        }
    }
}
