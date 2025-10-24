using UnityEngine;

namespace Scriptables.Entities
{
    [CreateAssetMenu(fileName = "StunBehaviourDatum", menuName = "Scriptable Objects/Entities/Behaviours/StunBehaviourDatum")]
    public class StunBehaviourDatum : ScriptableObject
    {
        [SerializeField] private float recoverDelay;
        
        [SerializeField] private float yVelocity;
        
        [SerializeField] private float zDuration;
        [SerializeField] private float zVelocity;
        [SerializeField] private AnimationCurve zCurve;
        
        public float RecoverDelay => recoverDelay;
        public float YVelocity => yVelocity;
        public float ZDuration => zDuration;
        public float ZVelocity => zVelocity;
        public AnimationCurve ZCurve => zCurve;
    }
}
