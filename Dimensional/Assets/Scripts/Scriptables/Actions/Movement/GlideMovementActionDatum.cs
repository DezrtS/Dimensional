using Systems.Actions;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "GlideMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/GlideMovementActionDatum")]
    public class GlideMovementActionDatum : MovementActionDatum
    {
        [SerializeField] private float glideFallSpeedThreshold;
        [SerializeField] private float glideFallTimeThreshold;
        [Space(10)] [SerializeField] private float glideFallSpeed;
        [SerializeField] private float glideMass;
        
        public float GlideFallSpeedThreshold => glideFallSpeedThreshold;
        public float GlideFallTimeThreshold => glideFallTimeThreshold;
        
        public float GlideFallSpeed => glideFallSpeed;
        public float GlideMass => glideMass;
        
        public override Action AttachAction(GameObject actionHolder)
        {
            var glideMovementAction = actionHolder.AddComponent<GlideMovementAction>();
            glideMovementAction.Initialize(this);
            return glideMovementAction;
        }
    }
}
