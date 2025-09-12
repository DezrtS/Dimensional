using Systems.Projectiles.Behaviours;
using UnityEngine;

namespace Scriptables.Projectiles.Behaviours
{
    public abstract class BaseProjectileBehaviourDatum : ScriptableObject
    {
        public abstract BaseProjectileBehaviour AttachProjectileBehaviour(GameObject projectileBehaviourHolder);
    }
}
