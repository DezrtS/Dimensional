using System;
using UnityEngine;

namespace Scriptables.Dialogue
{
    [Serializable]
    public struct DialogueChoice
    {
        [SerializeField] private string text;
        [SerializeField] private DialogueSequenceDatum dialogueSequenceDatum;
        
        public string Text => text;
        public DialogueSequenceDatum DialogueSequenceDatum => dialogueSequenceDatum;
    }
    
    [CreateAssetMenu(fileName = "DialogueSequenceDatum", menuName = "Scriptable Objects/Dialogue/DialogueSequenceDatum")]
    public class DialogueSequenceDatum : ScriptableObject
    {
        [SerializeField] private string sequenceKey;
        [SerializeField] private bool isDialogueSkippable;
        [Space]
        [SerializeField] private DialogueLineDatum[] dialogueLineData;
        [SerializeField] private DialogueChoice[] dialogueChoices;
        
        public string SequenceKey => sequenceKey;
        public bool IsDialogueSkippable => isDialogueSkippable;
        public DialogueLineDatum[] DialogueLineData => dialogueLineData;
        public DialogueChoice[] DialogueChoices => dialogueChoices;
    }
}
