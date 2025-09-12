using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Scriptables.Projectiles.Behaviours
{
    [CreateAssetMenu(fileName = "KnockbackProjectileBehaviourDatum", menuName = "Scriptable Objects/Projectiles/Behaviours/KnockbackProjectileBehaviourDatum")]
    public class KnockbackProjectileBehaviourDatum : BaseProjectileBehaviourDatum
    {
        [SerializeField] private float knockbackPower;
        [SerializeField] private KnockbackType knockbackType;
        [SerializeField] private bool addProjectileVelocity;
        
        public float KnockbackPower => knockbackPower;
        public KnockbackType KnockbackType => knockbackType;
        public bool AddProjectileVelocity => addProjectileVelocity;


        public override BaseProjectileBehaviour AttachProjectileBehaviour(GameObject projectileBehaviourHolder)
        {
            var knockProjectileBehaviour = projectileBehaviourHolder.AddComponent<KnockbackProjectileBehaviour>();
            knockProjectileBehaviour.Initialize(this);
            return knockProjectileBehaviour;
        }
    }
}
