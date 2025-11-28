using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "AttackPattern", menuName = "Scriptable Objects/AttackPattern")]
    public class AttackPattern : ScriptableObject
    {
        [SerializeField] private AttackPositionMovementActionDatum attackPositionMovementActionDatum;

        [SerializeField] private float windUpDuration;
        [SerializeField] private float cooldownDuration;
        
        [SerializeField] private bool canAttackOutsideRange;
        [SerializeField] private float minRange;
        [SerializeField] private float maxRange;

        public AttackPositionMovementActionDatum AttackPositionMovementActionDatum => attackPositionMovementActionDatum;
        
        public float WindUpDuration => windUpDuration;
        public float CooldownDuration => cooldownDuration;
        
        public bool CanAttackOutsideRange => canAttackOutsideRange;
        public float MinRange => minRange;
        public float MaxRange => maxRange;
    }
}
