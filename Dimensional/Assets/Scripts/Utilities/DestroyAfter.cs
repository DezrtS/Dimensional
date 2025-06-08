using System;
using UnityEngine;

namespace Utilities
{
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField] private float delay;
        private void Awake()
        {
            Destroy(gameObject, delay);
        }
    }
}
