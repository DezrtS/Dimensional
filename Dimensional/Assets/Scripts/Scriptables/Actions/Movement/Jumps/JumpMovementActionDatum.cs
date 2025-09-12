using Systems.Actions;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "JumpMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/Jumps/JumpMovementActionDatum")]
    public class JumpMovementActionDatum : PositionMovementActionDatum
    {
        [Space(10)]
        [SerializeField] private float cutJumpMultiplier;
        
        public float CutJumpMultiplier => cutJumpMultiplier;

        public override Action AttachAction(GameObject actionHolder)
        {
            var jumpMovementAction = actionHolder.AddComponent<JumpMovementAction>();
            jumpMovementAction.Initialize(this);
            return jumpMovementAction;
        }
    }
}
