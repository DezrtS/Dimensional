using Scriptables.Projectiles;
using Scriptables.Utilities;
using Systems.Actions;
using UnityEngine;
using Action = Systems.Actions.Action;

namespace Scriptables.Actions
{
    [CreateAssetMenu(fileName = "ProjectileActionDatum", menuName = "Scriptable Objects/Actions/ProjectileActionDatum")]
    public class ProjectileActionDatum : ActionDatum
    {
        [SerializeField] private ObjectPoolDatum projectilePoolDatum;
        [SerializeField] private BaseProjectileDatum baseProjectileDatum;
        
        public ObjectPoolDatum ProjectilePoolDatum => projectilePoolDatum;
        public BaseProjectileDatum ProjectileDatum => baseProjectileDatum;

        public override Action AttachAction(GameObject actionHolder)
        {
            var projectileAction = actionHolder.AddComponent<ProjectileAction>();
            projectileAction.Initialize(this);
            return projectileAction;
        }
    }
}
