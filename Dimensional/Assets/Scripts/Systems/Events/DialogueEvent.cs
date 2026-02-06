using System;
using Managers;
using Scriptables.Dialogue;

namespace Systems.Events
{
    [Serializable]
    public class DialogueEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.Gameplay;
        public DialogueSequenceDatum DialogueSequence;

        public override void Handle()
        {
            DialogueManager.Instance.StartDialogueSequence(DialogueSequence);
        }
    }
}