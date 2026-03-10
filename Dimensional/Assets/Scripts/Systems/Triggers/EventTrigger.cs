using Managers;
using Scriptables.Events;
using UnityEngine;

namespace Systems.Triggers
{
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField] private EventDatum[] eventData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            EventManager.SendEvents(eventData);
        }
    }
}
