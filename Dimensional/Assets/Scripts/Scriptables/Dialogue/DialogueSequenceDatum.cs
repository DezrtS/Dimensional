using System;
using FMODUnity;
using Scriptables.Events;
using Systems.Dialogue;
using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSequenceDatum", menuName = "Scriptable Objects/Dialogue/DialogueSequenceDatum")]
    public class DialogueSequenceDatum : ScriptableObject
    {
        [SerializeField] private bool isDialogueSkippable = true;
        [SerializeField] private EventDatum[] eventData;
        [Space]
        [SerializeField] private DialogueLine[] dialogueLines;
        
        public bool IsDialogueSkippable => isDialogueSkippable;
        public EventDatum[] EventData => eventData;
        
        public DialogueLine[] DialogueLines => dialogueLines;
    }
}
