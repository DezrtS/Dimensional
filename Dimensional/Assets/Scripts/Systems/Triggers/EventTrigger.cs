using Managers;
using Scriptables.Events;
using UnityEngine;

namespace Systems.Triggers
{
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField] private EventDatum[] eventData;
        [SerializeField] private GameEventDatum[] gameEventData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (eventData is { Length: > 0 }) EventManager.SendEvents(eventData);
            if (gameEventData is { Length: > 0 }) EventManager.HandleEvents(gameEventData);
        }
    }
}
