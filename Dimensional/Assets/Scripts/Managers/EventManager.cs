using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.Events;
using UnityEngine;
using Utilities;
using EventType = Scriptables.Events.EventType;

namespace Managers
{
    public struct CustomEventData
    {
        public EventType EventType;
        public string EventName;
        
        public Dictionary<string, object> EventParameters;
    }
    
    public class EventManager : Singleton<EventManager>
    {
        public static Action<CustomEventData> CustomEventSent;
        
        public static void SendEvents(IEnumerable<EventDatum> eventData)
        {
            Instance.StartCoroutine(HandleEventsRoutine(eventData));
        }
        
        public static void SendEvent(EventDatum eventDatum)
        {
            //Debug.Log($"Triggered Event - {eventDatum.EventName}");
            eventDatum.HandleEvent();
        }

        private static IEnumerator HandleEventsRoutine(IEnumerable<EventDatum> eventData)
        {
            foreach (var eventDatum in eventData)
            {
                SendEvent(eventDatum);
                yield return new WaitForSeconds(eventDatum.Duration);
            }
        }
    }
}
