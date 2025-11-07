using System;
using Managers;
using Scriptables.Selection_Wheels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace User_Interface.Selection_Wheels
{
    public class SelectionWheel : MonoBehaviour
    {
        public event Action<SelectionWheel> Cancelled;
        
        private GameObject _selectionWheelHolder;
        private WheelSection[] _wheelSections;

        private InputActionMap _inputActionMap;
        private InputAction _directionInputAction;

        public SelectionWheelDatum SelectionWheelDatum { get; private set; }
        public bool IsActive { get; private set; }
        public WheelSection SelectedWheelSection { get; private set; }

        private Vector2 _smoothedInput;
        private float _lastSwitchTime;

        public void Initialize(SelectionWheelDatum selectionWheelDatum)
        {
            SelectionWheelDatum = selectionWheelDatum;
        }

        private void AssignControls()
        {
            return;
            _directionInputAction = _inputActionMap.FindAction("Direction");

            if (SelectionWheelDatum.SelectionWheelSettingsDatum.UseOtherMethod)
            {
                var selectActionInputAction = _inputActionMap.FindAction("Select Action");
                selectActionInputAction.performed += OnQuickSelect;
            
                var selectShapeInputAction = _inputActionMap.FindAction("Select Shape");
                selectShapeInputAction.performed += OnQuickSelect;
            }
            else
            {
                var selectInputAction = _inputActionMap.FindAction("Select");
                selectInputAction.performed += OnSelect;

                var cancelInputAction = _inputActionMap.FindAction("Cancel");
                cancelInputAction.performed += OnCancel;
            }
        }

        private void UnassignControls()
        {
            return;
            if (SelectionWheelDatum.SelectionWheelSettingsDatum.UseOtherMethod)
            {
                var selectActionInputAction = _inputActionMap.FindAction("Select Action");
                selectActionInputAction.performed -= OnQuickSelect;
            
                var selectShapeInputAction = _inputActionMap.FindAction("Select Shape");
                selectShapeInputAction.performed -= OnQuickSelect;
            }
            else
            {
                var selectInputAction = _inputActionMap.FindAction("Select");
                selectInputAction.performed -= OnSelect;

                var cancelInputAction = _inputActionMap.FindAction("Cancel");
                cancelInputAction.performed -= OnCancel;
            }
        }

        private void FixedUpdate()
        {
            if (!IsActive || SelectionWheelDatum.SelectionWheelSettingsDatum.UseOtherMethod) return;

            var rawInput = _directionInputAction.ReadValue<Vector2>();

            // Deadzone
            if (rawInput.sqrMagnitude < SelectionWheelDatum.SelectionWheelSettingsDatum.DeadZone * SelectionWheelDatum.SelectionWheelSettingsDatum.DeadZone) return;

            rawInput.Normalize();

            // Smooth input direction
            _smoothedInput = Vector2.Lerp(_smoothedInput, rawInput, SelectionWheelDatum.SelectionWheelSettingsDatum.Smoothing);

            var wheelSection = CheckSelection(_smoothedInput);
            if (wheelSection == SelectedWheelSection) return;

            // Debounce: require time gap before switching
            if (Time.time - _lastSwitchTime < SelectionWheelDatum.SelectionWheelSettingsDatum.DebounceTime) return;

            if (SelectedWheelSection) SelectedWheelSection.StopHovering();
            SelectedWheelSection = wheelSection;
            SelectedWheelSection.Hover();

            _lastSwitchTime = Time.time;
        }
        
        private void OnQuickSelect(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            QuickSelect(input);
        }
        
        public void QuickSelect(Vector2 input)
        {
            if (SelectedWheelSection) SelectedWheelSection.StopHovering();
            SelectedWheelSection = FindClosest(input);
            SelectedWheelSection.Hover();
            Select();
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            if (!IsActive) return;
            Select();
        }

        public void Select()
        {
            if (SelectedWheelSection) SelectedWheelSection.Select();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (!IsActive) return;
            Cancel();
        }

        public void Cancel()
        {
            Cancelled?.Invoke(this);
        }

        private WheelSection CheckSelection(Vector2 direction)
        {
            // If we have a current selection, check if still inside its angular region
            if (SelectedWheelSection && SelectedWheelSection.IsInside(direction, SelectionWheelDatum.SelectionWheelSettingsDatum.HysteresisAngle))
                return SelectedWheelSection;

            // Otherwise, find the closest slice
            return FindClosest(direction);
        }

        private WheelSection FindClosest(Vector2 direction)
        {
            WheelSection newWheelSection = null;
            float angle = float.MaxValue;
            foreach (var wheelSection in _wheelSections)
            {
                var newAngle = Vector2.Angle(direction, wheelSection.Direction);
                if (newAngle < angle)
                {
                    angle = newAngle;
                    newWheelSection = wheelSection;
                }
            }
            return newWheelSection;
        }

        [ContextMenu("Regenerate Selection Wheel")]
        public void GenerateSelectionWheel()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Selection Wheel");

            if (SelectionWheelDatum) Destroy(_selectionWheelHolder);
            _selectionWheelHolder = new GameObject("SelectionWheel");
            _selectionWheelHolder.transform.SetParent(transform);
            _selectionWheelHolder.transform.localPosition = Vector3.zero;
            if (SelectionWheelDatum.HasSelectionWheelPrefab) Instantiate(SelectionWheelDatum.SelectionWheelPrefab, _selectionWheelHolder.transform);
            GenerateWheelSections();
            _selectionWheelHolder.transform.localScale = Vector3.one * Mathf.Min(SelectionWheelDatum.SelectionWheelSettingsDatum.Radius, Screen.height);
            foreach (var wheelSection in _wheelSections)
            {
                wheelSection.SetIsDisabled(true);
            }
            Hide();
        }

        private void GenerateWheelSections()
        {
            var size = SelectionWheelDatum.WheelSectionData.Length;
            var intervalAngle = 360f / size;
            _wheelSections = new WheelSection[size];
            for (var i = 0; i < SelectionWheelDatum.WheelSectionData.Length; i++)
            {
                _wheelSections[i] = SelectionWheelDatum.WheelSectionData[i].AttachWheelSection(_selectionWheelHolder.transform, this);
                _wheelSections[i].Format(i, intervalAngle, SelectionWheelDatum.SelectionWheelSettingsDatum.AngleMargin);
            }
        }
        
        public void Show() => _selectionWheelHolder.SetActive(true);
        public void Hide() => _selectionWheelHolder.SetActive(false);

        [ContextMenu("Activate Selection Wheel")]
        public void Activate()
        {
            if (IsActive) return;
            AssignControls();
            IsActive = true;
            foreach (var wheelSection in _wheelSections)
            {
                if (wheelSection == SelectedWheelSection) continue;
                wheelSection.SetIsDisabled(false);
            }
            //if (_selectedWheelSection) _selectedWheelSection.Hover();
        }

        [ContextMenu("Deactivate Selection Wheel")]
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            UnassignControls();
            //if (_selectedWheelSection) _selectedWheelSection.StopHovering();
            foreach (var wheelSection in _wheelSections)
            {
                if (wheelSection == SelectedWheelSection) continue;
                wheelSection.SetIsDisabled(true);
            }
        }

        private void OnDestroy()
        {
            if (IsActive) UnassignControls();
            if (SelectionWheelDatum) Destroy(_selectionWheelHolder);
        }
    }
}
