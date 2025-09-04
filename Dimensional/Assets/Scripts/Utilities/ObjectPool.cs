using System.Collections.Generic;
using Interfaces;
using Scriptables.Utilities;
using UnityEngine;

namespace Utilities
{
    public class ObjectPool<T> where T : MonoBehaviour, IObjectPoolable<T>
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
                var instanceObject = Object.Instantiate(prefab, _poolObject.transform);
                instanceObject.name = $"{objectPoolDatum.ObjectName} {i}";
                instanceObject.SetActive(false);
                var instance = instanceObject.GetComponent<T>();
                instance.Returned += ReturnToPool;
                _pool.Enqueue(instance);
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
                var instanceObject = Object.Instantiate(_prefab, _poolObject.transform);
                instanceObject.name = $"{ObjectPoolDatum.ObjectName} {_activePool.Count + 1}";
                var instance = instanceObject.GetComponent<T>();
                instance.Returned += ReturnToPool;
                _activePool.Add(instance);
                return instance;
            }

            return null;
        }

        private void ReturnToPool(T objectToReturn)
        {
            _activePool.Remove(objectToReturn);

            if (ObjectPoolDatum.DestroyObjectsOverPoolSize)
            {
                if (_pool.Count > ObjectPoolDatum.ObjectPoolSize)
                {
                    objectToReturn.Returned -= ReturnToPool;
                    Object.Destroy(objectToReturn.gameObject);
                }
            }
            
            objectToReturn.gameObject.SetActive(false);
            _pool.Enqueue(objectToReturn);
        }
    }
}