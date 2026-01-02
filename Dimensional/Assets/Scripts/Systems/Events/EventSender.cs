using Managers;
using Scriptables.Events;
using UnityEngine;

namespace Systems.Events
{
    public class EventSender : MonoBehaviour
    {
        [SerializeField] private EventDatum[] eventData;

        public void SendEvents()
        {
            EventManager.SendEvents(eventData);
        }   
    }
}
