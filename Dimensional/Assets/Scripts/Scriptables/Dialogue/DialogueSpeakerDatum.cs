using UnityEngine;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSpeakerDatum", menuName = "Scriptable Objects/Dialogue/DialogueSpeakerDatum")]
    public class DialogueSpeakerDatum : ScriptableObject
    {
        [SerializeField] private string speakerKey;
        [SerializeField] private string speakerName;
        [SerializeField] private bool canTalk;
        
        public string SpeakerKey => speakerKey;
        public string SpeakerName => speakerName;
        public bool CanTalk => canTalk;
    }
}