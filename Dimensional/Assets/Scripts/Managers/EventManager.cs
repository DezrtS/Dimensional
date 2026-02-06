using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Events;
using Systems.Events;
using Systems.Events.Busses;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class EventManager : Singleton<EventManager>
    {
        public static void HandleEvents(IEnumerable<GameEvent> gameEvents)
        {
            Instance.StartCoroutine(HandleEventsRoutine(gameEvents));
        }
        
        public static void HandleEvents(IEnumerable<GameEventDatum> gameEventData)
        {
            Instance.StartCoroutine(HandleEventsRoutine(ConvertGameEvents(gameEventData)));
        }
        
        public static List<GameEvent> ConvertGameEvents(IEnumerable<GameEventDatum> gameEventData)
        {
            return gameEventData.Select(gameEventDatum => gameEventDatum.GameEvent).ToList();
        }
        
        private static IEnumerator HandleEventsRoutine(IEnumerable<GameEvent> gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.Handle();
                yield return new WaitForSeconds(gameEvent.Duration);
            }
        }
        
        
        
        public static void SendEvents(IEnumerable<EventDatum> eventData)
        {
            Instance.StartCoroutine(HandleEventsRoutine(eventData));
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
                case EventBusType.Gameplay:
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
    }
}
