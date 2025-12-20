using System;
using Systems.Dialogue;
using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSequenceDatum", menuName = "Scriptable Objects/Dialogue/DialogueSequenceDatum")]
    public class DialogueSequenceDatum : ScriptableObject
    {
        [SerializeField] private string sequenceKey;
        [SerializeField] private bool isDialogueSkippable;
        [Space] 
        [SerializeField] private DialogueLine[] dialogueLines;
        
        public string SequenceKey => sequenceKey;
        public bool IsDialogueSkippable => isDialogueSkippable;
        
        public DialogueLine[] DialogueLines => dialogueLines;
    }
}
