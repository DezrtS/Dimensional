using Systems.Actions;
using Systems.Actions.Movement;
using Systems.Movement;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "SpringJumpMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/Jumps/SpringJumpMovementActionDatum")]
    public class SpringJumpMovementActionDatum : JumpMovementActionDatum
    {
        [Space(10)]
        [SerializeField] private float minChargeDuration;
        [SerializeField] private float maxChargeDuration;
        [SerializeField] private bool autoJumpOnCharge;

        public float MinChargeDuration => minChargeDuration;
        public float MaxChargeDuration => maxChargeDuration;
        public bool AutoJumpOnCharge => autoJumpOnCharge;

        public override Action AttachAction(GameObject actionHolder)
        {
            var springJumpMovementAction = actionHolder.AddComponent<SpringJumpMovementAction>();
            springJumpMovementAction.Initialize(this);
            return springJumpMovementAction;
        }
    }
}
