using UnityEngine;

namespace Scriptables.Entities
{
    [CreateAssetMenu(fileName = "EntityDatum", menuName = "Scriptable Objects/Entities/EntityDatum")]
    public class EntityDatum : ScriptableObject
    {
        public enum Type
        {
            None,
            Player,
            Npc,
            Enemy,
            Boss,
            Object,
            Item,
            Environment
        }

        [SerializeField] private string entityName;
        [SerializeField] private Type entityType;
        
        public string EntityName => entityName;
        public Type EntityType => entityType;
    }
}
