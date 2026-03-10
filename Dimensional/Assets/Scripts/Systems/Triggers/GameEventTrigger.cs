using System;
using Managers;
using Scriptables.Events;
using UnityEngine;

namespace Systems.Triggers
{
    public class GameEventTrigger : MonoBehaviour
    {
        [SerializeField] private GameEventDatum[] gameEventData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            EventManager.HandleEvents(gameEventData);
        }
    }
}
