using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "ForceEventDatum", menuName = "Scriptable Objects/Movement/ForceEventDatum")]
    public class ForceEventDatum : ScriptableObject
    {
        [SerializeField] private float duration;
        [Space]
        [SerializeField] private float xVelocity;
        [SerializeField] private AnimationCurve xCurve;
        [SerializeField] private float yVelocity;
        [SerializeField] private AnimationCurve yCurve;
        [SerializeField] private float zVelocity;
        [SerializeField] private AnimationCurve zCurve;
        
        public float Duration => duration;

        public Vector3 GetVelocity(float elapsedTime)
        {
            var ratioTime = elapsedTime / duration;
            var velocity = new Vector3(
                xVelocity * xCurve.Evaluate(ratioTime),
                yVelocity * yCurve.Evaluate(ratioTime),
                zVelocity * zCurve.Evaluate(ratioTime)
            );
            return velocity;
        }
    }
}
