using UnityEngine;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "SelectionWheelSettingsDatum", menuName = "Scriptable Objects/Selection Wheels/SelectionWheelSettingsDatum")]
    public class SelectionWheelSettingsDatum : ScriptableObject
    {
        [SerializeField] private float radius;
        [SerializeField] private float angleMargin;
        [SerializeField] private float angleOffset;
        [SerializeField] private bool rotateText;
        [SerializeField] private bool useOtherMethod;
        [SerializeField] private float transitionDuration;
        [Space]
        [SerializeField] private float deadZone = 0.2f;
        [SerializeField] private float smoothing = 0.2f;
        [SerializeField] private float hysteresisAngle = 5f;
        [SerializeField] private float debounceTime = 0.1f;
        [Space]
        [SerializeField] private Color defaultBackgroundColor;
        [SerializeField] private Color activeBackgroundColor;
        [SerializeField] private Color disabledBackgroundColor;
        [Space]
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color activeTextColor;
        [SerializeField] private Color disabledTextColor;
        [Space]
        [SerializeField] private Color defaultIconColor;
        [SerializeField] private Color activeIconColor;
        [SerializeField] private Color disabledIconColor;
        
        public float Radius => radius;
        public float AngleMargin => angleMargin;
        public float AngleOffset => angleOffset;
        public bool RotateText => rotateText;
        public bool UseOtherMethod => useOtherMethod;
        public float TransitionDuration => transitionDuration;
        
        public float DeadZone => deadZone;
        public float Smoothing => smoothing;
        public float HysteresisAngle => hysteresisAngle;
        public float DebounceTime => debounceTime;
        
        public Color DefaultBackgroundColor => defaultBackgroundColor;
        public Color ActiveBackgroundColor => activeBackgroundColor;
        public Color DisabledBackgroundColor => disabledBackgroundColor;
        
        public Color DefaultTextColor => defaultTextColor;
        public Color ActiveTextColor => activeTextColor;
        public Color DisabledTextColor => disabledTextColor;
        
        public Color DefaultIconColor => defaultIconColor;
        public Color ActiveIconColor => activeIconColor;
        public Color DisabledIconColor => disabledIconColor;
    }
}
