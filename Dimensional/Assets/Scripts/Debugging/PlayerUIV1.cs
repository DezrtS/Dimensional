using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Selection_Wheels;
using Systems.Actions.Movement;
using Systems.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using User_Interface.Selection_Wheels;

namespace Debugging
{
    public class PlayerUIV1 : MonoBehaviour
    {
        [SerializeField] private GameObject actionSelectionWheel1Transform;
        [SerializeField] private GameObject actionSelectionWheel2Transform;
        [SerializeField] private GameObject shapeSelectionWheelTransform;
        [SerializeField] private SelectionWheelDatum actionSelectionWheelDatum1;
        [SerializeField] private SelectionWheelDatum actionSelectionWheelDatum2;
        
        private InputActionMap _inputActionMap;
        
        private SelectionWheel _actionSelectionWheel1;
        private SelectionWheel _actionSelectionWheel2;

        private bool _switched = false;
        
        public GameObject ShapeSelectionWheelTransform => shapeSelectionWheelTransform;

        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Wheel Selection");
            AssignControls();
            
            _actionSelectionWheel1 = actionSelectionWheelDatum1.AttachSelectionWheel(actionSelectionWheel1Transform);
            _actionSelectionWheel1.GenerateSelectionWheel();
            _actionSelectionWheel1.Cancelled += SelectionWheelOnCancelled;
            _actionSelectionWheel1.Show();
            
            _actionSelectionWheel2 = actionSelectionWheelDatum2.AttachSelectionWheel(actionSelectionWheel2Transform);
            _actionSelectionWheel2.GenerateSelectionWheel();
            _actionSelectionWheel2.Cancelled += SelectionWheelOnCancelled;
            
            _actionSelectionWheel1.QuickSelect(Vector2.up);
            PlayerController.Instance.SwitchedWheel += PlayerControllerOnSwitchedWheel;
        }

        private void OnDisable()
        {
            UnassignControls();
        }

        private void AssignControls()
        {
            var actionWheelInputAction = _inputActionMap.FindAction("Action Wheel");
            actionWheelInputAction.performed += OnActionWheel;
            actionWheelInputAction.canceled += OnActionWheel;
            
            var shapeWheelInputAction = _inputActionMap.FindAction("Shape Wheel");
            shapeWheelInputAction.performed += OnShapeWheel;
            shapeWheelInputAction.canceled += OnShapeWheel;
            _inputActionMap.Enable();
        }

        private void UnassignControls()
        {
            var actionWheelInputAction = _inputActionMap.FindAction("Action Wheel");
            actionWheelInputAction.performed -= OnActionWheel;
            actionWheelInputAction.canceled -= OnActionWheel;
            
            var shapeWheelInputAction = _inputActionMap.FindAction("Shape Wheel");
            shapeWheelInputAction.performed -= OnShapeWheel;
            shapeWheelInputAction.canceled -= OnShapeWheel;
            _inputActionMap.Disable();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                actionSelectionWheel1Transform.SetActive(!actionSelectionWheel1Transform.activeSelf);
                actionSelectionWheel2Transform.SetActive(!actionSelectionWheel2Transform.activeSelf);
                ShapeSelectionWheelTransform.SetActive(!ShapeSelectionWheelTransform.activeSelf);
            }
        }

        private void OnActionWheel(InputAction.CallbackContext context)
        {
            if (context.performed) OpenActionShapeSelection();
            else if (context.canceled) CloseActionShapeSelection();
        }
        
        private void OnShapeWheel(InputAction.CallbackContext context)
        {
            if (context.performed) OpenShapeSelection();
            else if (context.canceled) CloseShapeSelection();
        }

        private void PlayerControllerOnSwitchedWheel()
        {
            _switched = !_switched;
            if (_switched)
            {
                _actionSelectionWheel1.Hide();
                _actionSelectionWheel2.Show();
                var actionWheelSection1 = (ActionWheelSection)_actionSelectionWheel1.SelectedWheelSection;
                if (actionWheelSection1) actionWheelSection1.ShapeSelectionWheel.Hide();
                var actionWheelSection2 = (ActionWheelSection)_actionSelectionWheel2.SelectedWheelSection;
                if (actionWheelSection2) actionWheelSection2.ShapeSelectionWheel.Show();
                _actionSelectionWheel2.QuickSelect(Vector2.up);
            }
            else
            {
                _actionSelectionWheel2.Hide();
                _actionSelectionWheel1.Show();
                var actionWheelSection1 = (ActionWheelSection)_actionSelectionWheel1.SelectedWheelSection;
                if (actionWheelSection1) actionWheelSection1.ShapeSelectionWheel.Show();
                var actionWheelSection2 = (ActionWheelSection)_actionSelectionWheel2.SelectedWheelSection;
                if (actionWheelSection2) actionWheelSection2.ShapeSelectionWheel.Hide();
                _actionSelectionWheel1.QuickSelect(Vector2.up);
            }
        }

        public void OpenActionShapeSelection()
        {
            if (_switched)
            {
                if (_actionSelectionWheel2.IsActive) return;
                _actionSelectionWheel2.Activate();
            }
            else
            {
                if (_actionSelectionWheel1.IsActive) return;
                _actionSelectionWheel1.Activate();
            }
            
            GameManager.Instance.SwitchInputActionMaps("Selection Wheel");
            GameManager.SetTimeScale(0.25f);
            UIManager.Instance.EnableFade();
        }

        public void CloseActionShapeSelection()
        {
            if (_actionSelectionWheel1.IsActive)
            {
                _actionSelectionWheel1.Deactivate();   
            }

            if (_actionSelectionWheel2.IsActive)
            {
                _actionSelectionWheel2.Deactivate();
            }
            
            GameManager.Instance.ResetInputActionMapToDefault();
            GameManager.SetTimeScale();
            UIManager.Instance.DisableFade();
        }

        public void OpenShapeSelection()
        {
            if (_switched)
            {
                var actionWheelSection = (ActionWheelSection)_actionSelectionWheel2.SelectedWheelSection;
                if (!actionWheelSection) return;
                actionWheelSection.ShapeSelectionWheel.Activate();
            }
            else
            {
                var actionWheelSection = (ActionWheelSection)_actionSelectionWheel1.SelectedWheelSection;
                if (!actionWheelSection) return;
                actionWheelSection.ShapeSelectionWheel.Activate();
            }
            
            GameManager.Instance.SwitchInputActionMaps("Selection Wheel");
            GameManager.SetTimeScale(0.25f);
            UIManager.Instance.EnableFade();
        }
        
        public void CloseShapeSelection()
        {
            if (_switched)
            {
                _actionSelectionWheel2.Cancel();
            }
            else
            {
                _actionSelectionWheel1.Cancel();
            }
            
            GameManager.Instance.ResetInputActionMapToDefault();
            GameManager.SetTimeScale();
            UIManager.Instance.DisableFade();
        }

        private void SelectionWheelOnCancelled(SelectionWheel selectionWheel)
        {
            CloseActionShapeSelection();
        }
    }
}
