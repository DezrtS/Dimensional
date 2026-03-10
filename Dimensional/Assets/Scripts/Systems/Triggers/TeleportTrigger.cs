using System;
using UnityEngine;

namespace Systems.Triggers
{
    public class TeleportTrigger : MonoBehaviour
    {
        [SerializeField] private Transform teleportTarget;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
        }
    }
}
