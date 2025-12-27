using System;
using Scriptables.Dialogue;
using UnityEngine;
using Utilities;

namespace Systems.Dialogue
{
    [Serializable]
    public class DialogueLine
    {
        [SerializeField] private DialogueSpeakerDatum dialogueSpeakerDatum;
        [TextArea(3, 10)] [SerializeField] private string text;
        [Space] 
        [SerializeField] private bool hasVoiceActing;
        [SerializeField] private EventReferenceWrapper eventReference;
        
        public DialogueSpeakerDatum DialogueSpeakerDatum => dialogueSpeakerDatum;
        public string Text => text;
        public bool HasVoiceActing => hasVoiceActing;
        public EventReferenceWrapper EventReferenceWrapper => eventReference;
    }
}
