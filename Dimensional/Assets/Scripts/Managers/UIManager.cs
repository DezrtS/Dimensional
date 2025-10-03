using System;
using Scriptables.Interactables;
using Scriptables.Selection_Wheels;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using UnityEngine;
using User_Interface;
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
            
            if (transitionOnAwake) Transition(true, false);
        }

        private void Start()
        {
            if (!actionSelectionWheelDatum) return;
            _actionSelectionWheel = actionSelectionWheelDatum.AttachSelectionWheel(selectionWheelTransform);
            _actionSelectionWheel.GenerateSelectionWheel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                controls.SetActive(!controls.activeSelf);
            }
        }

        public WorldUIAnchor SpawnWorldUIAnchor(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform)
        {
            return worldUIAnchorDatum.SpawnWorldUIAnchor(interactableIconTransform, worldTransform);
        }

        public void Transition(bool invert, bool reverse, float duration = -1)
        {
            _maskReveal.Transition(invert, reverse, duration);
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
