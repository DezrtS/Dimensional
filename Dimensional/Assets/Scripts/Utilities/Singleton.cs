using System;
using Interfaces;
using UnityEngine;

namespace Utilities
{
    public abstract class Singleton<T> : MonoBehaviour, ISingleton<T> where T : MonoBehaviour
    {
        public delegate void SingletonEventHandler(T singleton);
        public static event SingletonEventHandler Initialized;
        public static T Instance { get; private set; }
        
        protected virtual void OnEnable()
        {
            InitializeSingleton();
        }

        public virtual void InitializeSingleton()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"There were multiple instances of {name} in the scene");

                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            Initialized?.Invoke(Instance);
        }
    }

    public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour, ISingleton<T>
    {
        public override void InitializeSingleton()
        {
            base.InitializeSingleton();
            DontDestroyOnLoad(gameObject);
        }
    }
}