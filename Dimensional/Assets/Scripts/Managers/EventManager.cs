using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.Events;
using Systems.Events;
using Systems.Events.Busses;
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

        public static void SendEvents(IEnumerable<GameEvent> gameEvents)
        {
            
        }
        
        public static void SendEvent(EventDatum eventDatum)
        {
            //Debug.Log($"Triggered Event - {eventDatum.EventName}");
            eventDatum.HandleEvent();
        }
        
        public static void SendEvent(GameEvent gameEvent)
        {
            switch (gameEvent.BusType)
            {
                case EventBusType.Game:
                    break;
                case EventBusType.World:
                    break;
                case EventBusType.Quest:
                    QuestEventBus.Fire(gameEvent);
                    break;
                case EventBusType.UI:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerator HandleEventsRoutine(IEnumerable<EventDatum> eventData)
        {
            foreach (var eventDatum in eventData)
            {
                SendEvent(eventDatum);
                yield return new WaitForSeconds(eventDatum.Duration);
            }
        }
        
        private static IEnumerator HandleEventsRoutine(IEnumerable<GameEvent> gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                SendEvent(gameEvent);
                yield return new WaitForSeconds(gameEvent.Duration);
            }
        }
    }
}
