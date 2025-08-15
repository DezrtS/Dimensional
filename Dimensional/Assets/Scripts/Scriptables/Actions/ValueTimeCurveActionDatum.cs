using UnityEngine;

namespace Scriptables.Actions
{
    [CreateAssetMenu(fileName = "ValueTimeCurveActionDatum", menuName = "Scriptable Objects/Actions/ValueTimeCurveActionDatum")]
    public class ValueTimeCurveActionDatum : ActionDatum
    {
        [SerializeField] private float value;
        [SerializeField] private float time;
        [SerializeField] private AnimationCurve curve;
        
        public float Value => value;
        public float Time => time;
        public AnimationCurve Curve => curve;
    }
}
