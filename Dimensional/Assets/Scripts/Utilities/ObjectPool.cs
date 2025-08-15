using System.Collections.Generic;
using Scriptables.Utilities;
using UnityEngine;

namespace Utilities
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool;
        private readonly HashSet<T> _activePool;
        
        private readonly GameObject _poolObject;
        private readonly GameObject _prefab;

        private ObjectPoolDatum ObjectPoolDatum { get; set; }
        
        public ObjectPool(ObjectPoolDatum objectPoolDatum, GameObject prefab, Transform parent)
        {
            _pool = new Queue<T>();
            _activePool = new HashSet<T>();
            
            ObjectPoolDatum = objectPoolDatum;
            _poolObject = new GameObject($"{objectPoolDatum.PoolName}");
            _poolObject.transform.SetParent(parent);
            _prefab = prefab;

            for (var i = 0; i < objectPoolDatum.ObjectPoolSize; i++)
            {
                var instance = Object.Instantiate(prefab, _poolObject.transform);
                instance.name = $"{objectPoolDatum.ObjectName} {i}";
                instance.SetActive(false);
                _pool.Enqueue(instance.GetComponent<T>());
            }
        }

        public T GetObject()
        {
            if (_pool.Count > 0)
            {
                var activeObject = _pool.Dequeue();
                activeObject.gameObject.SetActive(true);
                _activePool.Add(activeObject);
                return activeObject;
            }
            else if (ObjectPoolDatum.IsDynamic)
            {
                var instance = Object.Instantiate(_prefab, _poolObject.transform);
                instance.name = $"{ObjectPoolDatum.ObjectName} {_activePool.Count + 1}";
                var activeObject = instance.GetComponent<T>();
                _activePool.Add(activeObject);
                return activeObject;
            }

            return null;
        }

        public void ReturnToPool(T objectToReturn)
        {
            _activePool.Remove(objectToReturn);

            if (ObjectPoolDatum.DestroyObjectsOverPoolSize)
            {
                if (_pool.Count > ObjectPoolDatum.ObjectPoolSize)
                {
                    Object.Destroy(objectToReturn.gameObject);
                }
            }
            
            objectToReturn.gameObject.SetActive(false);
            _pool.Enqueue(objectToReturn);
        }
    }
}