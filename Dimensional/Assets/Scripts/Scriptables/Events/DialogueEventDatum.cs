using Managers;
using Scriptables.Dialogue;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "DialogueEventDatum", menuName = "Scriptable Objects/Events/DialogueEventDatum")]
    public class DialogueEventDatum : EventDatum
    {
        [SerializeField] private DialogueSequenceDatum dialogueSequenceDatum;
        
        public override void HandleEvent()
        {
            DialogueManager.Instance.StartDialogueSequence(dialogueSequenceDatum);
        }
    }
}
