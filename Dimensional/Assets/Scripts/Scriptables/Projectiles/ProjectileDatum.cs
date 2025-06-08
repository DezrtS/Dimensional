using Interfaces;
using Systems.Projectiles;
using UnityEngine;

namespace Scriptables.Projectiles
{
    [CreateAssetMenu(fileName = "ProjectileDatum", menuName = "Scriptable Objects/Projectiles/ProjectileDatum")]
    public class ProjectileDatum : ScriptableObject
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int damage;
        [SerializeField] private float lifetimeDuration;

        public GameObject ProjectilePrefab => projectilePrefab;
        public int Damage => damage;
        public float LifetimeDuration => lifetimeDuration;
    
        public IProjectile<Projectile> Spawn()
        {
            var projectileObject = Instantiate(projectilePrefab);
            var projectile = projectileObject.GetComponent<IProjectile<Projectile>>();
            projectile.Initialize(this);
            return projectile;
        }
    }
}
