using Managers;
using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "StartQuestEventDatum", menuName = "Scriptable Objects/Events/StartQuestEventDatum")]
    public class StartQuestEventDatum : EventDatum
    {
        [SerializeField] private StartQuestEvent startQuestEvent;

        public override void HandleEvent()
        {
            EventManager.SendEvent(startQuestEvent);
        }
    }
}
