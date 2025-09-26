using Managers;
using Scriptables.Selection_Wheels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace User_Interface.Selection_Wheels
{
    public class SelectionWheel : MonoBehaviour
    {
        private GameObject _selectionWheelHolder;
        private WheelSection[] _wheelSections;

        private InputActionMap _inputActionMap;
        private InputAction _directionInputAction;

        private WheelSection _selectedWheelSection;

        public SelectionWheelDatum SelectionWheelDatum { get; private set; }
        private bool IsActive { get; set; }
        

        private Vector2 _smoothedInput;
        private float _lastSwitchTime;

        public void Initialize(SelectionWheelDatum selectionWheelDatum)
        {
            SelectionWheelDatum = selectionWheelDatum;
        }

        private void AssignControls()
        {
            _directionInputAction = _inputActionMap.FindAction("Direction");

            var selectInputAction = _inputActionMap.FindAction("Select");
            selectInputAction.performed += OnSelect;

            var cancelInputAction = _inputActionMap.FindAction("Cancel");
            cancelInputAction.performed += OnCancel;
        }

        private void UnassignControls()
        {
            var selectInputAction = _inputActionMap.FindAction("Select");
            selectInputAction.performed -= OnSelect;

            var cancelInputAction = _inputActionMap.FindAction("Cancel");
            cancelInputAction.performed -= OnCancel;
        }

        private void FixedUpdate()
        {
            if (!IsActive) return;

            var rawInput = _directionInputAction.ReadValue<Vector2>();

            // Deadzone
            if (rawInput.sqrMagnitude < SelectionWheelDatum.SelectionWheelSettingsDatum.DeadZone * SelectionWheelDatum.SelectionWheelSettingsDatum.DeadZone) return;

            rawInput.Normalize();

            // Smooth input direction
            _smoothedInput = Vector2.Lerp(_smoothedInput, rawInput, SelectionWheelDatum.SelectionWheelSettingsDatum.Smoothing);

            var wheelSection = CheckSelection(_smoothedInput);
            if (wheelSection == _selectedWheelSection) return;

            // Debounce: require time gap before switching
            if (Time.time - _lastSwitchTime < SelectionWheelDatum.SelectionWheelSettingsDatum.DebounceTime) return;

            if (_selectedWheelSection) _selectedWheelSection.OnHoverEnd();
            _selectedWheelSection = wheelSection;
            _selectedWheelSection.OnHover();

            _lastSwitchTime = Time.time;
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            if (!IsActive) return;
            if (_selectedWheelSection) _selectedWheelSection.Select();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (!IsActive) return;
            DeactivateSelectionWheel();
        }

        private WheelSection CheckSelection(Vector2 direction)
        {
            // If we have a current selection, check if still inside its angular region
            if (_selectedWheelSection && _selectedWheelSection.IsInside(direction, SelectionWheelDatum.SelectionWheelSettingsDatum.HysteresisAngle))
                return _selectedWheelSection;

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
            AssignControls();

            if (SelectionWheelDatum) Destroy(_selectionWheelHolder);
            _selectionWheelHolder = new GameObject("SelectionWheel");
            _selectionWheelHolder.transform.SetParent(transform);
            _selectionWheelHolder.transform.localPosition = Vector3.zero;
            GenerateWheelSections();
            _selectionWheelHolder.transform.localScale = Vector3.one * Mathf.Min(SelectionWheelDatum.SelectionWheelSettingsDatum.Radius, Screen.height);
            _selectionWheelHolder.SetActive(IsActive);
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

        [ContextMenu("Activate Selection Wheel")]
        private void Activate() => ActivateSelectionWheel();

        public void ActivateSelectionWheel(bool switchActionMap = true)
        {
            IsActive = true;
            _selectionWheelHolder.SetActive(IsActive);
            if (switchActionMap) GameManager.Instance.SwitchInputActionMaps("Selection Wheel");
            GameManager.SetTimeScale(0.25f);
            UIManager.Instance.EnableFade();
        }

        [ContextMenu("Deactivate Selection Wheel")]
        private void Deactivate() => DeactivateSelectionWheel();

        public void DeactivateSelectionWheel(bool resetActionMap = true)
        {
            IsActive = false;
            _selectionWheelHolder.SetActive(IsActive);
            if (resetActionMap) GameManager.Instance.ResetInputActionMapToDefault();
            GameManager.SetTimeScale();
            UIManager.Instance.DisableFade();
        }

        private void OnDestroy()
        {
            UnassignControls();
            if (SelectionWheelDatum) Destroy(_selectionWheelHolder);
        }
    }
}
