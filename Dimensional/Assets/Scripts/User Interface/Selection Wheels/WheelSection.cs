using System;
using Scriptables.Selection_Wheels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Selection_Wheels
{
    public class WheelSection : MonoBehaviour
    {
        private static readonly int Transition = Animator.StringToHash("Transition");
        [SerializeField] private Image sectionBackground;
        [SerializeField] private TextMeshProUGUI sectionText;
        [SerializeField] private Image sectionIcon;
        [Space]
        [SerializeField] private Image[] sectionMasks;
        [Space]
        [SerializeField] private RectTransform scalePivot;
        [Space]
        [SerializeField] private RectTransform sectionRotationPivot;
        [SerializeField] private RectTransform marginRotationPivot;
        [SerializeField] private RectTransform textRotationPivot;
        [SerializeField] private RectTransform iconRotationPivot;
        [Space]
        [SerializeField] private RectTransform textPivot;
        [SerializeField] private RectTransform iconPivot;
        
        private WheelSectionDatum _wheelSectionDatum;
        private SelectionWheelSettingsDatum _selectionWheelSettingsDatum;
        private Animator _animator;
        private bool _isActive;
        
        private float _halfAngleSize;
        
        private float _transitionValue;
        
        public Vector2 Direction { get; private set; }
        protected SelectionWheel SelectionWheel { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = _isActive ? Time.fixedDeltaTime : -Time.fixedDeltaTime;
            _transitionValue = Mathf.Clamp01((_transitionValue + fixedDeltaTime) / _selectionWheelSettingsDatum.TransitionDuration);
            //_animator.SetFloat(Transition, _transitionValue);
        }

        public virtual void Initialize(WheelSectionDatum wheelSectionDatum, SelectionWheel selectionWheel)
        {
            _wheelSectionDatum = wheelSectionDatum;
            SelectionWheel = selectionWheel;
            _selectionWheelSettingsDatum = SelectionWheel.SelectionWheelDatum.SelectionWheelSettingsDatum;
            
            sectionBackground.color = _selectionWheelSettingsDatum.DefaultBackgroundColor;
            sectionText.text = _wheelSectionDatum.SectionName;
            sectionText.color = _selectionWheelSettingsDatum.DefaultTextColor;
            sectionIcon.sprite = _wheelSectionDatum.SectionIcon;
            sectionIcon.color = _selectionWheelSettingsDatum.DefaultIconColor;
        }
        
        public bool IsInside(Vector2 inputDir, float hysteresis = 0f)
        {
            if (inputDir == Vector2.zero) return false;
            var angle = Vector2.Angle(Direction, inputDir);
            return angle <= _halfAngleSize + hysteresis;
        }


        public void Format(int index, float intervalAngle, float angleMargin)
        {
            var angleOffset = intervalAngle * index + _selectionWheelSettingsDatum.AngleOffset;
            _halfAngleSize = intervalAngle / 2f; //(intervalAngle - angleMargin) / 2f;
            var sectionRotation = Quaternion.Euler(0, 0, -(angleOffset - _halfAngleSize));
            sectionRotationPivot.localRotation = sectionRotation;

            var margin = angleMargin / 2f;
            var marginRotation = Quaternion.Euler(0, 0, -margin);
            marginRotationPivot.localRotation = marginRotation;
            
            var rotation = Quaternion.Euler(0, 0, -angleOffset);
            var inverseRotation = Quaternion.Inverse(rotation);
            
            iconRotationPivot.localRotation = rotation;
            iconPivot.localRotation = inverseRotation;

            if (_selectionWheelSettingsDatum.RotateText)
            {
                textRotationPivot.localRotation = rotation;
                textPivot.localRotation = inverseRotation;
                
                switch (angleOffset)
                {
                    case 0:
                        textPivot.pivot = new Vector2(0.5f, 0);
                        sectionText.alignment = TextAlignmentOptions.Center;
                        break;
                    case < 90:
                        textPivot.pivot = new Vector2(0, 0);
                        sectionText.alignment = TextAlignmentOptions.Left;
                        break;
                    case 90:
                        textPivot.pivot = new Vector2(0, 0.5f);
                        sectionText.alignment = TextAlignmentOptions.Left;
                        break;
                    case < 180:
                        textPivot.pivot = new Vector2(0, 1);
                        sectionText.alignment = TextAlignmentOptions.Left;
                        break;
                    case 180:
                        textPivot.pivot = new Vector2(0.5f, 1);
                        sectionText.alignment = TextAlignmentOptions.Center;
                        break;
                    case < 270:
                        textPivot.pivot = new Vector2(1, 1);
                        sectionText.alignment = TextAlignmentOptions.Right;
                        break;
                    case 270:
                        textPivot.pivot = new Vector2(1, 0.5f);
                        sectionText.alignment = TextAlignmentOptions.Right;
                        break;
                    case < 360:
                        textPivot.pivot = new Vector2(1, 0);
                        sectionText.alignment = TextAlignmentOptions.Right;
                        break;
                    default:
                        textPivot.pivot = new Vector2(0.5f, 0.5f);
                        sectionText.alignment = TextAlignmentOptions.Center;
                        break;
                }
                
                var position = textRotationPivot.localPosition;
                position.x = 0;
                textRotationPivot.localPosition = position;
            }
            
            Direction = new Vector2(
                Mathf.Sin(angleOffset * Mathf.Deg2Rad),
                Mathf.Cos(angleOffset * Mathf.Deg2Rad)
            );

            foreach (var sectionMask in sectionMasks)
            {
                sectionMask.fillAmount = (1f / 360f) * (intervalAngle - angleMargin);
            }
        }

        [ContextMenu("Hover")]
        public void Hover()
        {
            _isActive = true;
            sectionBackground.color = _selectionWheelSettingsDatum.ActiveBackgroundColor;
            sectionText.color = _selectionWheelSettingsDatum.ActiveTextColor;
            sectionIcon.color = _selectionWheelSettingsDatum.ActiveIconColor;
            OnHover();
        }
        
        protected virtual void OnHover() {}

        [ContextMenu("Stop Hover")]
        public void StopHovering()
        {
            _isActive = false;
            sectionBackground.color = _selectionWheelSettingsDatum.DefaultBackgroundColor;
            sectionText.color = _selectionWheelSettingsDatum.DefaultTextColor;
            sectionIcon.color = _selectionWheelSettingsDatum.DefaultIconColor;
            OnStopHovering();
        }
        
        protected virtual void OnStopHovering() {}

        public virtual void Select()
        {
            _wheelSectionDatum.Select(SelectionWheel, this);
            if (_wheelSectionDatum.CloseOnSelect) SelectionWheel.Deactivate();
        }

        public void SetIsDisabled(bool isDisabled)
        {
            sectionBackground.color = isDisabled ? _selectionWheelSettingsDatum.DisabledBackgroundColor : _selectionWheelSettingsDatum.DefaultBackgroundColor;
            sectionText.color = isDisabled ? _selectionWheelSettingsDatum.DisabledTextColor : _selectionWheelSettingsDatum.DefaultTextColor;
            sectionIcon.color = isDisabled ? _selectionWheelSettingsDatum.DisabledIconColor : _selectionWheelSettingsDatum.DefaultIconColor;
        }
    }
}
