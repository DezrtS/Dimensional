using System;
using FMOD.Studio;
using Mono.Cecil;
using Scriptables.Dialogue;
using UnityEngine;

namespace Systems.Dialogue
{
    [Serializable]
    public class DialogueLine
    {
        [SerializeField] private DialogueSpeakerDatum dialogueSpeakerDatum;
        [TextArea(3, 10)] [SerializeField] private string text;
        
        public DialogueSpeakerDatum DialogueSpeakerDatum => dialogueSpeakerDatum;
        public string Text => text;
    }
}
