using UnityEngine;

namespace Scriptables.Actions
{
    [CreateAssetMenu(fileName = "VectorTimeCurveActionDatum", menuName = "Scriptable Objects/Actions/VectorTimeCurveActionDatum")]
    public class VectorTimeCurveActionDatum : ScriptableObject
    {
        [SerializeField] private Vector3 vector;
        [SerializeField] private float time;
        [SerializeField] private AnimationCurve curve;
        
        public Vector3 Vector => vector;
        public float Time => time;
        public AnimationCurve Curve => curve;
    }
}
