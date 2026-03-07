using System;
using Managers;
using UnityEngine;

namespace Debugging
{
    public class StartStorm : MonoBehaviour
    {
        private bool _isStormy;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isStormy) return;

            if (!other.CompareTag("Player")) return;
            _isStormy = true;
            EffectManager.Instance.StartStorm();
        }
    }
}
