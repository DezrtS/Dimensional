using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueLineDatum", menuName = "Scriptable Objects/Dialogue/DialogueLineDatum")]
    public class DialogueLineDatum : ScriptableObject
    {
        [SerializeField] private DialogueSpeakerDatum dialogueSpeakerDatum;
        [TextArea(3, 10)] [SerializeField] private string text;
        
        public DialogueSpeakerDatum DialogueSpeakerDatum => dialogueSpeakerDatum;
        public string Text => text;
    }
}
