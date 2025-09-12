using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Scriptables.Projectiles.Behaviours
{
    [CreateAssetMenu(fileName = "DamageProjectileBehaviourDatum", menuName = "Scriptable Objects/Projectiles/Behaviours/DamageProjectileBehaviourDatum")]
    public class DamageProjectileBehaviourDatum : BaseProjectileBehaviourDatum
    {
        [SerializeField] private int damage;
        //[SerializeField] private LayerMask damageLayerMask;

        [SerializeField] private bool useVelocity;
        [SerializeField] private float minVelocity;
        [SerializeField] private float maxVelocity;
        [SerializeField] private AnimationCurve velocityDamageCurve;

        public int Damage => damage;
        //public LayerMask DamageLayerMask => damageLayerMask;
        
        public bool UseVelocity => useVelocity;
        public float MinVelocity => minVelocity;
        public float MaxVelocity => maxVelocity;
        public AnimationCurve VelocityDamageCurve => velocityDamageCurve;

        public override BaseProjectileBehaviour AttachProjectileBehaviour(GameObject projectileBehaviourHolder)
        {
            var damageProjectileBehaviour = projectileBehaviourHolder.AddComponent<DamageProjectileBehaviour>();
            damageProjectileBehaviour.Initialize(this);
            return damageProjectileBehaviour;
        }
    }
}
