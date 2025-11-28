using System.Collections;
using System.Collections.Generic;
using Scriptables.Events;
using Utilities;

namespace Managers
{
    public class EventManager : Singleton<EventManager>
    {
        public static void SendEvents(IEnumerable<EventDatum> eventData)
        {
            foreach (var eventDatum in eventData)
            {
                SendEvent(eventDatum);
            }
        }
        
        public static void SendEvent(EventDatum eventDatum)
        {
            //Debug.Log($"Triggered Event - {eventDatum.EventName}");
            eventDatum.HandleEvent();
        }
    }
}
