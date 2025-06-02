using UnityEngine;

namespace Scriptables.Object_Pools
{
    [CreateAssetMenu(fileName = "ObjectPoolDatum", menuName = "Scriptable Objects/Object Pools/ObjectPoolDatum")]
    public class ObjectPoolDatum : ScriptableObject
    {
        [SerializeField] private string poolName;
        [SerializeField] private string objectName;
        [SerializeField] private int objectPoolSize;
        [SerializeField] private bool isDynamic;
        [SerializeField] private bool destroyObjectsOverPoolSize;
        
        public string PoolName => poolName;
        public string ObjectName => objectName;
        public int ObjectPoolSize => objectPoolSize;
        public bool IsDynamic => isDynamic;
        public bool DestroyObjectsOverPoolSize => destroyObjectsOverPoolSize;
    }
}
