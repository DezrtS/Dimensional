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
        private readonly ISpawnPoolableObjects<T> _spawner;
        private readonly bool _useSpawner;
        
        private ObjectPoolDatum ObjectPoolDatum { get; set; }
        
        public ObjectPool(ObjectPoolDatum objectPoolDatum, GameObject prefab, Transform parent)
        {
            _pool = new Queue<T>();
            _activePool = new HashSet<T>();
            
            ObjectPoolDatum = objectPoolDatum;
            _poolObject = new GameObject($"{objectPoolDatum.PoolName}");
            _poolObject.transform.SetParent(parent);
            _prefab = prefab;
            _useSpawner = false;

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

        public ObjectPool(ObjectPoolDatum objectPoolDatum, ISpawnPoolableObjects<T> spawner, Transform parent)
        {
            _pool = new Queue<T>();
            _activePool = new HashSet<T>();
            
            ObjectPoolDatum = objectPoolDatum;
            _poolObject = new GameObject($"{objectPoolDatum.PoolName}");
            _poolObject.transform.SetParent(parent);
            _spawner = spawner;
            _useSpawner = true;

            for (var i = 0; i < objectPoolDatum.ObjectPoolSize; i++)
            {
                var instance = spawner.Spawn();
                instance.transform.SetParent(_poolObject.transform);
                instance.name = $"{objectPoolDatum.ObjectName} {i}";
                instance.gameObject.SetActive(false);
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

            if (!ObjectPoolDatum.IsDynamic) return null;
            
            if (_useSpawner)
            {
                var instance = _spawner.Spawn();
                instance.transform.SetParent(_poolObject.transform);
                instance.name = $"{ObjectPoolDatum.ObjectName} {_activePool.Count + 1}";
                instance.Returned += ReturnToPool;
                _activePool.Add(instance);
                return instance;
            }
            else
            {
                var instanceObject = Object.Instantiate(_prefab, _poolObject.transform);
                instanceObject.name = $"{ObjectPoolDatum.ObjectName} {_activePool.Count + 1}";
                var instance = instanceObject.GetComponent<T>();
                instance.Returned += ReturnToPool;
                _activePool.Add(instance);
                return instance;   
            }
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

        public void RecallPool()
        {
            var activeObjects = new List<T>(_activePool);
            foreach (var activeObject in activeObjects)
            {
                activeObject.ReturnToPool();
            }
        }

        public void DestroyObjectPool()
        {
            RecallPool();
            while (_pool.Count > 0)
            {
                var instance = _pool.Dequeue();
                instance.Returned -= ReturnToPool;
                Object.Destroy(instance.gameObject);
            }

            Object.Destroy(_poolObject);
        }
    }
}