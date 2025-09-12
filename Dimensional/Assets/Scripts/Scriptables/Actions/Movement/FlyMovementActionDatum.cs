using Systems.Actions;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "FlyMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/FlyMovementActionDatum")]
    public class FlyMovementActionDatum : MovementActionDatum
    {
        [SerializeField] private float minVelocityMagnitude;
        [SerializeField] private float maxVelocityMagnitude;
        [Space(10)]
        [SerializeField] private float flyFallSpeed;
        [SerializeField] private AnimationCurve flyFallCurve;
        [SerializeField] private float flyMass;
        
        public float MinVelocityMagnitude => minVelocityMagnitude;
        public float MaxVelocityMagnitude => maxVelocityMagnitude;
        
        public float FlyFallSpeed => flyFallSpeed;
        public AnimationCurve FlyFallCurve => flyFallCurve;
        public float FlyMass => flyMass;
        
        public override Action AttachAction(GameObject actionHolder)
        {
            var flyMovementAction = actionHolder.AddComponent<FlyMovementAction>();
            flyMovementAction.Initialize(this);
            return flyMovementAction;
        }
    }
}
