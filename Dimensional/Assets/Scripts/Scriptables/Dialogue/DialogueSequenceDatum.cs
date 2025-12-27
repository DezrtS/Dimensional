using System;
using FMODUnity;
using Systems.Dialogue;
using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSequenceDatum", menuName = "Scriptable Objects/Dialogue/DialogueSequenceDatum")]
    public class DialogueSequenceDatum : ScriptableObject
    {
        [SerializeField] private string fileName;
        [SerializeField] private bool isDialogueSkippable = true;
        
        [SerializeField] private DialogueSpeakerDatum[] dialogueSpeakersData;
        [SerializeField] private EventReference[] eventReferences;
        
        public string FileName => fileName;
        public bool IsDialogueSkippable => isDialogueSkippable;
        public DialogueSpeakerDatum[] DialogueSpeakersData => dialogueSpeakersData;
        public EventReference[] EventReferences => eventReferences;
    }
}
