using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "DialogueEventDatum", menuName = "Scriptable Objects/Events/DialogueEventDatum")]
    public class DialogueEventDatum : GameEventDatum
    {
        [SerializeField] private DialogueEvent dialogueEvent;

        public override GameEvent GameEvent => dialogueEvent;
    }
}
