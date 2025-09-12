using Scriptables.Projectiles;
using Scriptables.Utilities;
using Systems.Actions;
using Systems.Actions.Movement;
using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Scriptables.Actions.Movement
{
    [CreateAssetMenu(fileName = "AttackPositionMovementActionDatum", menuName = "Scriptable Objects/Actions/Movement/AttackPositionMovementActionDatum")]
    public class AttackPositionMovementActionDatum : PositionMovementActionDatum
    {
        [Space(10)] 
        [SerializeField] private ActionEventType attackEventType;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackRadius;
        [SerializeField] private Vector3 attackOffset;
        [SerializeField] private Vector3 attackDirection;
        [SerializeField] private LayerMask attackLayerMask;
        [Space(10)]
        [SerializeField] private bool hasKnockback;
        [SerializeField] private float knockbackPower;
        [SerializeField] private KnockbackType knockbackType;
        [Space(10)]
        [SerializeField] private ActionEventType projectileEventType;
        [SerializeField] private BaseProjectileDatum projectileDatum;
        [SerializeField] private ObjectPoolDatum projectilePoolDatum;
        [SerializeField] private Vector3 fireDirection;

        public ActionEventType AttackEventType => attackEventType;
        public int AttackDamage => attackDamage;
        public float AttackRadius => attackRadius;
        public Vector3 AttackOffset => attackOffset;
        public Vector3 AttackDirection => attackDirection;
        public LayerMask AttackLayerMask => attackLayerMask;
        
        public bool HasKnockback => hasKnockback;
        public float KnockbackPower => knockbackPower;
        public KnockbackType KnockbackType => knockbackType;
        
        public ActionEventType ProjectileEventType => projectileEventType;
        public BaseProjectileDatum ProjectileDatum => projectileDatum;
        public ObjectPoolDatum ProjectilePoolDatum => projectilePoolDatum;
        public Vector3 FireDirection => fireDirection;

        public override Action AttachAction(GameObject actionHolder)
        {
            var attackPositionMovementAction = actionHolder.AddComponent<AttackPositionMovementAction>();
            attackPositionMovementAction.Initialize(this);
            return attackPositionMovementAction;
        }
    }
}
