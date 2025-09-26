using UnityEngine;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "SelectionWheelSettingsDatum", menuName = "Scriptable Objects/Selection Wheels/SelectionWheelSettingsDatum")]
    public class SelectionWheelSettingsDatum : ScriptableObject
    {
        [SerializeField] private float radius;
        [SerializeField] private float angleMargin;
        [SerializeField] private float transitionDuration;
        [Space]
        [SerializeField] private float deadZone = 0.2f;
        [SerializeField] private float smoothing = 0.2f;
        [SerializeField] private float hysteresisAngle = 5f;
        [SerializeField] private float debounceTime = 0.1f;
        [Space]
        [SerializeField] private Color defaultBackgroundColor;
        [SerializeField] private Color activeBackgroundColor;
        [Space]
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color activeTextColor;
        [Space]
        [SerializeField] private Color defaultIconColor;
        [SerializeField] private Color activeIconColor;
        
        public float Radius => radius;
        public float AngleMargin => angleMargin;
        public float TransitionDuration => transitionDuration;
        
        public float DeadZone => deadZone;
        public float Smoothing => smoothing;
        public float HysteresisAngle => hysteresisAngle;
        public float DebounceTime => debounceTime;
        
        public Color DefaultBackgroundColor => defaultBackgroundColor;
        public Color ActiveBackgroundColor => activeBackgroundColor;
        
        public Color DefaultTextColor => defaultTextColor;
        public Color ActiveTextColor => activeTextColor;
        
        public Color DefaultIconColor => defaultIconColor;
        public Color ActiveIconColor => activeIconColor;
    }
}
