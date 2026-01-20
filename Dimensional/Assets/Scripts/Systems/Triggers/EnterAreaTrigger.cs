using System;
using Managers;
using Systems.Events;
using UnityEngine;

namespace Systems.Triggers
{
    public class EnterAreaTrigger : MonoBehaviour
    {
        [SerializeField] private string areaId;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            EventManager.SendEvent(new FlagEvent { FlagId = areaId });
        }
    }
}
