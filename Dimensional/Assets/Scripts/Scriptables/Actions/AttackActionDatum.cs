using Scriptables.Movement;
using Systems.Actions;
using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Scriptables.Actions
{
    [CreateAssetMenu(fileName = "AttackActionDatum", menuName = "Scriptable Objects/Actions/AttackActionDatum")]
    public class AttackActionDatum : ActionDatum
    {
        [Space(10)] 
        [SerializeField] private ActionEventType attackEventType;
        [SerializeField] private bool endActionOnHit;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackRadius;
        [SerializeField] private Vector3 attackOffset;
        [SerializeField] private LayerMask attackLayerMask;
        [Space(10)] 
        [SerializeField] private bool doScreenShakeOnHit;
        [SerializeField] private bool hasHitStop;
        [SerializeField] private float hitStopDuration;
        [Space(10)]
        [SerializeField] private bool hasKnockback;
        [SerializeField] private ForceEventDatum forceEventDatum;
        [SerializeField] private KnockbackType knockbackType;
        
        public ActionEventType AttackEventType => attackEventType;
        public bool EndActionOnHit => endActionOnHit;
        public int AttackDamage => attackDamage;
        public float AttackRadius => attackRadius;
        public Vector3 AttackOffset => attackOffset;
        public LayerMask AttackLayerMask => attackLayerMask;
        
        public bool DoScreenShakeOnHit => doScreenShakeOnHit;
        public bool HasHitStop => hasHitStop;
        public float HitStopDuration => hitStopDuration;
        
        public bool HasKnockback => hasKnockback;
        public ForceEventDatum ForceEventDatum => forceEventDatum;
        public KnockbackType KnockbackType => knockbackType;
        
        public override Action AttachAction(GameObject actionHolder)
        {
            var attackAction = actionHolder.AddComponent<AttackAction>();
            attackAction.Initialize(this);
            return attackAction;
        }
    }
}
